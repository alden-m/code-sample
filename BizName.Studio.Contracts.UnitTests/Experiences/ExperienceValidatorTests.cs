using FluentAssertions;
using FluentValidation;
using BizName.Studio.Contracts.Experiences;
using System.Reflection;
using Xunit;

namespace BizName.Studio.Contracts.UnitTests.Experiences;

public class ExperienceValidatorTests
{
    [Fact]
    public void Experience_Entity_Should_Have_FluentValidator()
    {
        // Arrange
        var experienceType = typeof(Experience);
        var expectedValidatorType = typeof(AbstractValidator<Experience>);
        var experienceAssembly = experienceType.Assembly;
        
        // Act
        var validatorTypes = experienceAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && expectedValidatorType.IsAssignableFrom(t))
            .ToList();
        
        // Assert
        validatorTypes.Should().NotBeEmpty(
            "Experience entity should have at least one validator that inherits from AbstractValidator<Experience>");
    }
}
