import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoriaInputComponent } from './categoria-input.component';

describe('CategoriaInputComponent', () => {
  let component: CategoriaInputComponent;
  let fixture: ComponentFixture<CategoriaInputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoriaInputComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(CategoriaInputComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
