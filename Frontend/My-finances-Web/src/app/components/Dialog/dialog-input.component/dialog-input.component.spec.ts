import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogInputComponent } from './dialog-input.component';

describe('DialogInputComponent', () => {
  let component: DialogInputComponent;
  let fixture: ComponentFixture<DialogInputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DialogInputComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DialogInputComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
