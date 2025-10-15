using FluentValidation;

namespace BizName.Studio.Contracts.Experiences.Conditions;

public class UrlContainsCondition : IExperienceCondition
{
    public string SearchText { get; set; } = string.Empty;
}

public class UrlContainsConditionValidator : AbstractValidator<UrlContainsCondition>
{
    public UrlContainsConditionValidator()
    {
        RuleFor(x => x.SearchText)
            .NotEmpty()
            .WithMessage("Search text is required");
    }
}
