import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';

import { RepoService } from '../repo.service';
import { RepoCard } from '../repo-card/repo-card';

@Component({
  selector: 'app-bookmarks',
  imports: [RepoCard],
  templateUrl: './bookmarks.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Bookmarks implements OnInit {
  protected readonly repos = inject(RepoService);

  ngOnInit(): void {
    void this.repos.loadBookmarks();
  }
}
