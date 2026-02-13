using System.Text.RegularExpressions;
using SkillExtraction.Application.Interfaces;
using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Infrastructure.SkillMatching;

/// <summary>
/// Extracts skills from text using dictionary-based matching with aliases.
/// </summary>
public class DictionaryBasedSkillExtractor : ISkillExtractor
{
    private readonly InMemorySkillDictionary _dictionary;

    public DictionaryBasedSkillExtractor(InMemorySkillDictionary dictionary)
    {
        _dictionary = dictionary;
    }

    public Task<IEnumerable<ExtractedSkill>> ExtractSkillsAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Task.FromResult(Enumerable.Empty<ExtractedSkill>());

        var normalizedText = NormalizeText(text);
        var skills = new List<ExtractedSkill>();
        var skillDefinitions = _dictionary.GetAllSkills();

        foreach (var skillDef in skillDefinitions)
        {
            var (matched, snippet, confidence) = FindSkillInText(skillDef, normalizedText, text);
            
            if (matched)
            {
                var skill = new ExtractedSkill(
                    name: skillDef.Name,
                    category: skillDef.Category,
                    confidence: confidence,
                    snippet: snippet
                );
                
                skills.Add(skill);
            }
        }

        return Task.FromResult(skills.AsEnumerable());
    }

    private static string NormalizeText(string text)
    {
        // Convert to lowercase and normalize whitespace
        var normalized = text.ToLowerInvariant();
        normalized = Regex.Replace(normalized, @"\s+", " ");
        return normalized;
    }

    private static (bool Matched, string Snippet, double Confidence) FindSkillInText(
        SkillDefinition skillDef, 
        string normalizedText, 
        string originalText)
    {
        var lowerName = skillDef.Name.ToLowerInvariant();
        var allTerms = new List<string> { lowerName };
        allTerms.AddRange(skillDef.Aliases.Select(a => a.ToLowerInvariant()));

        foreach (var term in allTerms)
        {
            // Escape special regex characters
            var escapedTerm = Regex.Escape(term);
            
            // Match whole words or terms bounded by word boundaries
            var pattern = $@"\b{escapedTerm}\b";
            var match = Regex.Match(normalizedText, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                // Calculate confidence based on whether it's the main name or an alias
                var confidence = term == lowerName ? 0.95 : 0.85;
                
                // Extract snippet from original text (preserve casing)
                var snippet = ExtractSnippet(originalText, match.Index, match.Length);
                
                return (true, snippet, confidence);
            }
        }

        return (false, string.Empty, 0.0);
    }

    private static string ExtractSnippet(string text, int matchIndex, int matchLength)
    {
        const int contextChars = 50;
        
        // Convert normalized index back to original text position (approximate)
        var start = Math.Max(0, matchIndex - contextChars);
        var end = Math.Min(text.Length, matchIndex + matchLength + contextChars);
        
        var snippet = text.Substring(start, end - start).Trim();
        
        // Add ellipsis if truncated
        if (start > 0)
            snippet = "..." + snippet;
        if (end < text.Length)
            snippet = snippet + "...";

        return snippet;
    }
}
