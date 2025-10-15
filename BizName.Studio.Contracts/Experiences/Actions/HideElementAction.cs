using FluentValidation;

namespace BizName.Studio.Contracts.Experiences.Actions;

public class HideElementAction : IExperienceAction
{
    public string Selector { get; set; } = string.Empty;
}

public class HideElementActionValidator : AbstractValidator<HideElementAction>
{
    public HideElementActionValidator()
    {
        RuleFor(x => x.Selector)
            .NotEmpty()
            .WithMessage("Selector is required");
    }
}
