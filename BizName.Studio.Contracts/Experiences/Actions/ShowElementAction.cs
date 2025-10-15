using FluentValidation;

namespace BizName.Studio.Contracts.Experiences.Actions;

public class ShowElementAction : IExperienceAction
{
    public string Selector { get; set; } = string.Empty;
}

public class ShowElementActionValidator : AbstractValidator<ShowElementAction>
{
    public ShowElementActionValidator()
    {
        RuleFor(x => x.Selector)
            .NotEmpty()
            .WithMessage("Selector is required");
    }
}
