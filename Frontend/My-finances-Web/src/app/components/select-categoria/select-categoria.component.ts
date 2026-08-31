import { Component, Input, Output, EventEmitter, ElementRef, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Categoria } from '../../models/categoria.model';

@Component({
  selector: 'app-select-categoria',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './select-categoria.component.html',
  styleUrl: './select-categoria.component.scss',
  
  // Vincula dinamicamente a classe CSS 'opened' à tag <app-select-categoria> sempre que isOpen for true,
  // permitindo elevar o z-index do componente no SCSS (:host.opened) e evitar que o dropdown seja cortado.
  host: {
    '[class.opened]': 'isOpen'
  }
})
export class SelectCategoriaComponent {
  @Input() categorias: Categoria[] = [];
  @Input() selectedValue: number | null = null;
  @Output() categoriaChange = new EventEmitter<number>();

  isOpen = false;

  constructor(private elementRef: ElementRef) {}

  // Pega o nome da categoria selecionada para exibir no campo
  get selectedNome(): string | undefined {
    return this.categorias?.find(c => c.id === this.selectedValue)?.nome;
  }

  toggleDropdown(): void {
    this.isOpen = !this.isOpen;
  }

  selectOption(cat: Categoria): void {
    this.selectedValue = cat.id;
    this.categoriaChange.emit(cat.id);
    this.isOpen = false;
  }

  // Fecha o dropdown automaticamente se o usuário clicar fora do componente
  @HostListener('document:click', ['$event'])
  onClickOutside(event: MouseEvent): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen = false;
    }
  }
}