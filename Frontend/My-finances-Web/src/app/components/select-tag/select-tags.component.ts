import { Component, Input, Output, EventEmitter, HostListener, ElementRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Tag } from '../../models/tag.model';

@Component({
  selector: 'app-select-tags',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './select-tags.component.html',
  styleUrl: './select-tags.component.scss',
})
export class SelectTagsComponent {
private elementRef = inject(ElementRef);

  @Input() tags: Tag[] = [];
  @Input() selectedTagIds: number[] = [];
  @Output() tagsChange = new EventEmitter<number[]>();

  isOpen = false;

  toggleDropdown(): void {
    this.isOpen = !this.isOpen;
  }

  isSelected(id: number): boolean {
    return this.selectedTagIds.includes(id);
  }

  toggleTag(id: number, event: Event): void {
    event.stopPropagation();
    if (this.isSelected(id)) {
      this.selectedTagIds = this.selectedTagIds.filter(tagId => tagId !== id);
    } else {
      this.selectedTagIds = [...this.selectedTagIds, id];
    }
    this.tagsChange.emit(this.selectedTagIds);
  }

  getFirstSelectedTagName(): string {
    if (!this.selectedTagIds.length) return '';
    const tag = this.tags.find(t => t.id === this.selectedTagIds[0]);
    return tag ? tag.nome : '';
  }

  // Fecha o menu ao clicar fora do componente
  @HostListener('document:click', ['$event'])
  onClickOutside(event: Event): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen = false;
    }
  }
}
