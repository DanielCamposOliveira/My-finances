import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SnackBarInfo } from './snack-bar-info';

describe('SnackBarInfo', () => {
  let component: SnackBarInfo;
  let fixture: ComponentFixture<SnackBarInfo>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SnackBarInfo],
    }).compileComponents();

    fixture = TestBed.createComponent(SnackBarInfo);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
