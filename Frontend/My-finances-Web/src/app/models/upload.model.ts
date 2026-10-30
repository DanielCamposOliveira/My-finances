// Interface que define o contrato da resposta ao solicitar permissão de upload
export interface PermissaoUploadResponse {
  url: string;
}

// Interface usada para emitir o progresso em tempo real para quem consome o serviço
export interface ChunkProgress {
  concluidos: number;
  total: number;
  percentual: number;
}

// Interface para a resposta da API ao completar
export interface UploadFinalResult {
  sucesso: boolean;
  nameFile?: string;
  completed?: boolean;
  token?: string;
}