import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

import { MainLayoutComponent } from './layout/main-layout/main-layout';
import { AuthService } from './service/Authentication/auth.service';
import { Dashboard } from './pages/dashboard/dashboard';

const authGuard = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.estaAutenticado()) {
    return true;
  }

  return router.createUrlTree(['/login']);
};

export const routes: Routes = [

  // Layout Principal (com Header + RouterOutlet)
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        component: Dashboard
      },
      {
        path: 'cadastro',
        // Exemplo: componente de cadastro
       loadComponent: () => import('./pages/cadastro/cadastro').then(m => m.Cadastro) 
      }
    ]
  },
  // Rotas Públicas
  { 
    path: 'login', 
    loadComponent: () => import('./pages/login/login').then(m => m.Login) // Ou o caminho correto do seu login
  },
  { 
    path: 'registre', 
    loadComponent: () => import('./pages/registre-user/registre-user').then(m => m.RegistreUser) 
  },

  // Rota Coringa (Redireciona URLs desconhecidas para o pagina dashboard)
  { 
    path: '**', 
    redirectTo: 'dashboard' 
  }
];