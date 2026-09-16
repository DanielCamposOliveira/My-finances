import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContafixaComponent } from './contafixa.component';

describe('ContafixaComponent', () => {
  let component: ContafixaComponent;
  let fixture: ComponentFixture<ContafixaComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContafixaComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ContafixaComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
