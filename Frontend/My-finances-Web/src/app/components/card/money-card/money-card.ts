import { Component ,Input} from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-money-card',
 
  imports: [CommonModule],
  templateUrl: './money-card.html',
  styleUrl: './money-card.scss',
})
export class MoneyCard {

  // Mapeia o atributo 'iconclass' do HTML para a variável interna 'iconClass'
  @Input() iconClass: string = '';
  @Input() TypeClass: string = '';
  @Input() title: string = ''; 
  @Input() balance: number | string = 0; 




}
