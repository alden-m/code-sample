using FluentValidation;
using BizName.Studio.Contracts.Common;

namespace BizName.Studio.Contracts.Experiences;

/// <summary>
///     Represents a data transformation pipeline configuration
/// </summary>
public class Experience : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SourceId { get; set; }
    public string? Name { get; set; } = string.Empty;
    public List<IExperienceCondition>? Rules { get; set; } = new List<IExperienceCondition>();
    public List<IExperienceAction>? Transformations { get; set; } = new List<IExperienceAction>();
    public bool IsActive { get; set; }
    public Dictionary<string, string> Configuration { get; set; } = new Dictionary<string, string>();
    public int Priority { get; set; } = 0;
    public DateTime? ScheduledTime { get; set; }
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

        RuleFor(x => x.SourceId)
            .NotEmpty()
            .WithMessage("Source ID is required");

        RuleFor(x => x.Transformations)
            .NotNull()
            .WithMessage("Transformations collection is required");

        RuleFor(x => x.Rules)
            .NotNull()
            .WithMessage("Rules collection is required");

        // When active, require at least one rule and one transformation
        When(x => x.IsActive, () => {
            RuleFor(x => x.Rules)
                .Must(rules => rules?.Any() == true)
                .WithMessage("Active pipelines must have at least one rule");
                
            RuleFor(x => x.Transformations)
                .Must(transformations => transformations?.Any() == true)
                .WithMessage("Active pipelines must have at least one transformation");
        });

        // Validate each transformation using its specific validator
        RuleForEach(x => x.Transformations).Custom((transformation, context) =>
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(transformation.GetType());
            if (_serviceProvider.GetService(validatorType) is IValidator validator)
            {
                var result = validator.Validate(new ValidationContext<object>(transformation));
                foreach (var error in result.Errors)
                    context.AddFailure(error.PropertyName, error.ErrorMessage);
            }
        });

        // Validate each rule using its specific validator
        RuleForEach(x => x.Rules).Custom((rule, context) =>
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(rule.GetType());
            if (_serviceProvider.GetService(validatorType) is IValidator validator)
            {
                var result = validator.Validate(new ValidationContext<object>(rule));
                foreach (var error in result.Errors)
                    context.AddFailure(error.PropertyName, error.ErrorMessage);
            }
        });
    }
}
