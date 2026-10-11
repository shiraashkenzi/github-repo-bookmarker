import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

import { Repo } from '../models';

@Component({
  selector: 'app-repo-card',
  templateUrl: './repo-card.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RepoCard {
  readonly repo = input.required<Repo>();
  readonly bookmarked = input(false);
  readonly toggle = output<Repo>();
}
