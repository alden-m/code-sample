using FluentAssertions;
using FluentValidation;
using BizName.Studio.Contracts.Tenants;
using System.Reflection;
using Xunit;

namespace BizName.Studio.Contracts.UnitTests.Tenants;

public class TenantValidatorTests
{
    [Fact]
    public void Tenant_Entity_Should_Have_FluentValidator()
    {
        // Arrange
        var tenantType = typeof(Tenant);
        var expectedValidatorType = typeof(AbstractValidator<Tenant>);
        var tenantAssembly = tenantType.Assembly;
        
        // Act
        var validatorTypes = tenantAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && expectedValidatorType.IsAssignableFrom(t))
            .ToList();
        
        // Assert
        validatorTypes.Should().NotBeEmpty(
            "Tenant entity should have at least one validator that inherits from AbstractValidator<Tenant>");
    }
}
