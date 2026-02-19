using MediatR;
using SkillExtraction.Application.Common;
using SkillExtraction.Application.Interfaces;

namespace SkillExtraction.Application.Queries;

/// <summary>
/// Handler for GetSkillDictionaryQuery that returns all skills from the dictionary.
/// </summary>
public sealed class GetSkillDictionaryQueryHandler : IRequestHandler<GetSkillDictionaryQuery, GetSkillDictionaryResultDto>
{
    private readonly ISkillDictionary _dictionary;

    public GetSkillDictionaryQueryHandler(ISkillDictionary dictionary)
    {
        _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
    }

    /// <summary>
    /// Handles the query to retrieve all skills from the dictionary.
    /// </summary>
    /// <param name="request">The query request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Complete skill dictionary with all skill definitions</returns>
    public Task<GetSkillDictionaryResultDto> Handle(GetSkillDictionaryQuery request, CancellationToken cancellationToken)
    {
        var skillDefinitions = _dictionary.GetAllSkills();
        
        var dtos = skillDefinitions.Select(s => new SkillDictionaryItemDto(
            Name: s.Name,
            Category: s.Category ?? "Uncategorized",
            Aliases: s.Aliases.ToList()
        )).ToList();

        var result = new GetSkillDictionaryResultDto(dtos);
        
        return Task.FromResult(result);
    }
}
