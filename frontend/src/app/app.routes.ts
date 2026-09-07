import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './guards/auth.guard';
import { LeaveFormComponent } from './pages/leave-form/leave-form.component';
import { LeaveListComponent } from './pages/leave-list/leave-list.component';
import { LoginComponent } from './pages/login/login.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'leaves' },
  { path: 'login', component: LoginComponent, canActivate: [guestGuard] },
  { path: 'leaves', component: LeaveListComponent, canActivate: [authGuard] },
  { path: 'leaves/new', component: LeaveFormComponent, canActivate: [authGuard] },
  { path: 'leaves/:id', component: LeaveFormComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: 'leaves' }
];
