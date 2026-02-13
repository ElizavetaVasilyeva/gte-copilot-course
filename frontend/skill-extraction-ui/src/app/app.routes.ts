import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/signin',
    pathMatch: 'full'
  },
  {
    path: 'signup',
    loadComponent: () => import('./features/auth/signup/signup.component').then(m => m.SignupComponent)
  },
  {
    path: 'signin',
    loadComponent: () => import('./features/auth/signin/signin.component').then(m => m.SigninComponent)
  },
  {
    path: 'upload',
    loadComponent: () => import('./features/upload/upload.component').then(m => m.UploadComponent),
    canActivate: [authGuard]
  },
  {
    path: 'skills',
    loadComponent: () => import('./features/skills-review/skills-review.component').then(m => m.SkillsReviewComponent),
    canActivate: [authGuard]
  },
  {
    path: 'admin',
    canActivate: [authGuard],
    children: [
      {
        path: 'dictionary',
        loadComponent: () => import('./features/admin/dictionary/dictionary.component').then(m => m.DictionaryComponent)
      }
    ]
  }
];

