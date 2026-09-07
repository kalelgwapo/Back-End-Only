import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, map, tap } from 'rxjs';
import {
  ApiLeaveApplication,
  LeaveApplication,
  LeaveApplicationRequest
} from '../models/leave-application.model';

@Injectable({ providedIn: 'root' })
export class LeaveService {
  private readonly http = inject(HttpClient);
  private readonly applicationsSignal = signal<LeaveApplication[]>([]);
  private readonly apiUrl = 'http://localhost:5260/api/leave-applications';

  readonly applications = this.applicationsSignal.asReadonly();

  constructor() {
    this.refresh();
  }

  refresh(): void {
    this.http.get<ApiLeaveApplication[]>(this.apiUrl).subscribe({
      next: (applications) => this.applicationsSignal.set(applications.map(this.toApplication)),
      error: () => this.applicationsSignal.set([])
    });
  }

  getById(id: number): LeaveApplication | undefined {
    return this.applicationsSignal().find((item) => item.id === id);
  }

  create(request: LeaveApplicationRequest): Observable<LeaveApplication> {
    return this.http.post<ApiLeaveApplication>(this.apiUrl, request).pipe(
      map(this.toApplication),
      tap((created) => this.applicationsSignal.update((items) => [...items, created]))
    );
  }

  update(id: number, payload: Omit<LeaveApplication, 'id'>): LeaveApplication | undefined {
    const existing = this.getById(id);
    if (!existing) {
      return undefined;
    }

    const updated: LeaveApplication = { ...payload, id };
    this.applicationsSignal.update((items) =>
      items.map((item) => (item.id === id ? updated : item))
    );
    return updated;
  }

  private readonly toApplication = (item: ApiLeaveApplication): LeaveApplication => ({
    id: item.id,
    applicant: item.applicant,
    manager: item.manager,
    leaveType: item.leaveType,
    startDate: item.startDate,
    endDate: item.endDate,
    returnDate: item.returnDate,
    numberOfDays: item.requestedDays,
    comments: item.generalComments ?? ''
  });
}
