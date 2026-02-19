namespace SkillExtraction.Domain.ValueObjects;

/// <summary>
/// Represents a skill definition in the dictionary with its aliases.
/// </summary>
public class SkillDefinition
{
    public string Name { get; private set; }
    public string? Category { get; private set; }
    public IReadOnlyList<string> Aliases { get; private set; }

    private SkillDefinition()
    {
        Name = null!;
        Aliases = new List<string>();
    }

    public SkillDefinition(string name, string? category, IEnumerable<string>? aliases = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Skill name cannot be empty.", nameof(name));

        Name = name;
        Category = category;
        Aliases = aliases?.Where(a => !string.IsNullOrWhiteSpace(a)).ToList() ?? new List<string>();
    }

    /// <summary>
    /// Checks if the given text matches this skill or any of its aliases (case-insensitive).
    /// </summary>
    public bool Matches(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var normalizedText = text.Trim().ToLowerInvariant();
        var normalizedName = Name.ToLowerInvariant();

        if (normalizedText == normalizedName)
            return true;

        return Aliases.Any(alias => alias.ToLowerInvariant() == normalizedText);
    }
}
