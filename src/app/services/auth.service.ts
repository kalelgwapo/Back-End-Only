import { Injectable, computed, signal } from '@angular/core';
import { User } from '../models/user.model';

interface StoredAccount {
  username: string;
  password: string;
  displayName: string;
}

const SESSION_KEY = 'leave-app.session';

const ACCOUNTS: StoredAccount[] = [
  { username: 'admin', password: 'admin123', displayName: 'Administrator' },
  { username: 'jdoe', password: 'password', displayName: 'Jane Doe' }
];

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly currentUserSignal = signal<User | null>(this.readSession());

  readonly currentUser = this.currentUserSignal.asReadonly();
  readonly isLoggedIn = computed(() => this.currentUserSignal() !== null);

  login(username: string, password: string): boolean {
    const match = ACCOUNTS.find(
      (account) =>
        account.username.toLowerCase() === username.trim().toLowerCase() &&
        account.password === password
    );

    if (!match) {
      return false;
    }

    const user: User = {
      username: match.username,
      displayName: match.displayName
    };

    sessionStorage.setItem(SESSION_KEY, JSON.stringify(user));
    this.currentUserSignal.set(user);
    return true;
  }

  logout(): void {
    sessionStorage.removeItem(SESSION_KEY);
    this.currentUserSignal.set(null);
  }

  private readSession(): User | null {
    const raw = sessionStorage.getItem(SESSION_KEY);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as User;
    } catch {
      sessionStorage.removeItem(SESSION_KEY);
      return null;
    }
  }
}
