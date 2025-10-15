using FluentValidation;
using FluentValidation.Results;
using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services.Impl;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using BizName.Studio.Contracts.Tenants;
using BizName.Studio.Contracts.Websites;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace BizName.Studio.App.UnitTests.Services;

public class TenantServiceImplEnsureProvisioningTests
{
    private readonly IApplicationRepository<Tenant> _mockTenantRepo = Substitute.For<IApplicationRepository<Tenant>>();
    private readonly IPartitionedRepository<Website> _mockWebsiteRepo = Substitute.For<IPartitionedRepository<Website>>();
    private readonly IValidator<Tenant> _mockValidator = Substitute.For<IValidator<Tenant>>();
    private readonly TenantServiceImpl _service;

    public TenantServiceImplEnsureProvisioningTests()
    {
        _service = new TenantServiceImpl(_mockTenantRepo, _mockWebsiteRepo, _mockValidator, NullLogger<TenantServiceImpl>.Instance);
    }

    [Fact]
    public async Task EnsureProvisioning_NullTenant_ThrowsArgumentNullException()
    {
        // Arrange
        Tenant tenant = null;
        var act = () => _service.EnsureProvisioning(tenant);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithMessage("*tenant*");
    }

    [Fact]
    public async Task EnsureProvisioning_TenantExists_ReturnsSuccess()
    {
        // Arrange
        var tenant = TestData.NewTenant;
        var existingTenant = TestData.NewTenant;
        _mockTenantRepo.GetAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
            .Returns(existingTenant);

        // Act
        var result = await _service.EnsureProvisioning(tenant);

        // Assert
        result.Success.Should().BeTrue();
        await _mockTenantRepo.DidNotReceive().AddAsync(Arg.Any<Tenant>());
    }

    [Fact]
    public async Task EnsureProvisioning_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var tenant = TestData.NewTenant;
        _mockTenantRepo.GetAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
            .Returns((Tenant)null);
        
        var validationResult = new ValidationResult([new ValidationFailure("Name", "Name is required")]);
        _mockValidator.ValidateAsync(Arg.Any<Tenant>()).Returns(validationResult);

        // Act
        var result = await _service.EnsureProvisioning(tenant);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
    }

    [Fact]
    public async Task EnsureProvisioning_Success_CreatesNewTenant()
    {
        // Arrange
        var tenant = TestData.NewTenant;
        _mockTenantRepo.GetAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
            .Returns((Tenant)null);
        _mockValidator.ValidateAsync(Arg.Any<Tenant>()).Returns(new ValidationResult());

        // Act
        var result = await _service.EnsureProvisioning(tenant);

        // Assert
        result.Success.Should().BeTrue();
        await _mockTenantRepo.Received(1).AddAsync(Arg.Any<Tenant>());
    }
}
