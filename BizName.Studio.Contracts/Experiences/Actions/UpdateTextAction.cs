using FluentValidation;

namespace BizName.Studio.Contracts.Experiences.Actions;

public class UpdateTextAction : IExperienceAction
{
        public string Selector { get; set; } = string.Empty;
    public string NewText { get; set; } = string.Empty;
}

public class UpdateTextActionValidator : AbstractValidator<UpdateTextAction>
{
    public UpdateTextActionValidator()
    {
        RuleFor(x => x.Selector)
            .NotEmpty()
            .WithMessage("Selector is required");

        RuleFor(x => x.NewText)
            .NotEmpty()
            .WithMessage("New text is required");
    }
}
