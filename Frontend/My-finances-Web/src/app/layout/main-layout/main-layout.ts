import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../../components/header/header';
import { AuthService } from '../../service/Authentication/auth.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss'
})
export class MainLayoutComponent implements OnInit {

  private router = inject(Router);
  private authService = inject(AuthService);


  Name: string = '';
  IsActive: boolean = false;
  IsAdmin: boolean = false;
  isDarkMode: boolean = true;

  ngOnInit(): void {
    // Esta requisição agora SÓ roda quando o usuário entra no Layout (área logada)
    this.ObterInformacoesUsuario();

    // verifica qual o status do DarkMode
    this.isDarkMode = this.authService.isDarkMode();
  }

  ObterInformacoesUsuario(): void {

  }

  onThemeChange(value: boolean): void {
    this.onDarkMode();
  }


  onDarkMode(): void
  {
    // inverte o valor do status
    const novoValor = !this.authService.isDarkMode();
    // grava o novo valor
    this.authService.setDarkMode(novoValor);
    // salva o novo valor na variavel
    this.isDarkMode = this.authService.isDarkMode();   
  }

  deslogar(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}