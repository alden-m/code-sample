using FluentAssertions;
using FluentValidation;
using BizName.Studio.Contracts.Experiences;
using System.Reflection;
using Xunit;

namespace BizName.Studio.Contracts.UnitTests.Experiences.Actions.Common;

public class ActionValidatorTests
{
    [Theory]
    [MemberData(nameof(GetActionTypes))]
    public void Each_Action_Type_Should_Have_Corresponding_FluentValidator(Type actionType)
    {
        // Arrange
        var expectedBaseType = typeof(AbstractValidator<>).MakeGenericType(actionType);
        var actionAssembly = actionType.Assembly;
        
        // Act
        var validatorTypes = actionAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && expectedBaseType.IsAssignableFrom(t))
            .ToList();
        
        // Assert
        validatorTypes.Should().NotBeEmpty(
            $"Action type '{actionType.Name}' should have at least one validator that inherits from AbstractValidator<{actionType.Name}>");
    }
    
    public static IEnumerable<object[]> GetActionTypes()
    {
        var actionTypes = Assembly.GetAssembly(typeof(IExperienceAction))!
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IExperienceAction).IsAssignableFrom(t))
            .Select(t => new object[] { t });
        
        return actionTypes;
    }
}
