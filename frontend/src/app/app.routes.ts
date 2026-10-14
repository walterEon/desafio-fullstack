import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login').then((m) => m.Login),
  },
  {
    path: 'products',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/product-list/product-list').then((m) => m.ProductList),
  },
  {
    path: 'products/:id/edit',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/product-edit/product-edit').then((m) => m.ProductEdit),
  },
  { path: '', pathMatch: 'full', redirectTo: 'products' },
  { path: '**', redirectTo: 'products' },
];
