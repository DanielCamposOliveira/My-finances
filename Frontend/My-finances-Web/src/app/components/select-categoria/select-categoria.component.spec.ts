import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectCategoriaComponent } from './select-categoria.component/select-categoria.component';

describe('SelectCategoriaComponent', () => {
  let component: SelectCategoriaComponent;
  let fixture: ComponentFixture<SelectCategoriaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SelectCategoriaComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SelectCategoriaComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
