import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CadastroPage } from './cadastro.page';

describe('CadastroPage', () => {
  let component: CadastroPage;
  let fixture: ComponentFixture<CadastroPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CadastroPage],
    }).compileComponents();

    fixture = TestBed.createComponent(CadastroPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
