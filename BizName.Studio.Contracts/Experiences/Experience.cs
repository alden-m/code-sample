using FluentValidation;
using BizName.Studio.Contracts.Common;

namespace BizName.Studio.Contracts.Experiences;

/// <summary>
///     If you want to create a new experience, use the ExperienceFactory class.
/// </summary>
public class Experience : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WebsiteId { get; set; }
    public string? Name { get; set; } = string.Empty;
    public List<IExperienceCondition>? Conditions { get; set; } = new List<IExperienceCondition>();
    public List<IExperienceAction>? Actions { get; set; } = new List<IExperienceAction>();
    public bool IsPublished { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}

/// <summary>
/// FluentValidation validator for Experience entity
/// </summary>
public class ExperienceValidator : AbstractValidator<Experience>
{
    private readonly IServiceProvider _serviceProvider;

    public ExperienceValidator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        RuleFor(x => x)
            .NotNull()
            .WithMessage("Experience data is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Experience name is required")
            .MinimumLength(2)
            .WithMessage("Experience name must be at least 2 characters long")
            .MaximumLength(100)
            .WithMessage("Experience name must not exceed 100 characters");

        RuleFor(x => x.WebsiteId)
            .NotEmpty()
            .WithMessage("Website ID is required");

        RuleFor(x => x.Actions)
            .NotNull()
            .WithMessage("Actions collection is required");

        RuleFor(x => x.Conditions)
            .NotNull()
            .WithMessage("Conditions collection is required");

        // When published, require at least one condition and one action
        When(x => x.IsPublished, () => {
            RuleFor(x => x.Conditions)
                .Must(conditions => conditions?.Any() == true)
                .WithMessage("Published experiences must have at least one condition");
                
            RuleFor(x => x.Actions)
                .Must(actions => actions?.Any() == true)
                .WithMessage("Published experiences must have at least one action");
        });

        // Validate each action using its specific validator
        RuleForEach(x => x.Actions).Custom((action, context) =>
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(action.GetType());
            if (_serviceProvider.GetService(validatorType) is IValidator validator)
            {
                var result = validator.Validate(new ValidationContext<object>(action));
                foreach (var error in result.Errors)
                    context.AddFailure(error.PropertyName, error.ErrorMessage);
            }
        });

        // Validate each condition using its specific validator
        RuleForEach(x => x.Conditions).Custom((condition, context) =>
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(condition.GetType());
            if (_serviceProvider.GetService(validatorType) is IValidator validator)
            {
                var result = validator.Validate(new ValidationContext<object>(condition));
                foreach (var error in result.Errors)
                    context.AddFailure(error.PropertyName, error.ErrorMessage);
            }
        });
    }
}
