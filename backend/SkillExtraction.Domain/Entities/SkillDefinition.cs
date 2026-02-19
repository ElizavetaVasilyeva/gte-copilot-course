namespace SkillExtraction.Domain.Entities;

/// <summary>
/// Represents a skill definition in the dictionary with aliases and categorization.
/// </summary>
public class SkillDefinition
{
    public string Name { get; private set; }
    public string? Category { get; private set; }
    public IReadOnlyList<string> Aliases { get; private set; }

    private SkillDefinition()
    {
        // Required for serialization/deserialization
        Name = null!;
        Aliases = Array.Empty<string>();
    }

    public SkillDefinition(string name, string? category = null, IEnumerable<string>? aliases = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Skill name cannot be empty.", nameof(name));

        Name = name;
        Category = category;
        Aliases = aliases?.ToList() ?? new List<string>();
    }

    public bool MatchesText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var lowerText = text.ToLowerInvariant();
        var lowerName = Name.ToLowerInvariant();

        // Check exact name match
        if (lowerText.Contains(lowerName))
            return true;

        // Check aliases
        return Aliases.Any(alias => lowerText.Contains(alias.ToLowerInvariant()));
    }
}
