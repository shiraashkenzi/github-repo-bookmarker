import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';

import { LoginResponse } from '../models';

const STORAGE_KEY = 'auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly session = signal<LoginResponse | null>(readStoredSession());

  readonly token = computed(() => this.session()?.token ?? null);
  readonly username = computed(() => this.session()?.username ?? null);
  readonly isLoggedIn = computed(() => this.session() !== null);

  async login(username: string, password: string): Promise<void> {
    const session = await firstValueFrom(
      this.http.post<LoginResponse>('/api/auth/login', { username, password }),
    );
    localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
    this.session.set(session);
  }

  async logout(): Promise<void> {
    // Ends the server-side session; local state is cleared even if that call fails.
    await firstValueFrom(this.http.post('/api/auth/logout', null)).catch(() => undefined);
    localStorage.removeItem(STORAGE_KEY);
    this.session.set(null);
    await this.router.navigateByUrl('/login');
  }
}

function readStoredSession(): LoginResponse | null {
  const stored = localStorage.getItem(STORAGE_KEY);
  if (!stored) return null;

  const session: LoginResponse = JSON.parse(stored);
  return new Date(session.expiresAt) > new Date() ? session : null;
}
