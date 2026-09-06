import { Injectable, signal } from '@angular/core';
import { LeaveApplication, LeaveType } from '../models/leave-application.model';

const STORAGE_KEY = 'leave-app.applications.v2';

const SEED_DATA: LeaveApplication[] = [
  {
    id: 1001,
    applicant: 'Jane Doe',
    manager: 'Robert Hale',
    leaveType: 'annual',
    startDate: '2026-09-15',
    endDate: '2026-09-19',
    returnDate: '2026-09-22',
    numberOfDays: 5,
    comments: 'Family vacation'
  },
  {
    id: 1002,
    applicant: 'John Smith',
    manager: 'Priya Nair',
    leaveType: 'sick',
    startDate: '2026-10-01',
    endDate: '2026-10-02',
    returnDate: '2026-10-03',
    numberOfDays: 2,
    comments: 'Medical appointment'
  },
  {
    id: 1003,
    applicant: 'Maria Santos',
    manager: 'David Kim',
    leaveType: 'annual',
    startDate: '2026-11-10',
    endDate: '2026-11-14',
    returnDate: '2026-11-17',
    numberOfDays: 5,
    comments: 'Annual leave'
  }
];

@Injectable({ providedIn: 'root' })
export class LeaveService {
  private readonly applicationsSignal = signal<LeaveApplication[]>(this.load());

  readonly applications = this.applicationsSignal.asReadonly();

  getById(id: number): LeaveApplication | undefined {
    return this.applicationsSignal().find((item) => item.id === id);
  }

  create(payload: Omit<LeaveApplication, 'id'>): LeaveApplication {
    const created: LeaveApplication = {
      ...payload,
      id: this.nextId()
    };

    this.applicationsSignal.update((items) => [...items, created]);
    this.persist();
    return created;
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
    this.persist();
    return updated;
  }

  private nextId(): number {
    const ids = this.applicationsSignal().map((item) => item.id);
    return ids.length ? Math.max(...ids) + 1 : 1001;
  }

  private load(): LeaveApplication[] {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(SEED_DATA));
      return SEED_DATA;
    }

    try {
      const parsed = JSON.parse(raw) as Array<Partial<LeaveApplication>>;
      if (!Array.isArray(parsed)) {
        return SEED_DATA;
      }

      return parsed.map((item, index) => this.normalize(item, index));
    } catch {
      return SEED_DATA;
    }
  }

  private normalize(item: Partial<LeaveApplication>, index: number): LeaveApplication {
    const leaveType = this.isLeaveType(item.leaveType) ? item.leaveType : 'annual';

    return {
      id: item.id ?? 1001 + index,
      applicant: item.applicant ?? '',
      manager: item.manager ?? '',
      leaveType,
      startDate: item.startDate ?? '',
      endDate: item.endDate ?? '',
      returnDate: item.returnDate ?? '',
      numberOfDays: item.numberOfDays ?? 1,
      comments: item.comments ?? ''
    };
  }

  private isLeaveType(value: unknown): value is LeaveType {
    return value === 'annual' || value === 'sick' || value === 'emergency';
  }

  private persist(): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(this.applicationsSignal()));
  }
}
