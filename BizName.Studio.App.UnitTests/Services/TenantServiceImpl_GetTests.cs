using BizName.Studio.App.Repositories;
using BizName.Studio.App.Services.Impl;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging.Abstractions;
using FluentValidation;
using BizName.Studio.Contracts.Tenants;
using BizName.Studio.Contracts.Websites;

namespace BizName.Studio.App.UnitTests.Services;

public class TenantServiceImplGetTests
{
    private readonly IApplicationRepository<Tenant> _mockTenantRepo = Substitute.For<IApplicationRepository<Tenant>>();
    private readonly IPartitionedRepository<Website> _mockWebsiteRepo = Substitute.For<IPartitionedRepository<Website>>();
    private readonly IValidator<Tenant> _mockValidator = Substitute.For<IValidator<Tenant>>();
    private readonly TenantServiceImpl _service;

    public TenantServiceImplGetTests()
    {
        _service = new TenantServiceImpl(_mockTenantRepo, _mockWebsiteRepo, _mockValidator, NullLogger<TenantServiceImpl>.Instance);
    }

    [Fact]
    public async Task GetById_TenantNotFound_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _mockTenantRepo.GetAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
            .Returns((Tenant)null);

        // Act
        var result = await _service.GetById(tenantId);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorType.Should().Be(OperationErrorType.NotFound);
    }

    [Fact]
    public async Task GetById_TenantFound_ReturnsSuccess()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = TestData.NewTenant;
        _mockTenantRepo.GetAsync(Arg.Any<Expression<Func<Tenant, bool>>>())
            .Returns(tenant);

        // Act
        var result = await _service.GetById(tenantId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().Be(tenant);
    }
}
