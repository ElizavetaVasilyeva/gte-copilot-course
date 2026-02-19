using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SkillExtraction.Application.Commands.Skills.ExtractSkills;
using SkillExtraction.Application.Interfaces;
using SkillExtraction.Domain.Entities;

namespace SkillExtraction.Tests.Unit.Commands.Skills;

public class ExtractSkillsCommandHandlerTests
{
    private readonly ITextExtractor _textExtractor;
    private readonly ISkillExtractor _skillExtractor;
    private readonly ILogger<ExtractSkillsCommandHandler> _logger;
    private readonly ExtractSkillsCommandHandler _handler;

    public ExtractSkillsCommandHandlerTests()
    {
        _textExtractor = Substitute.For<ITextExtractor>();
        _skillExtractor = Substitute.For<ISkillExtractor>();
        _logger = Substitute.For<ILogger<ExtractSkillsCommandHandler>>();
        _handler = new ExtractSkillsCommandHandler(_textExtractor, _skillExtractor, _logger);
    }

    [Fact]
    public async Task Handle_ValidCvOnly_ReturnsExtractedSkills()
    {
        // Arrange
        var cvStream = new MemoryStream();
        var command = new ExtractSkillsCommand
        {
            CvFileName = "resume.pdf",
            CvFileStream = cvStream
        };

        var extractedText = "C# .NET Angular TypeScript AWS";
        var skills = new List<ExtractedSkill>
        {
            new("C#", "Programming Languages", 0.9, "Strong C# skills"),
            new("Angular", "Frameworks", 0.85, "Frontend with Angular"),
            new("AWS", "Cloud Platforms", 0.7, "AWS experience")
        };

        _textExtractor.ExtractTextAsync(cvStream, "resume.pdf", Arg.Any<CancellationToken>())
            .Returns(extractedText);
        _skillExtractor.ExtractSkillsAsync(Arg.Is<string>(s => s.Contains(extractedText)), Arg.Any<CancellationToken>())
            .Returns(skills);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Skills.Should().HaveCount(3);
        result.Skills.Should().ContainSingle(s => s.Name == "C#");
        result.Skills.Should().ContainSingle(s => s.Name == "Angular");
        result.Skills.Should().ContainSingle(s => s.Name == "AWS");

        await _textExtractor.Received(1).ExtractTextAsync(cvStream, "resume.pdf", Arg.Any<CancellationToken>());
        await _skillExtractor.Received(1).ExtractSkillsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CvAndIfu_CombinesTextAndExtractsSkills()
    {
        // Arrange
        var cvStream = new MemoryStream();
        var ifuStream = new MemoryStream();
        var command = new ExtractSkillsCommand
        {
            CvFileName = "resume.pdf",
            CvFileStream = cvStream,
            IfuFileName = "ifu.docx",
            IfuFileStream = ifuStream
        };

        var cvText = "C# .NET experience";
        var ifuText = "Python Django skills";
        var skills = new List<ExtractedSkill>
        {
            new("C#", "Programming Languages", 0.9, "C# backend"),
            new("Python", "Programming Languages", 0.85, "Python scripting")
        };

        _textExtractor.ExtractTextAsync(cvStream, "resume.pdf", Arg.Any<CancellationToken>())
            .Returns(cvText);
        _textExtractor.ExtractTextAsync(ifuStream, "ifu.docx", Arg.Any<CancellationToken>())
            .Returns(ifuText);
        _skillExtractor.ExtractSkillsAsync(Arg.Is<string>(s => s.Contains(cvText) && s.Contains(ifuText)), Arg.Any<CancellationToken>())
            .Returns(skills);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Skills.Should().HaveCount(2);

        await _textExtractor.Received(1).ExtractTextAsync(cvStream, "resume.pdf", Arg.Any<CancellationToken>());
        await _textExtractor.Received(1).ExtractTextAsync(ifuStream, "ifu.docx", Arg.Any<CancellationToken>());
        await _skillExtractor.Received(1).ExtractSkillsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_EmptyText_ReturnsEmptySkillsList()
    {
        // Arrange
        var cvStream = new MemoryStream();
        var command = new ExtractSkillsCommand
        {
            CvFileName = "empty.pdf",
            CvFileStream = cvStream
        };

        _textExtractor.ExtractTextAsync(cvStream, "empty.pdf", Arg.Any<CancellationToken>())
            .Returns(string.Empty);
        _skillExtractor.ExtractSkillsAsync(string.Empty, Arg.Any<CancellationToken>())
            .Returns(new List<ExtractedSkill>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Skills.Should().BeEmpty();

        await _textExtractor.Received(1).ExtractTextAsync(cvStream, "empty.pdf", Arg.Any<CancellationToken>());
        await _skillExtractor.Received(1).ExtractSkillsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TextExtractionFails_ThrowsException()
    {
        // Arrange
        var cvStream = new MemoryStream();
        var command = new ExtractSkillsCommand
        {
            CvFileName = "corrupt.pdf",
            CvFileStream = cvStream
        };

        _textExtractor.ExtractTextAsync(cvStream, "corrupt.pdf", Arg.Any<CancellationToken>())
            .Returns(Task.FromException<string>(new InvalidOperationException("Corrupt PDF file")));

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Corrupt PDF file");

        await _textExtractor.Received(1).ExtractTextAsync(cvStream, "corrupt.pdf", Arg.Any<CancellationToken>());
        await _skillExtractor.DidNotReceive().ExtractSkillsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
