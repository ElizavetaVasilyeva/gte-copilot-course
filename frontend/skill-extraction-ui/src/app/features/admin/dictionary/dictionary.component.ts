import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SkillsService } from '../../../core/services/skills.service';
import { SkillDictionaryItem } from '../../../core/models/skill.models';

@Component({
  selector: 'app-dictionary',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dictionary-container">
      <div class="dictionary-header">
        <h2>Skill Dictionary</h2>
        <p class="dictionary-subtitle">
          Browse all {{ totalSkills }} skills across {{ categories.length }} categories used for CV extraction
        </p>
      </div>

      @if (isLoading) {
        <div class="loading">Loading skill dictionary...</div>
      }

      @if (errorMessage) {
        <div class="error-message">{{ errorMessage }}</div>
      }

      @if (!isLoading && !errorMessage) {
        <div class="category-filter">
          <button 
            (click)="selectedCategory = null" 
            [class.active]="selectedCategory === null"
            class="filter-button">
            All Categories ({{ totalSkills }})
          </button>
          @for (category of categories; track category) {
            <button 
              (click)="selectedCategory = category" 
              [class.active]="selectedCategory === category"
              class="filter-button">
              {{ category }} ({{ getSkillsByCategory(category).length }})
            </button>
          }
        </div>

        <div class="skills-grid">
          @for (skill of filteredSkills; track skill.name) {
            <div class="skill-card">
              <div class="skill-header">
                <h3 class="skill-name">{{ skill.name }}</h3>
                <span class="skill-category">{{ skill.category }}</span>
              </div>
              @if (skill.aliases.length > 0) {
                <div class="skill-aliases">
                  <strong>Aliases:</strong>
                  <div class="alias-tags">
                    @for (alias of skill.aliases; track alias) {
                      <span class="alias-tag">{{ alias }}</span>
                    }
                  </div>
                </div>
              }
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .dictionary-container {
      max-width: 1400px;
      margin: 0 auto;
      padding: 2rem;
    }

    .dictionary-header {
      margin-bottom: 2rem;
    }

    h2 {
      margin: 0 0 0.5rem 0;
      color: #333;
      font-size: 2rem;
    }

    .dictionary-subtitle {
      color: #666;
      font-size: 1rem;
      margin: 0;
    }

    .loading {
      text-align: center;
      padding: 3rem;
      color: #0066cc;
      font-size: 1.1rem;
    }

    .error-message {
      padding: 1rem 1.5rem;
      background-color: #fee;
      border: 2px solid #fcc;
      border-radius: 8px;
      color: #c33;
      font-weight: 500;
      margin-bottom: 2rem;
    }

    .category-filter {
      display: flex;
      flex-wrap: wrap;
      gap: 0.75rem;
      margin-bottom: 2rem;
      padding-bottom: 1.5rem;
      border-bottom: 2px solid #e0e0e0;
    }

    .filter-button {
      padding: 0.625rem 1.25rem;
      background-color: #f5f5f5;
      color: #333;
      border: 2px solid transparent;
      border-radius: 24px;
      font-size: 0.95rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s ease;
    }

    .filter-button:hover {
      background-color: #e8f4ff;
      border-color: #b3d9ff;
    }

    .filter-button.active {
      background-color: #0066cc;
      color: white;
      border-color: #0066cc;
    }

    .skills-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
      gap: 1.25rem;
    }

    .skill-card {
      padding: 1.5rem;
      background: white;
      border: 2px solid #e0e0e0;
      border-radius: 10px;
      transition: all 0.2s ease;
    }

    .skill-card:hover {
      border-color: #0066cc;
      box-shadow: 0 4px 12px rgba(0, 102, 204, 0.15);
      transform: translateY(-2px);
    }

    .skill-header {
      display: flex;
      justify-content: space-between;
      align-items: start;
      gap: 1rem;
      margin-bottom: 1rem;
    }

    .skill-name {
      margin: 0;
      color: #0066cc;
      font-size: 1.15rem;
      font-weight: 600;
    }

    .skill-category {
      padding: 0.25rem 0.75rem;
      background-color: #f0f7ff;
      color: #0066cc;
      border-radius: 12px;
      font-size: 0.8rem;
      font-weight: 500;
      white-space: nowrap;
      flex-shrink: 0;
    }

    .skill-aliases {
      padding-top: 0.75rem;
      border-top: 1px solid #f0f0f0;
    }

    .skill-aliases strong {
      color: #666;
      font-size: 0.875rem;
      display: block;
      margin-bottom: 0.5rem;
    }

    .alias-tags {
      display: flex;
      flex-wrap: wrap;
      gap: 0.5rem;
    }

    .alias-tag {
      display: inline-block;
      padding: 0.25rem 0.75rem;
      background-color: #f9f9f9;
      color: #555;
      border: 1px solid #e0e0e0;
      border-radius: 16px;
      font-size: 0.85rem;
    }
  `]
})
export class DictionaryComponent implements OnInit {
  private skillsService = inject(SkillsService);

  skills: SkillDictionaryItem[] = [];
  categories: string[] = [];
  selectedCategory: string | null = null;
  isLoading = false;
  errorMessage = '';

  get totalSkills(): number {
    return this.skills.length;
  }

  get filteredSkills(): SkillDictionaryItem[] {
    if (this.selectedCategory === null) {
      return this.skills;
    }
    return this.skills.filter(skill => skill.category === this.selectedCategory);
  }

  ngOnInit(): void {
    this.loadDictionary();
  }

  loadDictionary(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.skillsService.getSkillDictionary().subscribe({
      next: (response) => {
        this.skills = response.skills.sort((a, b) => a.name.localeCompare(b.name));
        this.categories = [...new Set(this.skills.map(s => s.category))].sort();
        this.isLoading = false;
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = error.error?.message || 'Failed to load skill dictionary. Please try again.';
      }
    });
  }

  getSkillsByCategory(category: string): SkillDictionaryItem[] {
    return this.skills.filter(skill => skill.category === category);
  }
}

