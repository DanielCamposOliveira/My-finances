import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../../components/header/header';
import { AuthService } from '../../service/Authentication/auth.service';
import{LocalstorageService} from '../../service/localstorage/localstorage-service'

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
  private localStorage = inject(LocalstorageService);

  Name: string = '';
  IsActive: boolean = false;
  IsAdmin: boolean = false;
  isDarkMode: boolean = true;

  ngOnInit(): void {
    // Esta requisição agora SÓ roda quando o usuário entra no Layout (área logada)
    this.ObterInformacoesUsuario();

    // verifica qual o status do DarkMode
    this.isDarkMode = this.localStorage.isDarkMode();
  }

  ObterInformacoesUsuario(): void {

  }

  onThemeChange(value: boolean): void {
    this.onDarkMode();
  }


  onDarkMode(): void
  {
    // inverte o valor do status
    const novoValor = !this.localStorage.isDarkMode();
    // grava o novo valor
    this.localStorage.setDarkMode(novoValor);
    // salva o novo valor na variavel
    this.isDarkMode = this.localStorage.isDarkMode();   
  }

  deslogar(): void {    
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}