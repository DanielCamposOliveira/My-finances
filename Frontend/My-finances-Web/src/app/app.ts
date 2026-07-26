import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MoneyChart } from "./components/graphic/money-chart/money-chart";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MoneyChart],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('My-finances-Web');
}
