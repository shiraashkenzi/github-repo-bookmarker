import { Routes } from '@angular/router';

import { authGuard } from './auth/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./login/login').then((m) => m.Login) },
  {
    path: 'search',
    canActivate: [authGuard],
    loadComponent: () => import('./search/search').then((m) => m.Search),
  },
  {
    path: 'bookmarks',
    canActivate: [authGuard],
    loadComponent: () => import('./bookmarks/bookmarks').then((m) => m.Bookmarks),
  },
  { path: '**', redirectTo: 'search' },
];
