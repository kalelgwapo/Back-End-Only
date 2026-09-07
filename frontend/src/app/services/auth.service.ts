import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { User } from '../models/user.model';

interface LoginResponse {
  accessToken: string;
  expiresAtUtc: string;
  user: {
    id: number;
    username: string;
    fullName: string;
    email: string;
    role: string;
  };
}

interface StoredSession {
  user: User;
  accessToken: string;
  expiresAtUtc: string;
}

const SESSION_KEY = 'leave-app.session';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly currentUserSignal = signal<User | null>(this.readSession());
  private readonly apiUrl = 'http://localhost:5260/api/auth/login';

  readonly currentUser = this.currentUserSignal.asReadonly();
  readonly isLoggedIn = computed(() => this.currentUserSignal() !== null);

  async login(username: string, password: string): Promise<boolean> {
    try {
      const response = await firstValueFrom(
        this.http.post<LoginResponse>(this.apiUrl, {
          username: username.trim(),
          password
        })
      );
      const user: User = {
        username: response.user.username,
        displayName: response.user.fullName,
        id: response.user.id,
        email: response.user.email,
        role: response.user.role
      };
      const session: StoredSession = {
        user,
        accessToken: response.accessToken,
        expiresAtUtc: response.expiresAtUtc
      };

      sessionStorage.setItem(SESSION_KEY, JSON.stringify(session));
      this.currentUserSignal.set(user);
      return true;
    } catch {
      return false;
    }
  }

  getAccessToken(): string | null {
    const raw = sessionStorage.getItem(SESSION_KEY);
    if (!raw) {
      return null;
    }

    try {
      const session = JSON.parse(raw) as StoredSession;
      if (new Date(session.expiresAtUtc).getTime() <= Date.now()) {
        this.logout();
        return null;
      }
      return session.accessToken;
    } catch {
      this.logout();
      return null;
    }
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
      const session = JSON.parse(raw) as StoredSession;
      if (new Date(session.expiresAtUtc).getTime() <= Date.now()) {
        sessionStorage.removeItem(SESSION_KEY);
        return null;
      }
      return session.user;
    } catch {
      sessionStorage.removeItem(SESSION_KEY);
      return null;
    }
  }
}
