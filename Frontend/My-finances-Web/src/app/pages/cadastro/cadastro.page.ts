import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { SelectCategoriaComponent } from '../../components/select-categoria/select-categoria.component';
import { SelectTagsComponent } from '../../components/select-tag/select-tags.component';

import { LancamentosService } from '../../service/Lancamentos/lancamentos-service';
import { CategoriaService } from '../../service/Categoria/categoria-service';
import { TagService } from '../../service/Tag/tag-service';

import { Categoria } from '../../models/categoria.model';
import { Tag } from '../../models/tag.model';
import { LancamentoCadastro } from '../../models/lancamentos.model';

@Component({
  selector: 'app-cadastro',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SelectCategoriaComponent,
    SelectTagsComponent
  ],
  templateUrl: './cadastro.page.html',
  styleUrl: './cadastro.page.scss'
})
export class CadastroPage implements OnInit {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private lancamentosService = inject(LancamentosService);
  private categoriaService = inject(CategoriaService);
  private tagService = inject(TagService);

  formCadastro!: FormGroup;
  categorias: Categoria[] = [];
  tags: Tag[] = [];

  ngOnInit(): void {
    this.initForm();
    this.carregarDados();
  }

  private initForm(): void {
    this.formCadastro = this.fb.group({
      descricao: ['', [Validators.required]],
      valorTotal: [null, [Validators.required, Validators.min(0.01)]],
      qtdParcelas: [1, [Validators.required, Validators.min(1)]],
      dataPrimeiroVencimento: ['', [Validators.required]],
      categoriaId: [null, [Validators.required]],
      tagIds: [[]]
    });
  }

  private carregarDados(): void {
    this.categoriaService.getCategoria().subscribe({
      next: (res) => (this.categorias = [...res]),
      error: (err) => console.error('Erro ao carregar categorias:', err)
    });

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

    const payload: LancamentoCadastro = this.formCadastro.value;

    this.lancamentosService.Lancamento(payload).subscribe({
      next: () => {
        this.voltar();
      },
      error: (err) => console.error('Erro ao registrar lançamento:', err)
    });
  }

  voltar(): void {
    this.router.navigate(['/parcela']);
  }
}