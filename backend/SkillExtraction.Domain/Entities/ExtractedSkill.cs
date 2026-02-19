namespace SkillExtraction.Domain.Entities;

/// <summary>
/// Represents a skill extracted from a document.
/// </summary>
public class ExtractedSkill
{
    public string Name { get; private set; }
    public string? Category { get; private set; }
    public double Confidence { get; private set; }
    public string Snippet { get; private set; }
    public string? Notes { get; set; }

    private ExtractedSkill()
    {
        // Required for serialization/deserialization
        Name = null!;
        Snippet = null!;
    }

    public ExtractedSkill(string name, string? category, double confidence, string snippet, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Skill name cannot be empty.", nameof(name));

        if (confidence < 0 || confidence > 1)
            throw new ArgumentOutOfRangeException(nameof(confidence), "Confidence must be between 0 and 1.");

        if (string.IsNullOrWhiteSpace(snippet))
            throw new ArgumentException("Snippet cannot be empty.", nameof(snippet));

        Name = name;
        Category = category;
        Confidence = confidence;
        Snippet = snippet;
        Notes = notes;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }
}
