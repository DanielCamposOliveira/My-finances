import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MoneyCard } from './money-card';

describe('MoneyCard', () => {
  let component: MoneyCard;
  let fixture: ComponentFixture<MoneyCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MoneyCard],
    }).compileComponents();

    fixture = TestBed.createComponent(MoneyCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
