import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { SkillsService } from '../../core/services/skills.service';
import { SkillsStateService } from '../../core/services/skills-state.service';

@Component({
  selector: 'app-upload',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="upload-container">
      <h2>Upload CV and Extract Skills</h2>
      
      <div class="upload-form">
        <div class="form-group">
          <label class="field-label">CV File (Required) *</label>
          <div class="file-upload-wrapper">
            <input 
              type="file" 
              id="cvFile" 
              (change)="onCvFileSelected($event)"
              accept=".pdf,.docx"
              [disabled]="isUploading"
              class="file-input"
              #cvFileInput>
            <label for="cvFile" class="file-button" [class.disabled]="isUploading">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
                <polyline points="17 8 12 3 7 8"></polyline>
                <line x1="12" y1="3" x2="12" y2="15"></line>
              </svg>
              <span>Choose CV File</span>
            </label>
            @if (cvFile) {
              <div class="file-selected">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path>
                  <polyline points="14 2 14 8 20 8"></polyline>
                </svg>
                <span class="file-name">{{ cvFile.name }}</span>
                <span class="file-size">({{ formatFileSize(cvFile.size) }})</span>
              </div>
            }
          </div>
          <small>Accepted formats: PDF, DOCX (max 10MB)</small>
        </div>

        <div class="form-group">
          <label class="field-label">IFU File (Optional)</label>
          <div class="file-upload-wrapper">
            <input 
              type="file" 
              id="ifuFile" 
              (change)="onIfuFileSelected($event)"
              accept=".pdf,.docx"
              [disabled]="isUploading"
              class="file-input"
              #ifuFileInput>
            <label for="ifuFile" class="file-button" [class.disabled]="isUploading">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
                <polyline points="17 8 12 3 7 8"></polyline>
                <line x1="12" y1="3" x2="12" y2="15"></line>
              </svg>
              <span>Choose IFU File</span>
            </label>
            @if (ifuFile) {
              <div class="file-selected">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"></path>
                  <polyline points="14 2 14 8 20 8"></polyline>
                </svg>
                <span class="file-name">{{ ifuFile.name }}</span>
                <span class="file-size">({{ formatFileSize(ifuFile.size) }})</span>
              </div>
            }
          </div>
          <small>Accepted formats: PDF, DOCX (max 10MB)</small>
        </div>

        @if (errorMessage) {
          <div class="error-message">
            {{ errorMessage }}
          </div>
        }

        <button 
          (click)="onUpload()" 
          [disabled]="!cvFile || isUploading"
          class="upload-button">
          @if (isUploading) {
            <span>Extracting Skills...</span>
          } @else {
            <span>Extract Skills</span>
          }
        </button>
      </div>
    </div>
  `,
  styles: [`
    .upload-container {
      max-width: 700px;
      margin: 2rem auto;
      padding: 2rem;
    }

    h2 {
      margin-bottom: 1.5rem;
      color: #333;
    }

    .upload-form {
      display: flex;
      flex-direction: column;
      gap: 2rem;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .field-label {
      font-weight: 600;
      color: #333;
      font-size: 1rem;
    }

    .file-upload-wrapper {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }

    .file-input {
      position: absolute;
      width: 1px;
      height: 1px;
      opacity: 0;
      overflow: hidden;
      z-index: -1;
    }

    .file-button {
      display: inline-flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.875rem 1.5rem;
      background: linear-gradient(135deg, #0066cc 0%, #0052a3 100%);
      color: white;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.3s ease;
      box-shadow: 0 2px 8px rgba(0, 102, 204, 0.25);
      align-self: flex-start;
    }

    .file-button:hover {
      background: linear-gradient(135deg, #0052a3 0%, #003d7a 100%);
      box-shadow: 0 4px 12px rgba(0, 102, 204, 0.35);
      transform: translateY(-2px);
    }

    .file-button:active {
      transform: translateY(0);
      box-shadow: 0 2px 6px rgba(0, 102, 204, 0.3);
    }

    .file-button.disabled {
      background: linear-gradient(135deg, #cccccc 0%, #999999 100%);
      cursor: not-allowed;
      box-shadow: none;
      transform: none;
    }

    .file-button svg {
      flex-shrink: 0;
    }

    .file-selected {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.875rem 1.25rem;
      background-color: #f0f7ff;
      border: 2px solid #0066cc;
      border-radius: 8px;
      color: #0066cc;
      font-size: 0.95rem;
    }

    .file-selected svg {
      flex-shrink: 0;
      color: #0066cc;
    }

    .file-name {
      font-weight: 500;
      color: #333;
    }

    .file-size {
      color: #666;
      font-size: 0.875rem;
    }

    small {
      color: #777;
      font-size: 0.875rem;
    }

    .error-message {
      padding: 0.875rem 1.25rem;
      background-color: #fee;
      border: 2px solid #fcc;
      border-radius: 8px;
      color: #c33;
      font-weight: 500;
    }

    .upload-button {
      padding: 0.875rem 2rem;
      background: linear-gradient(135deg, #0066cc 0%, #0052a3 100%);
      color: white;
      border: none;
      border-radius: 8px;
      font-size: 1.05rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.3s ease;
      box-shadow: 0 4px 12px rgba(0, 102, 204, 0.3);
      margin-top: 1rem;
    }

    .upload-button:hover:not(:disabled) {
      background: linear-gradient(135deg, #0052a3 0%, #003d7a 100%);
      box-shadow: 0 6px 16px rgba(0, 102, 204, 0.4);
      transform: translateY(-2px);
    }

    .upload-button:active:not(:disabled) {
      transform: translateY(0);
      box-shadow: 0 3px 10px rgba(0, 102, 204, 0.35);
    }

    .upload-button:disabled {
      background: linear-gradient(135deg, #cccccc 0%, #999999 100%);
      cursor: not-allowed;
      box-shadow: none;
      transform: none;
    }
  `]
})
export class UploadComponent {
  cvFile: File | null = null;
  ifuFile: File | null = null;
  isUploading = false;
  errorMessage = '';

  constructor(
    private skillsService: SkillsService,
    private skillsStateService: SkillsStateService,
    private router: Router
  ) {}

  onCvFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      if (this.validateFile(file)) {
        this.cvFile = file;
        this.errorMessage = '';
      } else {
        this.cvFile = null;
        input.value = '';
      }
    }
  }

  onIfuFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      if (this.validateFile(file)) {
        this.ifuFile = file;
        this.errorMessage = '';
      } else {
        this.ifuFile = null;
        input.value = '';
      }
    }
  }

  validateFile(file: File): boolean {
    const validExtensions = ['.pdf', '.docx'];
    const fileExtension = file.name.toLowerCase().substring(file.name.lastIndexOf('.'));
    
    if (!validExtensions.includes(fileExtension)) {
      this.errorMessage = `Invalid file type. Only ${validExtensions.join(', ')} files are allowed.`;
      return false;
    }

    const maxSize = 10 * 1024 * 1024; // 10MB
    if (file.size > maxSize) {
      this.errorMessage = 'File size exceeds 10MB limit.';
      return false;
    }

    return true;
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i];
  }

  onUpload(): void {
    if (!this.cvFile) {
      this.errorMessage = 'Please select a CV file.';
      return;
    }

    this.isUploading = true;
    this.errorMessage = '';

    this.skillsService.extractSkills(this.cvFile, this.ifuFile || undefined)
      .subscribe({
        next: (response) => {
          this.skillsStateService.setSkills(response.skills);
          this.router.navigate(['/skills']);
        },
        error: (error) => {
          this.isUploading = false;
          this.errorMessage = error.error?.message || 'An error occurred while extracting skills. Please try again.';
        }
      });
  }
}
