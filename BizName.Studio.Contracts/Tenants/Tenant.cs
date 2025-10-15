using FluentValidation;
using BizName.Studio.Contracts.Common;

namespace BizName.Studio.Contracts.Tenants;

public class Tenant : IEntity
{
    public required Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public static Tenant New(Guid tenantId, string? name = null) => new() {
        Id = tenantId,
        Name = name ?? $"org_{tenantId}"
    };
}

/// <summary>
/// FluentValidation validator for Tenant entity
/// </summary>
public class TenantValidator : AbstractValidator<Tenant>
{
    public TenantValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("Tenant data is required");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Tenant ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tenant name is required")
            .MinimumLength(2)
            .WithMessage("Tenant name must be at least 2 characters long")
            .MaximumLength(100)
            .WithMessage("Tenant name must not exceed 100 characters");
    }
}
