using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace BizName.Studio.Contracts.Experiences.Actions;

public class RedirectAction : IExperienceAction
{
        public string PageUrl { get; set; } = string.Empty;
}

public class RedirectActionValidator : AbstractValidator<RedirectAction>
{
    public RedirectActionValidator()
    {
        RuleFor(x => x.PageUrl)
            .NotEmpty()
            .WithMessage("Page URL is required")
            .Must(BeValidUrl)
            .WithMessage("Page URL must be a valid URL");
    }

    private static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
