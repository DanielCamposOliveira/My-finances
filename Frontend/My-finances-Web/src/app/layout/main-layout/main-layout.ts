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
  isDarkMode: boolean = false;

  ngOnInit(): void {
    // Esta requisição agora SÓ roda quando o usuário entra no Layout (área logada)
    this.ObterInformacoesUsuario();
  }

  ObterInformacoesUsuario(): void {

  }

  onThemeChange(value: boolean): void {

  }

  deslogar(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}