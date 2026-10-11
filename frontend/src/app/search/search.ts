import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { RepoService } from '../repo.service';
import { RepoCard } from '../repo-card/repo-card';

@Component({
  selector: 'app-search',
  imports: [FormsModule, RepoCard],
  templateUrl: './search.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Search implements OnInit {
  protected readonly repos = inject(RepoService);
  protected readonly query = signal('');

  ngOnInit(): void {
    // Needed so cards show which results are already bookmarked.
    void this.repos.loadBookmarks();
  }

  protected search(): void {
    const query = this.query().trim();
    if (query) void this.repos.search(query);
  }
}
