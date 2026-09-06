import { DestroyRef, Component, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  EMPLOYEES,
  LEAVE_TYPES,
  LeaveType,
  MANAGERS,
  leaveTypeMaxDays
} from '../../models/leave-application.model';
import { LeaveService } from '../../services/leave.service';

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
  private readonly destroyRef = inject(DestroyRef);

  readonly employees = EMPLOYEES;
  readonly managers = MANAGERS;
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

    const payload = {
      applicant: value.applicant,
      manager: value.manager,
      leaveType: value.leaveType as LeaveType,
      startDate: value.startDate,
      endDate: value.endDate,
      returnDate: value.returnDate,
      numberOfDays,
      comments: value.comments.trim()
    };

    if (this.editingId) {
      this.leaveService.update(this.editingId, payload);
    } else {
      this.leaveService.create(payload);
    }

    void this.router.navigate(['/leaves']);
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

  private toInputDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
