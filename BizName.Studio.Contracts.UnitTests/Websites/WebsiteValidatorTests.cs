using FluentAssertions;
using FluentValidation;
using BizName.Studio.Contracts.Websites;
using System.Reflection;
using Xunit;

namespace BizName.Studio.Contracts.UnitTests.Websites;

public class WebsiteValidatorTests
{
    [Fact]
    public void Website_Entity_Should_Have_FluentValidator()
    {
        // Arrange
        var websiteType = typeof(Website);
        var expectedValidatorType = typeof(AbstractValidator<Website>);
        var websiteAssembly = websiteType.Assembly;
        
        // Act
        var validatorTypes = websiteAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && expectedValidatorType.IsAssignableFrom(t))
            .ToList();
        
        // Assert
        validatorTypes.Should().NotBeEmpty(
            "Website entity should have at least one validator that inherits from AbstractValidator<Website>");
    }
}
