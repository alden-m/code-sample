using FluentValidation;

namespace BizName.Studio.Contracts.Experiences.Actions;

public class InsertHtmlAction : IExperienceAction
{
    public string Selector { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;
    public InsertPosition Position { get; set; } = InsertPosition.After;
}

public enum InsertPosition
{
    Before,
    After,
    Replace,
    PrependInside,
    AppendInside
}

public class InsertHtmlActionValidator : AbstractValidator<InsertHtmlAction>
{
    public InsertHtmlActionValidator()
    {
        RuleFor(x => x.Selector)
            .NotEmpty()
            .WithMessage("Selector is required");

        RuleFor(x => x.Html)
            .NotEmpty()
            .WithMessage("HTML content is required");

        RuleFor(x => x.Position)
            .IsInEnum()
            .WithMessage("Position must be a valid value");
    }
}
