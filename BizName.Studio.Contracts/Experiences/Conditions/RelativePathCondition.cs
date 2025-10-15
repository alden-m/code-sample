using FluentValidation;

namespace BizName.Studio.Contracts.Experiences.Conditions;

public class RelativePathCondition : IExperienceCondition
{
    public string RelativePathPattern { get; set; } = string.Empty;
}

public class RelativePathConditionValidator : AbstractValidator<RelativePathCondition>
{
    public RelativePathConditionValidator()
    {
        RuleFor(x => x.RelativePathPattern)
            .NotEmpty()
            .WithMessage("Relative path pattern is required")
            .Must(path => path.StartsWith("/"))
            .WithMessage("Relative path pattern must start with '/'");
    }
}
