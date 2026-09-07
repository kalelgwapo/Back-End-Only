import { DestroyRef, Component, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  LEAVE_TYPES,
  LeaveType,
  leaveTypeMaxDays
} from '../../models/leave-application.model';
import { ApiUser } from '../../models/user.model';
import { LeaveService } from '../../services/leave.service';
import { UserService } from '../../services/user.service';
import { LeaveApplicationRequest } from '../../models/leave-application.model';

@Component({
  selector: 'app-leave-form',
  imports: [ReactiveFormsModule],
  templateUrl: './leave-form.component.html',
  styleUrl: './leave-form.component.scss'
})
export class LeaveFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly leaveService = inject(LeaveService);
  private readonly userService = inject(UserService);
  private readonly destroyRef = inject(DestroyRef);

  employees: string[] = [];
  managers: string[] = [];
  private users: ApiUser[] = [];
  readonly leaveTypes = LEAVE_TYPES;

  editingId: number | null = null;
  notFound = false;
  submitted = false;
  leaveTypeLimitError = '';

  readonly form = this.fb.nonNullable.group({
    applicant: ['', Validators.required],
    manager: ['', Validators.required],
    leaveType: ['' as LeaveType | '', Validators.required],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    returnDate: ['', Validators.required],
    numberOfDays: [null as number | null, [Validators.required, Validators.min(1)]],
    comments: ['']
  });

  get isEditMode(): boolean {
    return this.editingId !== null;
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');

    if (idParam) {
      const id = Number(idParam);
      const existing = this.leaveService.getById(id);

      if (!existing) {
        this.notFound = true;
        return;
      }

      this.editingId = id;
      this.form.patchValue(existing);
    }

    this.userService
      .getUsers()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (users) => this.setUsers(users),
        error: () => {
          this.employees = [];
          this.managers = [];
        }
      });

    this.form.controls.startDate.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.syncCalculatedFields());
    this.form.controls.endDate.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.syncCalculatedFields());
    this.form.controls.leaveType.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.leaveTypeLimitError = '';
      });
  }

  submit(): void {
    this.submitted = true;
    this.leaveTypeLimitError = '';
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      return;
    }

    const value = this.form.getRawValue();
    const numberOfDays = Number(value.numberOfDays);
    const maxDays = leaveTypeMaxDays(value.leaveType);

    if (maxDays !== null && numberOfDays > maxDays) {
      this.leaveTypeLimitError = `This leave type allows a maximum of ${maxDays} days per year.`;
      return;
    }

    const applicant = this.users.find((user) => user.fullName === value.applicant);
    const manager = this.users.find((user) => user.fullName === value.manager);
    const leaveType = this.leaveTypes.find((option) => option.value === value.leaveType);
    if (!applicant || !manager || !leaveType) {
      this.leaveTypeLimitError = 'Please select a valid applicant, manager, and leave type.';
      return;
    }

    const payload: LeaveApplicationRequest = {
      applicantId: applicant.id,
      managerId: manager.id,
      leaveTypeId: leaveType.id,
      startDate: value.startDate,
      endDate: value.endDate,
      returnDate: value.returnDate,
      requestedDays: numberOfDays,
      generalComments: value.comments.trim()
    };

    if (this.editingId) {
      this.leaveService.update(this.editingId, {
        applicant: value.applicant,
        manager: value.manager,
        leaveType: value.leaveType as LeaveType,
        startDate: value.startDate,
        endDate: value.endDate,
        returnDate: value.returnDate,
        numberOfDays,
        comments: value.comments.trim()
      });
      void this.router.navigate(['/leaves']);
    } else {
      this.leaveService.create(payload).subscribe({
        next: () => void this.router.navigate(['/leaves']),
        error: () => {
          this.leaveTypeLimitError = 'The leave request could not be submitted.';
        }
      });
    }
  }

  cancel(): void {
    void this.router.navigate(['/leaves']);
  }

  private syncCalculatedFields(): void {
    const start = this.form.controls.startDate.value;
    const end = this.form.controls.endDate.value;

    if (!start || !end) {
      return;
    }

    const startDate = new Date(start);
    const endDate = new Date(end);

    if (endDate < startDate) {
      return;
    }

    const milliseconds = endDate.getTime() - startDate.getTime();
    const days = Math.round(milliseconds / (1000 * 60 * 60 * 24)) + 1;
    this.form.controls.numberOfDays.setValue(days, { emitEvent: false });

    if (!this.form.controls.returnDate.dirty) {
      const nextDay = new Date(endDate);
      nextDay.setDate(nextDay.getDate() + 1);
      this.form.controls.returnDate.setValue(this.toInputDate(nextDay), { emitEvent: false });
    }
  }

  private setUsers(users: ApiUser[]): void {
    this.users = users;
    this.employees = users
      .filter((user) => user.role.toLowerCase() === 'employee')
      .map((user) => user.fullName);
    this.managers = users
      .filter((user) => user.role.toLowerCase() === 'manager')
      .map((user) => user.fullName);
  }

  private toInputDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
