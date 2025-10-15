using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services.Impl;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using FluentValidation;
using BizName.Studio.Contracts.Tenants;
using BizName.Studio.Contracts.Websites;

namespace BizName.Studio.App.UnitTests.Services;

public class TenantServiceImplDeleteTests
{
    private readonly IApplicationRepository<Tenant> _mockTenantRepo = Substitute.For<IApplicationRepository<Tenant>>();
    private readonly IPartitionedRepository<Website> _mockWebsiteRepo = Substitute.For<IPartitionedRepository<Website>>();
    private readonly IValidator<Tenant> _mockValidator = Substitute.For<IValidator<Tenant>>();
    private readonly TenantServiceImpl _service;

    public TenantServiceImplDeleteTests()
    {
        _service = new TenantServiceImpl(_mockTenantRepo, _mockWebsiteRepo, _mockValidator, NullLogger<TenantServiceImpl>.Instance);
    }

    [Fact]
    public async Task DeleteTenant_EmptyTenantId_ThrowsArgumentException()
    {
        // Arrange
        var emptyTenantId = Guid.Empty;
        var act = () => _service.DeleteTenant(emptyTenantId);

        // Act & Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("TenantId cannot be empty*");
    }

    [Fact]
    public async Task DeleteTenant_TenantNotFound_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _mockTenantRepo.GetAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
            .Returns((Tenant)null);

        // Act
        var result = await _service.DeleteTenant(tenantId);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.NotFound);
        result.ErrorMessage.Should().Contain($"Tenant with ID {tenantId} not found");
    }

    [Fact]
    public async Task DeleteTenant_WebsitesExist_ReturnsValidationFailure()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = TestData.NewTenant;
        tenant.Id = tenantId;
        
        _mockTenantRepo.GetAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
            .Returns(tenant);

        var websites = new List<Website> 
        { 
            TestData.NewWebsite, 
            TestData.NewWebsite 
        };
        _mockWebsiteRepo.ListAsync(tenantId)
            .Returns(websites);

        // Act
        var result = await _service.DeleteTenant(tenantId);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.ValidationError);
        result.ErrorMessage.Should().Be("Delete all existing websites first.");
        await _mockTenantRepo.DidNotReceive().DeleteAsync(Arg.Any<Tenant>());
    }

    [Fact]
    public async Task DeleteTenant_Success_DeletesTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = TestData.NewTenant;
        tenant.Id = tenantId;
        
        _mockTenantRepo.GetAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
            .Returns(tenant);

        _mockWebsiteRepo.ListAsync(tenantId)
            .Returns(new List<Website>());

        // Act
        var result = await _service.DeleteTenant(tenantId);

        // Assert
        result.Success.Should().BeTrue();
        await _mockTenantRepo.Received(1).DeleteAsync(tenant);
    }
}
