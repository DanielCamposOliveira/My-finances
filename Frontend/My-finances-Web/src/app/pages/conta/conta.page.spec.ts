import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContaPage } from './conta.page';

describe('ContaPage', () => {
  let component: ContaPage;
  let fixture: ComponentFixture<ContaPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContaPage],
    }).compileComponents();

    fixture = TestBed.createComponent(ContaPage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
