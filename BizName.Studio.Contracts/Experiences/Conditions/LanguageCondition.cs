using FluentValidation;

namespace BizName.Studio.Contracts.Experiences.Conditions;

public class LanguageCondition : IExperienceCondition
{
    public string[] Languages { get; set; } = Array.Empty<string>();
}

public class LanguageConditionValidator : AbstractValidator<LanguageCondition>
{
    public LanguageConditionValidator()
    {
        RuleFor(x => x.Languages)
            .NotEmpty()
            .WithMessage("At least one language must be specified");

        RuleForEach(x => x.Languages)
            .NotEmpty()
            .WithMessage("Language code cannot be empty")
            .Matches(@"^[a-z]{2}(-[A-Z]{2})?$")
            .WithMessage("Language code must be in format 'xx' or 'xx-XX' (e.g., 'en', 'en-US')");
    }
}
