export interface ExtractedSkill {
  name: string;
  category?: string;
  confidence: number;
  snippet: string;
  notes?: string;
}

export interface ExtractSkillsResponse {
  skills: ExtractedSkill[];
  rawTextLength: number;
}

export interface ExportSkillsRequest {
  skills: ExtractedSkill[];
}
