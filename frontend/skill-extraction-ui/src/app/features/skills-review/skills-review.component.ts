import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { SkillsService } from '../../core/services/skills.service';
import { SkillsStateService } from '../../core/services/skills-state.service';
import { ExtractedSkill } from '../../core/models/skill.models';

@Component({
  selector: 'app-skills-review',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="review-container">
      <div class="header">
        <h2>Review Extracted Skills</h2>
        <div class="actions">
          <button (click)="onBackToUpload()" class="secondary-button">
            Upload New CV
          </button>
          <button (click)="onExport()" class="primary-button" [disabled]="skills.length === 0 || isExporting">
            @if (isExporting) {
              <span>Exporting...</span>
            } @else {
              <span>Export to Excel</span>
            }
          </button>
        </div>
      </div>

      @if (skills.length === 0) {
        <div class="empty-state">
          <p>No skills extracted yet.</p>
          <button (click)="onBackToUpload()" class="primary-button">
            Upload CV
          </button>
        </div>
      } @else {
        <div class="table-container">
          <table>
            <thead>
              <tr>
                <th>Skill Name</th>
                <th>Category</th>
                <th>Confidence</th>
                <th>Snippet</th>
                <th>Notes</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (skill of skills; track $index) {
                <tr>
                  <td>
                    @if (editingIndex === $index) {
                      <input 
                        type="text" 
                        [(ngModel)]="editingSkill.name"
                        class="edit-input"
                        placeholder="Skill name">
                    } @else {
                      <span (dblclick)="onEditSkill($index)">{{ skill.name }}</span>
                    }
                  </td>
                  <td>
                    @if (editingIndex === $index) {
                      <input 
                        type="text" 
                        [(ngModel)]="editingSkill.category"
                        class="edit-input"
                        placeholder="Category">
                    } @else {
                      <span (dblclick)="onEditSkill($index)">{{ skill.category || '-' }}</span>
                    }
                  </td>
                  <td>
                    @if (editingIndex === $index) {
                      <input 
                        type="number" 
                        [(ngModel)]="editingSkill.confidence"
                        min="0"
                        max="1"
                        step="0.05"
                        class="edit-input-small">
                    } @else {
                      <div class="confidence-cell" (dblclick)="onEditSkill($index)">
                        <div class="confidence-bar">
                          <div 
                            class="confidence-fill" 
                            [style.width.%]="skill.confidence * 100">
                          </div>
                        </div>
                        <span class="confidence-text">{{ (skill.confidence * 100).toFixed(0) }}%</span>
                      </div>
                    }
                  </td>
                  <td>
                    @if (editingIndex === $index) {
                      <input 
                        type="text" 
                        [(ngModel)]="editingSkill.snippet"
                        class="edit-input"
                        placeholder="Evidence snippet">
                    } @else {
                      <span class="snippet-cell" (dblclick)="onEditSkill($index)">{{ skill.snippet }}</span>
                    }
                  </td>
                  <td>
                    @if (editingIndex === $index) {
                      <input 
                        type="text" 
                        [(ngModel)]="editingSkill.notes"
                        (keyup.enter)="onSaveSkill($index)"
                        (keyup.escape)="onCancelEdit()"
                        class="edit-input"
                        placeholder="Notes"
                        #notesInput>
                    } @else {
                      <span 
                        (dblclick)="onEditSkill($index)"
                        class="notes-display"
                        [class.empty]="!skill.notes">
                        {{ skill.notes || 'Double-click to edit' }}
                      </span>
                    }
                  </td>
                  <td>
                    <div class="action-buttons">
                      @if (editingIndex === $index) {
                        <button (click)="onSaveSkill($index)" class="save-button" title="Save">
                          ✓
                        </button>
                        <button (click)="onCancelEdit()" class="cancel-button" title="Cancel">
                          ✕
                        </button>
                      } @else {
                        <button (click)="onEditSkill($index)" class="edit-button" title="Edit">
                          ✏️
                        </button>
                        <button (click)="onRemoveSkill($index)" class="delete-button" title="Delete">
                          🗑️
                        </button>
                      }
                    </div>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>

        <div class="add-skill-section">
          <button (click)="onAddSkill()" class="add-button">
            + Add New Skill
          </button>
        </div>

        <div class="summary">
          <p><strong>Total Skills:</strong> {{ skills.length }}</p>
        </div>
      }
    </div>
  `,
  styles: [`
    .review-container {
      max-width: 1200px;
      margin: 2rem auto;
      padding: 2rem;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 2rem;
      flex-wrap: wrap;
      gap: 1rem;
    }

    h2 {
      margin: 0;
      color: #333;
    }

    .actions {
      display: flex;
      gap: 1rem;
    }

    .primary-button {
      padding: 0.75rem 1.5rem;
      background-color: #0066cc;
      color: white;
      border: none;
      border-radius: 4px;
      font-size: 1rem;
      cursor: pointer;
      transition: background-color 0.2s;
    }

    .primary-button:hover:not(:disabled) {
      background-color: #0052a3;
    }

    .primary-button:disabled {
      background-color: #ccc;
      cursor: not-allowed;
    }

    .secondary-button {
      padding: 0.75rem 1.5rem;
      background-color: white;
      color: #0066cc;
      border: 1px solid #0066cc;
      border-radius: 4px;
      font-size: 1rem;
      cursor: pointer;
      transition: background-color 0.2s;
    }

    .secondary-button:hover {
      background-color: #f0f7ff;
    }

    .empty-state {
      text-align: center;
      padding: 3rem;
      background-color: #f9f9f9;
      border-radius: 8px;
    }

    .empty-state p {
      color: #666;
      margin-bottom: 1rem;
    }

    .table-container {
      overflow-x: auto;
      background-color: white;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    table {
      width: 100%;
      border-collapse: collapse;
    }

    th {
      background-color: #f5f5f5;
      padding: 1rem;
      text-align: left;
      font-weight: 600;
      color: #555;
      border-bottom: 2px solid #ddd;
    }

    td {
      padding: 0.75rem 1rem;
      border-bottom: 1px solid #eee;
      vertical-align: middle;
    }

    tr:hover {
      background-color: #fafafa;
    }

    .confidence-cell {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .confidence-bar {
      flex: 1;
      height: 8px;
      background-color: #e0e0e0;
      border-radius: 4px;
      overflow: hidden;
      min-width: 60px;
    }

    .confidence-fill {
      height: 100%;
      background-color: #4caf50;
      transition: width 0.3s;
    }

    .confidence-text {
      font-size: 0.875rem;
      color: #666;
      min-width: 35px;
    }

    .snippet-cell {
      max-width: 300px;
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
      font-size: 0.875rem;
      color: #666;
    }

    .notes-display {
      cursor: pointer;
      display: block;
      padding: 0.25rem;
      border-radius: 3px;
      transition: background-color 0.2s;
    }

    .notes-display:hover {
      background-color: #f0f0f0;
    }

    .notes-display.empty {
      color: #999;
      font-style: italic;
    }

    .notes-input,
    .edit-input {
      width: 100%;
      padding: 0.5rem;
      border: 1px solid #0066cc;
      border-radius: 3px;
      font-size: 0.875rem;
    }

    .edit-input-small {
      width: 80px;
      padding: 0.5rem;
      border: 1px solid #0066cc;
      border-radius: 3px;
      font-size: 0.875rem;
    }

    .action-buttons {
      display: flex;
      gap: 0.5rem;
      justify-content: center;
    }

    .delete-button,
    .edit-button,
    .save-button,
    .cancel-button {
      padding: 0.25rem 0.5rem;
      border: none;
      border-radius: 3px;
      cursor: pointer;
      font-size: 1rem;
      transition: background-color 0.2s;
    }

    .delete-button,
    .edit-button {
      background-color: transparent;
    }

    .delete-button:hover {
      background-color: #fee;
    }

    .edit-button:hover {
      background-color: #e8f4ff;
    }

    .save-button {
      background-color: #4caf50;
      color: white;
    }

    .save-button:hover {
      background-color: #45a049;
    }

    .cancel-button {
      background-color: #f44336;
      color: white;
    }

    .cancel-button:hover {
      background-color: #da190b;
    }

    .add-skill-section {
      margin-top: 1rem;
    }

    .add-button {
      padding: 0.75rem 1.5rem;
      background-color: #4caf50;
      color: white;
      border: none;
      border-radius: 4px;
      font-size: 1rem;
      cursor: pointer;
      transition: background-color 0.2s;
    }

    .add-button:hover {
      background-color: #45a049;
    }

    .summary {
      margin-top: 1.5rem;
      padding: 1rem;
      background-color: #f9f9f9;
      border-radius: 4px;
    }

    .summary p {
      margin: 0;
      color: #555;
    }
  `]
})
export class SkillsReviewComponent implements OnInit {
  skills: ExtractedSkill[] = [];
  editingIndex: number | null = null;
  editingSkill: ExtractedSkill = { name: '', category: '', confidence: 0, snippet: '', notes: '' };
  isExporting = false;

  constructor(
    private skillsService: SkillsService,
    private skillsStateService: SkillsStateService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.skillsStateService.skills$.subscribe(skills => {
      this.skills = skills;
    });
  }

  onEditSkill(index: number): void {
    this.editingIndex = index;
    this.editingSkill = { ...this.skills[index] };
    // Focus input after Angular renders it
    setTimeout(() => {
      const input = document.querySelector('.edit-input') as HTMLInputElement;
      if (input) {
        input.focus();
      }
    }, 0);
  }

  onSaveSkill(index: number): void {
    // Validate
    if (!this.editingSkill.name.trim()) {
      alert('Skill name is required');
      return;
    }
    if (this.editingSkill.confidence < 0 || this.editingSkill.confidence > 1) {
      alert('Confidence must be between 0 and 1');
      return;
    }
    if (!this.editingSkill.snippet.trim()) {
      alert('Snippet is required');
      return;
    }

    this.skillsStateService.updateSkill(index, this.editingSkill);
    this.editingIndex = null;
    this.editingSkill = { name: '', category: '', confidence: 0, snippet: '', notes: '' };
  }

  onCancelEdit(): void {
    this.editingIndex = null;
    this.editingSkill = { name: '', category: '', confidence: 0, snippet: '', notes: '' };
  }

  onRemoveSkill(index: number): void {
    if (confirm('Are you sure you want to remove this skill?')) {
      this.skillsStateService.removeSkill(index);
    }
  }

  onAddSkill(): void {
    const newSkill: ExtractedSkill = {
      name: 'New Skill',
      category: undefined,
      confidence: 0.5,
      snippet: 'Manually added',
      notes: ''
    };
    this.skillsStateService.addSkill(newSkill);
    // Automatically enter edit mode for the new skill
    setTimeout(() => {
      this.onEditSkill(this.skills.length - 1);
    }, 0);
  }

  onBackToUpload(): void {
    this.router.navigate(['/upload']);
  }

  onExport(): void {
    if (this.skills.length === 0 || this.isExporting) {
      return;
    }

    this.isExporting = true;

    this.skillsService.exportSkills(this.skills).subscribe({
      next: (blob) => {
        // Create download link
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        
        // Generate filename with timestamp
        const timestamp = new Date().toISOString().replace(/[:.]/g, '-').slice(0, -5);
        link.download = `ExtractedSkills_${timestamp}.xlsx`;
        
        // Trigger download
        document.body.appendChild(link);
        link.click();
        
        // Cleanup
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
        
        this.isExporting = false;
      },
      error: (error) => {
        console.error('Error exporting skills:', error);
        let errorMessage = 'Failed to export skills. Please try again.';
        
        if (error.error) {
          if (typeof error.error === 'string') {
            errorMessage = error.error;
          } else if (error.error.error) {
            errorMessage = error.error.error;
          } else if (error.error.errors) {
            errorMessage = error.error.errors.join(', ');
          }
        } else if (error.message) {
          errorMessage = error.message;
        }
        
        alert(errorMessage);
        this.isExporting = false;
      }
    });
  }
}
