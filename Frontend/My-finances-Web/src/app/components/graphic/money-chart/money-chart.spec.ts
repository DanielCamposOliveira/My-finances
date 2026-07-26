import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MoneyChart } from './money-chart';

describe('MoneyChart', () => {
  let component: MoneyChart;
  let fixture: ComponentFixture<MoneyChart>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MoneyChart],
    }).compileComponents();

    fixture = TestBed.createComponent(MoneyChart);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
