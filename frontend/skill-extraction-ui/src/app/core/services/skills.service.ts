import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ExtractedSkill, ExtractSkillsResponse } from '../models/skill.models';

@Injectable({
  providedIn: 'root'
})
export class SkillsService {
  private http = inject(HttpClient);

  extractSkills(cvFile: File, ifuFile?: File): Observable<ExtractSkillsResponse> {
    const formData = new FormData();
    formData.append('cvFile', cvFile);
    
    if (ifuFile) {
      formData.append('ifuFile', ifuFile);
    }

    return this.http.post<ExtractSkillsResponse>(
      `${environment.apiBaseUrl}/skills/extract`,
      formData
    );
  }

  exportSkills(skills: ExtractedSkill[]): Observable<Blob> {
    const payload = {
      skills: skills.map(skill => ({
        name: skill.name,
        category: skill.category,
        confidence: skill.confidence,
        snippet: skill.snippet,
        notes: skill.notes
      }))
    };

    return this.http.post(
      `${environment.apiBaseUrl}/skills/export`,
      payload,
      { responseType: 'blob' }
    );
  }
}
