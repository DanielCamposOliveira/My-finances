import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContaFixa } from './conta-fixa';

describe('ContaFixa', () => {
  let component: ContaFixa;
  let fixture: ComponentFixture<ContaFixa>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContaFixa],
    }).compileComponents();

    fixture = TestBed.createComponent(ContaFixa);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
