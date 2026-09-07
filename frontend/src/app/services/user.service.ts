import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiUser } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly usersUrl = 'http://localhost:5260/api/Lookup/Users';

  getUsers(): Observable<ApiUser[]> {
    return this.http.get<ApiUser[]>(this.usersUrl);
  }
}