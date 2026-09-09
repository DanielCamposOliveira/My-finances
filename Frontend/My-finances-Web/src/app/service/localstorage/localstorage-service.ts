import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class LocalstorageService {

    historyMonthly(): boolean {
        const registro = localStorage.getItem('generator');
        const hoje = new Date();

        if (registro === null) {
            const data = hoje.toISOString().split('T')[0];
            localStorage.setItem('generator', data);
            return false;
        }

        const dataRegistro = new Date(registro);
        const mesmoMes =
            dataRegistro.getMonth() === hoje.getMonth() &&
            dataRegistro.getFullYear() === hoje.getFullYear();

        if (mesmoMes) {
            return true;
        }

        const dataAtual = hoje.toISOString().split('T')[0];
        localStorage.setItem('generator', dataAtual);
        return false;

    }

   
    // ler o valor da chave
    isDarkMode(): boolean {
        // 1. Obtém o valor como string (ou null se não existir)
        const darkMode = localStorage.getItem('darkMode');

        // 2. Se for null (não existe), cria com 'false' e retorna false
        if (darkMode === null) {
            localStorage.setItem('darkMode', 'false');
            return false;
        }

        // 3. Converte a string salva ('true'/'false') para boolean
        return darkMode === 'true';
    }
  
    // var novo valor na chave
    setDarkMode(value: boolean): void {
        // Converte o boolean para string e grava no localStorage
        localStorage.setItem('darkMode', String(value));
    }




    // remove a chave
    removeDakMode(): void {
        localStorage.removeItem('darkMode');
    }
    
    // remove o registro do generator
    removeHistoryMonthly(): void {
        localStorage.removeItem('generator');
    }
}


