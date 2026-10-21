import { Component, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { UploadService } from '../../service/Upload/upload-service';
import { PermissaoUploadResponse } from '../../models/upload.model';

// Configuração do componente Angular no padrão standalone (sem necessidade de NgModule)
@Component({
  selector: 'app-upload',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './upload.page.html',
  styleUrl: './upload.page.scss',
})
export class UploadPage {

  // Obtém a referência direta do elemento <input type="file"> oculto no template HTML
  @ViewChild('fileInput') fileInputRef!: ElementRef<HTMLInputElement>;

  // Identificador textual da biblioteca ou repositório de destino
  libraryName: string = 'Biblioteca Principal';

  // Configuração do formulário
  selectedFile: File | null = null;
  isDragOver: boolean = false; // Controla o feedback visual na área de arrastar e soltar (drag and drop)

  // Estado e monitoramento do progresso do upload
  isUploading: boolean = false;
  progressPercent: number = 0;
  progressText: string = 'Aguardando pedaços...';

  // Mensagens e Status
  statusMessage: string = '';
  statusType: 'success' | 'danger' | '' = '';


  // Injeção de dependência do serviço de upload
  constructor(private uploadService: UploadService) { }

  // Disparado quando o usuário arrasta um arquivo para dentro da área designada
  onDragOver(event: DragEvent): void {
    event.preventDefault(); // Impede o navegador de abrir o arquivo diretamente na aba
    this.isDragOver = true;
  }

  // Disparado quando o arquivo arrastado sai dos limites da área designada
  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = false;
  }

  // Disparado quando o usuário solta o arquivo na área de arrasto
  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = false;
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.selectedFile = files[0]; // Captura apenas o primeiro arquivo arrastado
    }
  }

  // Captura o evento nativo de seleção de arquivo via janela de navegação (input de arquivo)
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  // Aciona o clique no elemento <input type="file"> invisível através de um botão visual
  openFileDialog(): void {
    this.fileInputRef.nativeElement.click();
  }

  // Função principal que orquestra o início da requisição e envio dos pedaços
  async iniciarUpload(): Promise<void> {

    // Verifica se caso nenhum arquivo tenha sido selecionado
    if (!this.selectedFile) return;

    this.resetInterface();
    this.progressText = 'Obtendo rota de upload...';

    // 1º Passo: Solicita a permissão/URL de upload para a API
    this.uploadService.obterUrlUpload().subscribe({

      next: async (permissao) => {
        try {

          // 2º Passo: Dispara o envio paralelo passando o arquivo, URL e a callback de progresso
          const resultado = await this.uploadService.uploadParalelo(
            this.selectedFile!,
            permissao.url,
            (progress) => {

              // Atualiza o progresso na interface conforme cada pedaço é finalizado
              this.progressPercent = progress.percentual;
              this.progressText = `Pedaço ${progress.concluidos} de ${progress.total} concluído`;
              console.log(`Pedaço ${progress.concluidos} de ${progress.total} concluído`);
            }
          );

          // Trata o desfecho positivo reportado pelo serviço
          if (resultado.sucesso) {
            console.log('[Upload Finalizado]');
            this.showStatus(`Upload concluído com sucesso! Arquivo: ${resultado.nameFile}`, 'success');
          }
        } catch (error: any) {

          // Trata exceções lançadas durante o envio dos pedaços (ex: estouro de retentativas)
          this.showStatus(error.message || 'Erro durante o envio dos pedaços.', 'danger');
        } finally {
          
          // Garante que o indicador de upload em andamento seja desativado ao término
          this.isUploading = false;
        }
      },
      // Tratamento de falha ao tentar obter a URL de autorização inicial
      error: (err) => {
        console.error('Erro ao obter URL:', err);
        this.showStatus('Falha ao obter URL de upload da API.', 'danger');
        this.isUploading = false;
      }
    });

  }


  // Limpa estados e prepara as variáveis de exibição para um novo ciclo de upload
  private resetInterface(): void {
    this.isUploading = true;
    this.progressPercent = 0;
    this.statusMessage = '';
    this.statusType = '';
  }

  // Exibe a mensagem de retorno na interface e agenda sua remoção automática após 5 segundos
  private showStatus(msg: string, type: 'success' | 'danger'): void {
    this.statusMessage = msg;
    this.statusType = type;

    setTimeout(() => {
      this.statusMessage = '';
      this.statusType = '';

      // Limpa a seleção do arquivo e reseta o input nativo caso o envio tenha tido sucesso
      if (type === 'success') {
        this.selectedFile = null;
        if (this.fileInputRef) {
          this.fileInputRef.nativeElement.value = '';
        }
      }
    }, 5000);
  }



  
}


