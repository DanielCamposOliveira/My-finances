import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MoneyTable } from './money-table';

describe('MoneyTable', () => {
  let component: MoneyTable;
  let fixture: ComponentFixture<MoneyTable>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MoneyTable],
    }).compileComponents();

    fixture = TestBed.createComponent(MoneyTable);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
