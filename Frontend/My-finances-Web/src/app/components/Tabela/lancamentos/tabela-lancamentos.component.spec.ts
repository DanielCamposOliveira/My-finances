import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TabelaLancamentosComponent } from './tabela-lancamentos.component';

describe('TabelaLancamentosComponent', () => {
  let component: TabelaLancamentosComponent;
  let fixture: ComponentFixture<TabelaLancamentosComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TabelaLancamentosComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TabelaLancamentosComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
