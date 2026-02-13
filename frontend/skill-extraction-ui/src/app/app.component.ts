import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { SkillsStateService } from './core/services/skills-state.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  private authService = inject(AuthService);
  private skillsStateService = inject(SkillsStateService);
  private router = inject(Router);
  
  currentUser$ = this.authService.currentUser$;

  onLogout(): void {
    this.authService.signOut();
    this.skillsStateService.clearSkills();
    this.router.navigate(['/signin']);
  }
}
