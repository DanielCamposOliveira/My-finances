import { Component, inject, signal, OnInit, AfterViewInit, NgZone, ElementRef, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router'; 
import { AuthService } from '../../service/Authentication/auth.service';
import { environment } from '../../../environments/environment';

declare const google: any;


@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink], // Importado RouterLink aqui
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login implements OnInit, AfterViewInit {
  @ViewChild('googleButton', { static: false }) googleButton!: ElementRef;

  email = '';
  senha = '';

  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private ngZone = inject(NgZone);
  
  mensagemErro = signal<string | null>(null);
  estaCarregando = signal<boolean>(false);
  private googleInicializado = false;
  
  ngOnInit(): void {
    
    // Remove as classes de tema para que o body volte a respeitar o :root puro
    document.body.classList.remove('light-theme', 'dark-theme');

    // verifica se o usuario esta logado, caso esteja redireciona para Pagina dashboard
    if (this.authService.estaAutenticado()) {
      this.router.navigate(['/dashboard']);
    }
  }

  ngAfterViewInit(): void {
    this.carregarScriptGoogle();
  }

  private carregarScriptGoogle(): void {
      if (typeof google !== 'undefined' && google.accounts) {
        this.inicializarGoogle();
        return;
      }

      const scriptExistente = document.querySelector('script[src="https://accounts.google.com/gsi/client"]');
      if (!scriptExistente) {
        const script = document.createElement('script');
        script.src = 'https://accounts.google.com/gsi/client';
        script.async = true;
        script.defer = true;
        script.onload = () => this.inicializarGoogle();
        document.head.appendChild(script);
      } else {
        const intervalo = setInterval(() => {
          if (typeof google !== 'undefined' && google.accounts) {
            clearInterval(intervalo);
            this.inicializarGoogle();
          }
        }, 100);
      }
    }

private inicializarGoogle(): void {
    if (this.googleInicializado) return;

google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: any) => this.handleGoogleCredentialResponse(response),
      auto_select: false,
      ux_mode: 'popup'
    });

    this.googleInicializado = true;

    // Renderiza o botão oficial em um container oculto/customizado para garantir abertura sem restrição de clique
    if (this.googleButton) {
      google.accounts.id.renderButton(this.googleButton.nativeElement, {
      theme: 'outline',
      size: 'large',
      width: 350,
      type: 'standard',
      text: 'signin_with',
      shape: 'rectangular',
      logo_alignment: 'left'
      });
    }
  }

  private handleGoogleCredentialResponse(response: any): void {

    if (!response?.credential) {
      this.mensagemErro.set('Nenhuma credencial recebida do Google.');
      return;
    }
    
    this.ngZone.run(() => {
      this.estaCarregando.set(true);
      this.mensagemErro.set(null);

      this.authService.LoginComGoogle({ idToken: response.credential }).subscribe({
        next: () => {
          this.estaCarregando.set(false);
          this.router.navigate(['/dashboard']);
        },
        error: (erro) => {
          this.estaCarregando.set(false);
          if (erro.status >= 400 && erro.status < 500) {
            this.mensagemErro.set(erro.error?.message || 'Falha ao autenticar com o Google.');
          } else {
            this.mensagemErro.set('Não foi possível conectar ao servidor. Tente novamente mais tarde.');
          }
        }
      });
    });
  }








  login() {
    if (this.estaCarregando()) return;

    this.mensagemErro.set(null);
    this.estaCarregando.set(true);

    this.authService.Login({
      email: this.email,
      password: this.senha
    }).subscribe({
      next: () => {
        this.estaCarregando.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (erro) => {
        this.estaCarregando.set(false);
        if (erro.status >= 400 && erro.status < 500) {
          this.mensagemErro.set('E-mail ou senha inválidos.');
        } else {
          this.mensagemErro.set('Não foi possível conectar ao servidor. Tente novamente mais tarde.');
        }
      }
    });
  }

  








}