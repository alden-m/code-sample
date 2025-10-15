using FluentAssertions;
using FluentValidation;
using BizName.Studio.Contracts.Experiences;
using System.Reflection;
using Xunit;

namespace BizName.Studio.Contracts.UnitTests.Experiences.Conditions.Common;

public class ConditionValidatorTests
{
    [Theory]
    [MemberData(nameof(GetConditionTypes))]
    public void Each_Condition_Type_Should_Have_Corresponding_FluentValidator(Type conditionType)
    {
        // Arrange
        var expectedBaseType = typeof(AbstractValidator<>).MakeGenericType(conditionType);
        var conditionAssembly = conditionType.Assembly;
        
        // Act
        var validatorTypes = conditionAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && expectedBaseType.IsAssignableFrom(t))
            .ToList();
        
        // Assert
        validatorTypes.Should().NotBeEmpty(
            $"Condition type '{conditionType.Name}' should have at least one validator that inherits from AbstractValidator<{conditionType.Name}>");
    }
    
    public static IEnumerable<object[]> GetConditionTypes()
    {
        var conditionTypes = Assembly.GetAssembly(typeof(IExperienceCondition))!
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IExperienceCondition).IsAssignableFrom(t))
            .Select(t => new object[] { t });
        
        return conditionTypes;
    }
}
