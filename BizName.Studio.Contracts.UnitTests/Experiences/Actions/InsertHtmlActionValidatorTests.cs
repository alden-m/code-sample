using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using BizName.Studio.Contracts.Experiences.Actions;
using Xunit;

namespace BizName.Studio.Contracts.UnitTests.Experiences.Actions;

public class InsertHtmlActionValidatorTests
{
    private readonly InsertHtmlActionValidator _validator = new();


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Selector_Should_Be_Required(string selector)
    {
        // Arrange
        var action = new InsertHtmlAction
        {
            Selector = selector,
            Html = "valid html",
            Position = InsertPosition.After
        };

        // Act & Assert
        _validator.TestValidate(action).ShouldHaveValidationErrorFor(x => x.Selector).WithErrorMessage("Selector is required");
    }

    [Fact]
    public void Selector_Should_Be_Valid_When_Not_Empty()
    {
        // Arrange
        var action = new InsertHtmlAction
        {
            Selector = ".valid-selector",
            Html = "valid html",
            Position = InsertPosition.After
        };

        // Act & Assert
        _validator.TestValidate(action).ShouldNotHaveValidationErrorFor(x => x.Selector);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Html_Should_Be_Required(string html)
    {
        // Arrange
        var action = new InsertHtmlAction
        {
            Selector = ".valid-selector",
            Html = html,
            Position = InsertPosition.After
        };

        // Act & Assert
        _validator.TestValidate(action).ShouldHaveValidationErrorFor(x => x.Html).WithErrorMessage("HTML content is required");
    }

    [Fact]
    public void Html_Should_Be_Valid_When_Not_Empty()
    {
        // Arrange
        var action = new InsertHtmlAction
        {
            Selector = ".valid-selector",
            Html = "<div>Valid HTML</div>",
            Position = InsertPosition.After
        };

        // Act & Assert
        _validator.TestValidate(action).ShouldNotHaveValidationErrorFor(x => x.Html);
    }

    [Theory]
    [InlineData(InsertPosition.Before)]
    [InlineData(InsertPosition.After)]
    [InlineData(InsertPosition.Replace)]
    [InlineData(InsertPosition.PrependInside)]
    [InlineData(InsertPosition.AppendInside)]
    public void Position_Should_Be_Valid_For_All_Enum_Values(InsertPosition position)
    {
        // Arrange
        var action = new InsertHtmlAction
        {
            Selector = ".valid-selector",
            Html = "<div>Valid HTML</div>",
            Position = position
        };

        // Act & Assert
        _validator.TestValidate(action).ShouldNotHaveValidationErrorFor(x => x.Position);
    }

    [Fact]
    public void Valid_InsertHtmlAction_Should_Pass_Validation()
    {
        // Arrange
        var action = new InsertHtmlAction
        {
            Selector = ".valid-selector",
            Html = "<div>Valid HTML content</div>",
            Position = InsertPosition.After
        };

        // Act
        var result = _validator.TestValidate(action);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
