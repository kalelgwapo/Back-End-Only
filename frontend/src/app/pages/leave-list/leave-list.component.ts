import { DatePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LeaveType, LEAVE_TYPES } from '../../models/leave-application.model';
import { LeaveService } from '../../services/leave.service';

@Component({
  selector: 'app-leave-list',
  imports: [DatePipe, RouterLink],
  templateUrl: './leave-list.component.html',
  styleUrl: './leave-list.component.scss'
})
export class LeaveListComponent {
  private readonly leaveService = inject(LeaveService);

  readonly applications = this.leaveService.applications;

  leaveTypeName(value: LeaveType): string {
    const match = LEAVE_TYPES.find((option) => option.value === value);
    if (!match) {
      return '—';
    }

    return match.label.split(' (')[0];
  }
}
