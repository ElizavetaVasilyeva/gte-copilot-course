using FluentAssertions;
using NSubstitute;
using SkillExtraction.Application.Commands.Skills.ExportSkills;
using SkillExtraction.Application.Interfaces;
using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Tests.Unit.Commands.Skills;

public class ExportSkillsCommandHandlerTests
{
    private readonly IExcelExporter _excelExporter;
    private readonly ExportSkillsCommandHandler _handler;

    public ExportSkillsCommandHandlerTests()
    {
        _excelExporter = Substitute.For<IExcelExporter>();
        _handler = new ExportSkillsCommandHandler(_excelExporter);
    }

    [Fact]
    public async Task Handle_ValidSkills_ReturnsExcelFileData()
    {
        // Arrange
        var skillDtos = new List<ExportSkillDto>
        {
            new("C#", "Programming Languages", 0.9, "Backend development", null),
            new("Angular", "Frameworks", 0.85, "Frontend framework", null),
            new("SQL", "Databases", 0.75, "Database management", "Expert level")
        };

        var command = new ExportSkillsCommand(skillDtos);

        var excelBytes = new byte[] { 0x50, 0x4B, 0x03, 0x04 }; // Fake Excel file header
        _excelExporter.ExportSkillsAsync(Arg.Any<IEnumerable<ExtractedSkill>>(), Arg.Any<CancellationToken>())
            .Returns(excelBytes);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FileContents.Should().BeEquivalentTo(excelBytes);
        result.FileName.Should().Contain("ExtractedSkills_");
        result.FileName.Should().EndWith(".xlsx");
        result.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        await _excelExporter.Received(1).ExportSkillsAsync(
            Arg.Is<IEnumerable<ExtractedSkill>>(skills => skills.Count() == 3), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptySkillsList_ReturnsExcelWithHeaders()
    {
        // Arrange
        var command = new ExportSkillsCommand(new List<ExportSkillDto>());

        var excelBytes = new byte[] { 0x50, 0x4B, 0x03, 0x04 };
        _excelExporter.ExportSkillsAsync(Arg.Any<IEnumerable<ExtractedSkill>>(), Arg.Any<CancellationToken>())
            .Returns(excelBytes);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FileContents.Should().NotBeEmpty();
        result.FileName.Should().EndWith(".xlsx");

        await _excelExporter.Received(1).ExportSkillsAsync(Arg.Any<IEnumerable<ExtractedSkill>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LargeSkillsList_ReturnsExcelFile()
    {
        // Arrange
        var skillDtos = Enumerable.Range(1, 100)
            .Select(i => new ExportSkillDto(
                $"Skill {i}",
                "Programming Languages",
                0.8 + (i % 20) * 0.01,
                $"Snippet for skill {i}",
                $"Notes for skill {i}"))
            .ToList();

        var command = new ExportSkillsCommand(skillDtos);

        var excelBytes = new byte[10000]; // Simulate larger file
        _excelExporter.ExportSkillsAsync(Arg.Any<IEnumerable<ExtractedSkill>>(), Arg.Any<CancellationToken>())
            .Returns(excelBytes);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FileContents.Should().HaveCount(10000);

        await _excelExporter.Received(1).ExportSkillsAsync(
            Arg.Is<IEnumerable<ExtractedSkill>>(skills => skills.Count() == 100),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FileNameFormat_ContainsTimestamp()
    {
        // Arrange
        var command = new ExportSkillsCommand(new List<ExportSkillDto>());

        _excelExporter.ExportSkillsAsync(Arg.Any<IEnumerable<ExtractedSkill>>(), Arg.Any<CancellationToken>())
            .Returns(new byte[] { 0x01, 0x02 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.FileName.Should().MatchRegex(@"ExtractedSkills_\d{8}_\d{6}\.xlsx");
        // Format: ExtractedSkills_YYYYMMDD_HHMMSS.xlsx
    }

    [Fact]
    public async Task Handle_ExcelExporterFails_ThrowsException()
    {
        // Arrange
        var command = new ExportSkillsCommand(new List<ExportSkillDto>());

        _excelExporter.ExportSkillsAsync(Arg.Any<IEnumerable<ExtractedSkill>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<byte[]>(new InvalidOperationException("Excel generation failed")));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Excel generation failed");

        await _excelExporter.Received(1).ExportSkillsAsync(Arg.Any<IEnumerable<ExtractedSkill>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SkillsWithSpecialCharacters_ProcessesCorrectly()
    {
        // Arrange
        var skillDtos = new List<ExportSkillDto>
        {
            new("C#/.NET", "Programming Languages", 0.95, "Backend with C#", "Backend with \"quotes\""),
            new("Vue.js", "Frameworks", 0.88, "Frontend with Vue", "Notes with, commas"),
            new("Node.js", "Runtime Environments", 0.9, "Server runtime", "Runtime\nwith\nnewlines")
        };

        var command = new ExportSkillsCommand(skillDtos);

        var excelBytes = new byte[] { 0x50, 0x4B };
        _excelExporter.ExportSkillsAsync(Arg.Any<IEnumerable<ExtractedSkill>>(), Arg.Any<CancellationToken>())
            .Returns(excelBytes);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        await _excelExporter.Received(1).ExportSkillsAsync(
            Arg.Is<IEnumerable<ExtractedSkill>>(skills => 
                skills.Any(s => s.Name.Contains('#')) &&
                skills.Any(s => s.Notes != null && s.Notes.Contains(',')) &&
                skills.Any(s => s. Notes != null && s.Notes.Contains('\n'))),
            Arg.Any<CancellationToken>());
    }
}
