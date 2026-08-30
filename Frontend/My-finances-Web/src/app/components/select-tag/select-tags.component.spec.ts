import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectTagsComponent } from './select-tags.component';

describe('SelectTagsComponent', () => {
  let component: SelectTagsComponent;
  let fixture: ComponentFixture<SelectTagsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SelectTagsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SelectTagsComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
