import { Component } from '@angular/core';
import { TableContasFixa } from "../../components/Tabela/contas/TableContasFixa";


@Component({
  selector: 'app-cadastro',
  imports: [TableContasFixa],
  templateUrl: './cadastro.html',
  styleUrl: './cadastro.scss',
})
export class Cadastro {}
