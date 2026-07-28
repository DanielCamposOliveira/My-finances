import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Dashboard } from "./pages/dashboard/dashboard";


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,  Dashboard],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {

  iconClass : string ="ph ph-wallet";
  title: string ="Saldo acumulado";

  // Coloque o valor sem ponto de milhar (1180 em vez de 1.180)
  rawBalance: number = 1180;

  // Formata o valor direto via JavaScript/TS
  get balanceFormatted(): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(this.rawBalance); // Resultado: "R$ 1.180,00"
  }


}
