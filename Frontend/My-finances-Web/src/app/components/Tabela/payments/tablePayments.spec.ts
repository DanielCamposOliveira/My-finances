import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TableParcela } from './tablePayments';

describe('TableParcela', () => {
  let component: TableParcela;
  let fixture: ComponentFixture<TableParcela>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TableParcela],
    }).compileComponents();

    fixture = TestBed.createComponent(TableParcela);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
