import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom, from } from 'rxjs';
import SparkMD5 from 'spark-md5';
import { PermissaoUploadResponse, ChunkProgress, UploadFinalResult } from '../../models/upload.model';
import { environment } from '../../../environments/environment';

// Decorator que registra o serviço como singleton disponível em toda a aplicação (root)
@Injectable({
    providedIn: 'root'
})
export class UploadService {

    // Configurações padrão de particionamento e controle de concorrência
    private readonly chunkSize = 5 * 1024 * 1024;  // Tamanho de cada parte (5 MB em bytes)
    private readonly concurrencyLimit = 8;         // Máximo de requisições de upload simultâneas
    private readonly maxRetriesPerChunk = 3;       // Quantidade máxima de tentativas para cada pedaço
    private readonly EndPoint = `${environment.apiUrl}`;
    constructor(private http: HttpClient) { }

    // btém a URL base de upload da API
    // Faz uma requisição GET simples para recuperar o endpoint correto de envio
    obterUrlUpload(): Observable<PermissaoUploadResponse> {
        const Url = `${this.EndPoint}/Arquivos/upload`;
        return this.http.get<PermissaoUploadResponse>(Url);
    }


    //Executa o upload paralelo com retries e cálculo de integridade MD5
    //formatado para o DTO UploadChunkDTO
    uploadParalelo(file: File, targetUrl: string, onProgress: (progress: ChunkProgress) => void): Promise<UploadFinalResult> {
           
        // Calcula o número total de pedaços necessários para cobrir todo o arquivo
        const totalChunks = Math.ceil(file.size / this.chunkSize);

        // Fila com os metadados de cada chunk: seu índice e o contador de tentativas
        const chunksQueue = Array.from({ length: totalChunks }, (_, i) => ({ index: i, retryCount: 0 }));

        // Array booleano para rastrear se cada chunk específico foi enviado e confirmado com sucesso
        const chunksConfirmados = Array.from({ length: totalChunks }, () => false);

        // Separação do nome do arquivo e de sua extensão para o backend
        const lastDotIndex = file.name.lastIndexOf('.');
        const fileNameOnly = lastDotIndex !== -1 ? file.name.substring(0, lastDotIndex) : file.name;
        const extension = lastDotIndex !== -1 ? file.name.substring(lastDotIndex) : '';

        // Flags para controlar encerramento imediato em caso de erro fatal e armazenar o nome gerado pelo servidor
        let erroCriticoDisparado = false;
        let finalNameFile: string | undefined;
        let finalToken: string | undefined;
        
        return new Promise(async (resolve, reject) => {

            // Função de trabalho assíncrona ("worker") que consome itens da fila recursivamente
            const uploadNextChunk = async (): Promise<void> => {

                // Se não há mais partes na fila ou se ocorreu erro crítico em outro worker, encerra a execução                
                if (chunksQueue.length === 0 || erroCriticoDisparado) return;

                // Retira o próximo chunk da fila
                const chunkItem = chunksQueue.shift()!;
                const i = chunkItem.index;
           
                // Prevenção de duplicidade: caso o chunk já tenha sido confirmado, passa para o próximo
                if (chunksConfirmados[i]) {
                    await uploadNextChunk();
                    return;
                }

                // Delimita os bytes de início e fim da fatia (slice) deste pedaço
                const start = i * this.chunkSize;
                const end = Math.min(file.size, start + this.chunkSize);
                const chunkBlob = file.slice(start, end);
                             

                try {

                    // Calcula o hash MD5 apenas deste pedaço binário para validação de integridade no servidor
                    const chunkMd5 = await this.calcularMd5Chunk(chunkBlob);

                    // Montagem do payload Multipart/Form-Data correspondente ao DTO esperado
                    const formData = new FormData();
                    formData.append('ChunkIndex', (i + 1).toString());
                    formData.append('TotalChunks', totalChunks.toString());
                    formData.append('FileName', fileNameOnly);
                    formData.append('extension', extension);
                    formData.append('chunkMd5', chunkMd5);
                    formData.append('File', chunkBlob, file.name);

                    // Envio do pedaço para a API externa
                    // Envia a requisição HTTP POST e converte o Observable do HttpClient em Promise
                    const response = await firstValueFrom(
                        this.http.post<{ completed?: boolean; nameFile?: string; token?: string }>(targetUrl, formData)
                    );

                    // Marca este chunk como enviado e confirmado
                    chunksConfirmados[i] = true;

                    // Calcula o progresso atual com base no número total de chunks concluídos
                    const totalConfirmados = chunksConfirmados.filter(Boolean).length;
                    
                    onProgress({
                        concluidos: totalConfirmados,
                        total: totalChunks,
                        percentual: Math.round((totalConfirmados / totalChunks) * 100)
                    });
                
                    // Captura o retorno final quando o upload é concluído
                    if (response?.completed) {
                        if (response.token)
                            finalToken = response.token;

                    }

                    // Processa o próximo chunk disponível na fila
                    await uploadNextChunk();

                } catch (error: any) {

                    // Em caso de falha transitória, tenta reenfileirar o chunk caso não tenha atingido o limite
                    if (chunkItem.retryCount < this.maxRetriesPerChunk && !erroCriticoDisparado) {
                        chunkItem.retryCount++;
                        chunksQueue.push(chunkItem); // Devolve o chunk ao fim da fila
                        await uploadNextChunk();
                    } else if (!erroCriticoDisparado) {
                        // Limite de tentativas excedido: aborta todo o fluxo para evitar uploads corrompidos/incompletos
                        erroCriticoDisparado = true;
                        reject(new Error(`O pedaço ${i} falhou repetidas vezes. Upload cancelado.`));
                    }
                }
            };


            // Define o pool de conexões paralelas ativas respeitando o limite configurado
            const totalWorkersAtivos = Math.min(this.concurrencyLimit, totalChunks);
            const workers = Array.from({ length: totalWorkersAtivos }, () => uploadNextChunk());

            try {
                // Aguarda a finalização de todas as threads concorrentes de upload
                await Promise.all(workers);

                // Valida se todos os pedaços foram concluídos com sucesso antes de resolver
                const sucesso = chunksConfirmados.every(Boolean);
                       
                resolve({
                    sucesso,
                    nameFile: finalNameFile,
                    completed: sucesso,
                    token: finalToken
                });
                
            } catch (err) {
                reject(err);
            }
        });
    }
    
    // Método auxiliar privado que lê os bytes de um Blob e calcula a soma de verificação MD5
    private calcularMd5Chunk(chunkBlob: Blob): Promise<string> {

        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            const spark = new SparkMD5.ArrayBuffer();

            // Callback disparado quando o FileReader termina de carregar o buffer em memória
            reader.onload = (e: ProgressEvent<FileReader>) => {
                if (e.target?.result) {
                    spark.append(e.target.result as ArrayBuffer);
                    resolve(spark.end()); // Retorna o hash MD5 em formato hexadecimal
                } else {
                    reject(new Error('Buffer vazio.'));
                }
            };

            // Callback disparado em caso de erro na leitura do arquivo pelo navegador
            reader.onerror = () => reject(new Error('Erro ao processar bytes do chunk para MD5.'));
            reader.readAsArrayBuffer(chunkBlob);
        });
    }
}