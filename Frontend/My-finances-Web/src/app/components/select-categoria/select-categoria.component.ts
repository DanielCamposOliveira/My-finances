import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Categoria } from '../../models/categoria.model';

@Component({
  selector: 'app-select-categoria',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './select-categoria.component.html',
  styleUrl: './select-categoria.component.scss',
})
export class SelectCategoriaComponent {
  @Input() categorias: Categoria[] = [];
  @Input() selectedValue: number | null = null;
  @Output() categoriaChange = new EventEmitter<number>();

onChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    const value = target.value ? Number(target.value) : null;
    if (value !== null) {
      this.categoriaChange.emit(value);
    }
  }
}
