import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { Repo } from './models';

@Injectable({ providedIn: 'root' })
export class RepoService {
  private readonly http = inject(HttpClient);

  private readonly _results = signal<Repo[]>([]);
  private readonly _bookmarks = signal<Repo[]>([]);
  private readonly _loading = signal(false);
  private readonly _error = signal<string | null>(null);

  readonly results = this._results.asReadonly();
  readonly bookmarks = this._bookmarks.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly bookmarkedIds = computed(() => new Set(this._bookmarks().map((repo) => repo.id)));

  async search(query: string): Promise<void> {
    this._loading.set(true);
    this._error.set(null);
    try {
      const repos = await firstValueFrom(
        this.http.get<Repo[]>('/api/repos/search', { params: { q: query } }),
      );
      this._results.set(repos);
    } catch {
      this._error.set('Search failed. Please try again.');
    } finally {
      this._loading.set(false);
    }
  }

  async loadBookmarks(): Promise<void> {
    this._error.set(null);
    try {
      this._bookmarks.set(await firstValueFrom(this.http.get<Repo[]>('/api/bookmarks')));
    } catch {
      this._error.set('Could not load bookmarks.');
    }
  }

  async toggleBookmark(repo: Repo): Promise<void> {
    this._error.set(null);
    try {
      if (this.bookmarkedIds().has(repo.id)) {
        await firstValueFrom(this.http.delete(`/api/bookmarks/${repo.id}`));
        this._bookmarks.update((list) => list.filter((r) => r.id !== repo.id));
      } else {
        this._bookmarks.set(await firstValueFrom(this.http.post<Repo[]>('/api/bookmarks', repo)));
      }
    } catch {
      this._error.set('Could not update bookmark.');
    }
  }

  clear(): void {
    this._results.set([]);
    this._bookmarks.set([]);
    this._error.set(null);
  }
}
