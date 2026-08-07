import { TestBed } from '@angular/core/testing';

import { ContaFixaService } from './conta-fixa-service';

describe('ContaFixaService', () => {
  let service: ContaFixaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ContaFixaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
