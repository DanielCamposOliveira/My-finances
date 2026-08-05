import { TestBed } from '@angular/core/testing';

import { DashboardServe } from './dashboard-serve';

describe('DashboardServe', () => {
  let service: DashboardServe;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DashboardServe);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
