using FluentValidation;
using BizName.Studio.Contracts.Common;

namespace BizName.Studio.Contracts.Websites;

public class Website : IEntity
{
    public Guid Id { get; set; }

    public string? Name { get; set; } = string.Empty;
    
    public string? Description { get; set; } = string.Empty;
    
    public string? Url { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>
/// FluentValidation validator for Website entity
/// </summary>
public class WebsiteValidator : AbstractValidator<Website>
{
    public WebsiteValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("Website data is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Website name is required")
            .MinimumLength(2)
            .WithMessage("Website name must be at least 2 characters long")
            .MaximumLength(100)
            .WithMessage("Website name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Website description must not exceed 500 characters");

        RuleFor(x => x.Url)
            .Must(BeValidUrlOrEmpty)
            .WithMessage("Website URL must be a valid HTTP or HTTPS URL");

        RuleFor(x => x.CreatedAt)
            .NotEmpty()
            .WithMessage("Created date is required");

        RuleFor(x => x.UpdatedAt)
            .NotEmpty()
            .WithMessage("Updated date is required");
    }

    private static bool BeValidUrlOrEmpty(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true; // Optional field

        return Uri.TryCreate(url, UriKind.Absolute, out var result) 
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
