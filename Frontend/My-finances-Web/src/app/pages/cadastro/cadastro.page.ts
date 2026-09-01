import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

// Angular Material Datepicker
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule, MAT_DATE_LOCALE } from '@angular/material/core';

import { SelectCategoriaComponent } from '../../components/select-categoria/select-categoria.component';
import { SelectTagsComponent } from '../../components/select-tag/select-tags.component';

import { LancamentosService } from '../../service/Lancamentos/lancamentos-service';
import { ContaFixaService } from '../../service/ContaFixa/conta-fixa-service';
import { CategoriaService } from '../../service/Categoria/categoria-service';
import { TagService } from '../../service/Tag/tag-service';

import { Categoria, CategoriaCadastro } from '../../models/categoria.model';
import { Tag, TagCadastro } from '../../models/tag.model';
import { LancamentoCadastro } from '../../models/lancamentos.model';
import { ContaFixaCadastro } from '../../models/canta-fixa';

import { MatDialog } from '@angular/material/dialog';
import { DialogDataCategoria, CategoriaInputComponent } from '../../components/Dialog/categoria-input.component/categoria-input.component';
import { DialogDataTag, TagInputComponent } from '../../components/Dialog/tag-input.component/tag-input.component';

import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackBarCustomComponent } from '../../components/SnackBar/snack-bar-info/snack-bar-info';

export type TipoSnack = 'sucesso' | 'erro' | 'info' | 'despesa' | 'receita';
export type TipoCadastro = 'lancamento' | 'contafixa';

@Component({
  selector: 'app-cadastro',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    SelectCategoriaComponent,
    SelectTagsComponent
  ],
  providers: [
    { provide: MAT_DATE_LOCALE, useValue: 'pt-BR' } // Deixa os meses e dias em Português
  ],
  templateUrl: './cadastro.page.html',
  styleUrl: './cadastro.page.scss'
})
export class CadastroPage implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);


  private lancamentosService = inject(LancamentosService);
  private contaFixaService = inject(ContaFixaService);
  private categoriaService = inject(CategoriaService);
  private tagService = inject(TagService);

  readonly dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);


  formCadastro!: FormGroup;
  categorias: Categoria[] = [];
  tags: Tag[] = [];

tipoCadastro: TipoCadastro = 'lancamento';
  
  ngOnInit(): void {
    this.obterTipoCadastro();
    this.initForm();
    this.carregarCategoria();
    this.CarregaTag();
  }

  private obterTipoCadastro(): void {
    const tipo = this.route.snapshot.data['tipo'] as TipoCadastro;
    if (tipo) {
      this.tipoCadastro = tipo;
    }
  }

private initForm(): void {
    this.formCadastro = this.fb.group({
      descricao: ['', [Validators.required]],
      valorTotal: [null, [Validators.required, Validators.min(0.01)]],
      qtdParcelas: [1, this.tipoCadastro === 'lancamento' ? [Validators.required, Validators.min(1)] : []],
      dataPrimeiroVencimento: ['', [Validators.required]],
      categoriaId: [null, [Validators.required]],
      tagIds: [[]]
    });
  }

  private carregarCategoria(): void {
    this.categoriaService.getCategoria().subscribe({
      next: (res) => (this.categorias = [...res]),
      error: (err) => console.error('Erro ao carregar categorias:', err)
    });
  }

  private CarregaTag(): void { 
        this.tagService.GetTag().subscribe({
      next: (res) => (this.tags = [...res]),
      error: (err) => console.error('Erro ao carregar tags:', err)
    });
  }

  onCategoriaSelecionada(id: number): void {
    this.formCadastro.patchValue({ categoriaId: id });
  }

  onTagsSelecionadas(ids: number[]): void {
    this.formCadastro.patchValue({ tagIds: ids });
  }

  salvar(): void {
    if (this.formCadastro.invalid) {
      this.formCadastro.markAllAsTouched();
      return;
    }

    const formValues = this.formCadastro.value;

    if (this.tipoCadastro === 'lancamento') {
     
      // Monta o pacote 
      const payload: LancamentoCadastro = {
        descricao: formValues.descricao,
        valorTotal: formValues.valorTotal,
        qtdParcelas: formValues.qtdParcelas,
        dataPrimeiroVencimento: formValues.dataPrimeiroVencimento,
        categoriaId: formValues.categoriaId,
        tagIds: formValues.tagIds
      }
      
      this.lancamentosService.Lancamento(payload).subscribe({
        next: () => {
          this.abrirSnackBar('Lançamento cadastrado com sucesso!', 3, 'fa-solid fa-circle-check', 'sucesso');
          this.voltar();
        },
        error: (err) => {
          console.error('Erro ao registrar lançamento:', err);
          this.abrirSnackBar('Erro ao registrar lançamento', 3, 'fa-solid fa-ban', 'erro');
        }
      });

    }
    else {

      // Extrai apenas o número do dia (ex: 10)
      const diaVencimento = this.extrairDia(formValues.dataPrimeiroVencimento);
      
      // Monta o pacote 
      const payload: ContaFixaCadastro = {
        descricao: formValues.descricao,
        categoriaId: formValues.categoriaId,
        diaVencimento: diaVencimento,
        valorBase: formValues.valorTotal,
        tagIds:formValues.tagIds
      };
    
      this.contaFixaService.ContaFixa(payload).subscribe({
        next: () => {
          this.abrirSnackBar('Conta Fixa cadastrada com sucesso!', 3, 'fa-solid fa-circle-check', 'sucesso');
          this.voltar();
        },
        error: (err) => {
          console.error('Erro ao registrar conta fixa:', err);
          this.abrirSnackBar('Erro ao registrar conta fixa', 3, 'fa-solid fa-ban', 'erro');
        }
      });
    }    
  }


// Função auxiliar para extrair o dia com segurança
private extrairDia(data: Date | string): number {
  if (data instanceof Date) {
    return data.getDate();
  }
  // Caso venha como string no formato ISO (ex: "2026-08-10T03:00:00.000Z")
  const dataString = String(data);
  if (dataString.includes('-')) {
    const parteDia = dataString.split('T')[0].split('-')[2];
    return Number(parteDia);
  }
  return new Date(data).getDate();
}


  voltar(): void {
    this.router.navigate(['/parcela']);
  }


  //Abre a modal para cadastro de uma nova tag e processa o retorno
  abrirModalTag(): void {
    const configData: DialogDataTag = {
      titulo: 'Nova Tag',
      label: 'Nome da Tag',
      placeholder: 'Ex: Pix, Alimentação...',
      textoConfirmar: 'Salvar',
      textoCancelar: 'Cancelar'
    };
    
    // Instancia e abre a modal de criação de tag e passa as configurações e textos
    const dialogRef = this.dialog.open(TagInputComponent, {
      width: '400px',
      panelClass: 'custom-dialog-container',
      data: configData
    });

    // Escuta o fechamento do diálogo recebendo o payload tipado
    dialogRef.afterClosed().subscribe((novaTag: TagCadastro | undefined) => {
      // Valida se o usuário confirmou e se o nome não está vazio/apenas espaços
      if (novaTag && novaTag.nome.trim()) {
        this.criarTag(novaTag);
      }
    });
  }

   //Abre a modal para cadastro de uma nova tag e processa o retorno
  abrirModalCategoria(): void {
    const configData: DialogDataTag = {
      titulo: 'Nova Categoria',
      label: 'Nome da Categoria',
      placeholder: 'Ex: Pix, Alimentação...',
      textoConfirmar: 'Salvar',
      textoCancelar: 'Cancelar'
    };
    
    // Instancia e abre a modal de criação de tag e passa as configurações e textos
    const dialogRef = this.dialog.open(CategoriaInputComponent, {
      width: '400px',
      panelClass: 'custom-dialog-container',
      data: configData
    });

    // Escuta o fechamento do diálogo recebendo o payload tipado
    dialogRef.afterClosed().subscribe((novaCategoria: CategoriaCadastro | undefined) => {
      // Valida se o usuário confirmou e se o nome não está vazio/apenas espaços
      if (novaCategoria && novaCategoria.nome.trim()) {
       this.criaCategoria(novaCategoria);     
      }
    });

  }


  criarTag(novaTag: TagCadastro): void {
    const payload: TagCadastro = {
      nome: novaTag.nome
    };

    this.tagService.PostTag(payload).subscribe({
      next: () => {

        // recarrega as tag no 
        this.CarregaTag();
        // mostra uma msg do tipo SnackBar
        this.abrirSnackBar(`Tag criada`, 3, 'fa-solid fa-circle-check', "info");
      },
      error: (erro) => {
        this.abrirSnackBar(`Tag erro`, 3, 'fa-solid fa-ban', 'info')
        console.log("Ocorreu um erro ao tentar criar uma TAG", erro) 
       }
            
    });
  }

  criaCategoria(novoCategoria: CategoriaCadastro): void{
    const payload: CategoriaCadastro = {
      atribuicao: novoCategoria.atribuicao,
      nome: novoCategoria.nome
    }

    this.categoriaService.PostCategoria(payload).subscribe({
      next: () => {
        this.carregarCategoria();
        this.abrirSnackBar(`Categoria criada`, 3, 'fa-solid fa-circle-check', "info");
      },
      error: (erro) => {
        this.abrirSnackBar(`Categoria erro`, 3, 'fa-solid fa-ban', 'info')
      }
    });
  }

  // Chama o componente SnackBar
  abrirSnackBar(
    mensagem: string,
    duracaoSegundos: number = 3,
    icone: string = 'fa-solid fa-circle-check',
    tipo: TipoSnack = 'info'
  ): void {
    this.snackBar.openFromComponent(SnackBarCustomComponent, {
      duration: duracaoSegundos * 1000,
      horizontalPosition: 'center',
      verticalPosition: 'bottom',
      // Aplica a base + a variação de cor escolhida
      panelClass: ['snack-app-theme', `snack-${tipo}`],
      data: {
        mensagem,
        icone
      }
    });
  }



}