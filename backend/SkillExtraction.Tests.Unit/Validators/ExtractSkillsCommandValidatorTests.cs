using FluentAssertions;
using SkillExtraction.Application.Commands.Skills.ExtractSkills;

namespace SkillExtraction.Tests.Unit.Validators;

public class ExtractSkillsCommandValidatorTests
{
    private readonly ExtractSkillsCommandValidator _validator;

    public ExtractSkillsCommandValidatorTests()
    {
        _validator = new ExtractSkillsCommandValidator();
    }

    [Fact]
    public void Validate_ValidPdfCommand_PassesValidation()
    {
        // Arrange
        var command = new ExtractSkillsCommand
        {
            CvFileName = "resume.pdf",
            CvFileStream = new MemoryStream()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ValidDocxCommand_PassesValidation()
    {
        // Arrange
        var command = new ExtractSkillsCommand
        {
            CvFileName = "document.docx",
            CvFileStream = new MemoryStream()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyFileName_FailsValidation(string? fileName)
    {
        // Arrange
        var command = new ExtractSkillsCommand
        {
            CvFileName = fileName!,
            CvFileStream = new MemoryStream()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ExtractSkillsCommand.CvFileName));
    }

    [Fact]
    public void Validate_NullStream_FailsValidation()
    {
        // Arrange
        var command = new ExtractSkillsCommand
        {
            CvFileName = "resume.pdf",
            CvFileStream = null!
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ExtractSkillsCommand.CvFileStream));
    }

    [Theory]
    [InlineData("document.pdf")]
    [InlineData("resume.docx")]
    [InlineData("CV.PDF")]
    [InlineData("skills.DOCX")]
    public void Validate_SupportedExtensions_PassValidation(string fileName)
    {
        // Arrange
        var command = new ExtractSkillsCommand
        {
            CvFileName = fileName,
            CvFileStream = new MemoryStream()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("document.txt")]
    [InlineData("resume.doc")]
    [InlineData("file.xlsx")]
    [InlineData("image.jpg")]
    public void Validate_UnsupportedExtensions_FailsValidation(string fileName)
    {
        // Arrange
        var command = new ExtractSkillsCommand
        {
            CvFileName = fileName,
            CvFileStream = new MemoryStream()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == nameof(ExtractSkillsCommand.CvFileName) &&
            (e.ErrorMessage.Contains("pdf", StringComparison.OrdinalIgnoreCase) ||
             e.ErrorMessage.Contains("docx", StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void Validate_WithValidIfuFile_PassesValidation()
    {
        // Arrange
        var command = new ExtractSkillsCommand
        {
            CvFileName = "resume.pdf",
            CvFileStream = new MemoryStream(),
            IfuFileName = "ifu.docx",
            IfuFileStream = new MemoryStream()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithInvalidIfuExtension_FailsValidation()
    {
        // Arrange
        var command = new ExtractSkillsCommand
        {
            CvFileName = "resume.pdf",
            CvFileStream = new MemoryStream(),
            IfuFileName = "ifu.txt",
            IfuFileStream = new MemoryStream()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ExtractSkillsCommand.IfuFileName));
    }

    [Fact]
    public void Validate_FileNameWithPath_PassesValidation()
    {
        // Arrange
        var command = new ExtractSkillsCommand
        {
            CvFileName = "C:\\Documents\\resume.pdf",
            CvFileStream = new MemoryStream()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
