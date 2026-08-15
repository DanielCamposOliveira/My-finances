import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http'; // Importa o provedor HTTP
import { routes } from './app.routes';
import { authInterceptor } from './service/authGuard/auth.interceptor'; // Importe o interceptor que criamos

export const appConfig: ApplicationConfig = {
  providers: [

    // Ativa o modo Zoneless
    provideZonelessChangeDetection(),

    provideBrowserGlobalErrorListeners(),

    // Sistema de rotas
    provideRouter(routes),

    // Cliente HTTP com o interceptor de seguranca injetado
    provideHttpClient(
      withInterceptors([authInterceptor])
    )    
  ]
  
};
