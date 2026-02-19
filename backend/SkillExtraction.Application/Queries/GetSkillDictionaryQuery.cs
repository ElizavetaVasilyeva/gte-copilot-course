using MediatR;
using SkillExtraction.Application.Common;

namespace SkillExtraction.Application.Queries;

/// <summary>
/// Query to retrieve the complete skill dictionary.
/// </summary>
public sealed record GetSkillDictionaryQuery : IRequest<GetSkillDictionaryResultDto>;
