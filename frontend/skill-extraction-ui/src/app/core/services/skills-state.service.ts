import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ExtractedSkill } from '../models/skill.models';

@Injectable({
  providedIn: 'root'
})
export class SkillsStateService {
  private skillsSubject = new BehaviorSubject<ExtractedSkill[]>([]);
  public skills$ = this.skillsSubject.asObservable();

  setSkills(skills: ExtractedSkill[]): void {
    this.skillsSubject.next(skills);
  }

  getSkills(): ExtractedSkill[] {
    return this.skillsSubject.value;
  }

  addSkill(skill: ExtractedSkill): void {
    const currentSkills = this.skillsSubject.value;
    this.skillsSubject.next([...currentSkills, skill]);
  }

  updateSkill(index: number, skill: ExtractedSkill): void {
    const currentSkills = [...this.skillsSubject.value];
    currentSkills[index] = skill;
    this.skillsSubject.next(currentSkills);
  }

  removeSkill(index: number): void {
    const currentSkills = this.skillsSubject.value.filter((_, i) => i !== index);
    this.skillsSubject.next(currentSkills);
  }

  clearSkills(): void {
    this.skillsSubject.next([]);
  }
}
