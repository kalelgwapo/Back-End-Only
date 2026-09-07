import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  errorMessage = '';

  readonly form = this.fb.nonNullable.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });

  async submit(): Promise<void> {
    this.errorMessage = '';
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      this.errorMessage = 'Please enter a username and password.';
      return;
    }

    const { username, password } = this.form.getRawValue();
    const success = await this.auth.login(username, password);

    if (!success) {
      this.errorMessage = 'Invalid username or password.';
      return;
    }

    void this.router.navigate(['/leaves']);
  }
}
