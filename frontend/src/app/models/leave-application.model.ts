export type LeaveType = 'annual' | 'sick' | 'emergency';

export interface LeaveTypeOption {
  id: number;
  value: LeaveType;
  label: string;
  maxDaysPerYear: number;
}

export interface LeaveApplication {
  id: number;
  applicant: string;
  manager: string;
  leaveType: LeaveType;
  startDate: string;
  endDate: string;
  returnDate: string;
  numberOfDays: number;
  comments: string;
}

export interface LeaveApplicationRequest {
  applicantId: number;
  managerId: number;
  leaveTypeId: number;
  startDate: string;
  endDate: string;
  returnDate: string;
  requestedDays: number;
  generalComments: string;
}

export interface ApiLeaveApplication {
  id: number;
  applicantId: number;
  applicant: string;
  managerId: number;
  manager: string;
  leaveTypeId: number;
  leaveType: LeaveType;
  startDate: string;
  endDate: string;
  returnDate: string;
  requestedDays: number;
  generalComments: string | null;
  status: string;
  createdAt: string;
}

export const LEAVE_TYPES: LeaveTypeOption[] = [
  {
    id: 1,
    value: 'annual',
    label: 'Annual Leave (Max allowed duration: 20 days/year)',
    maxDaysPerYear: 20
  },
  {
    id: 2,
    value: 'sick',
    label: 'Sick Leave (Max allowed duration: 7 days/year)',
    maxDaysPerYear: 7
  },
  {
    id: 3,
    value: 'emergency',
    label: 'Emergency Leave (Max allowed duration: 3 days/year)',
    maxDaysPerYear: 3
  }
];

export const EMPLOYEES = [
  'Jane Doe',
  'John Smith',
  'Maria Santos',
  'Alex Chen',
  'Riley Patel'
] as const;

export const MANAGERS = [
  'Robert Hale',
  'Priya Nair',
  'David Kim'
] as const;

export function leaveTypeLabel(value: LeaveType | string | undefined): string {
  return LEAVE_TYPES.find((option) => option.value === value)?.label ?? '—';
}

export function leaveTypeMaxDays(value: LeaveType | string | undefined): number | null {
  return LEAVE_TYPES.find((option) => option.value === value)?.maxDaysPerYear ?? null;
}
