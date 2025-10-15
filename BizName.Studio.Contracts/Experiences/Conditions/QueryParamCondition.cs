using FluentValidation;

namespace BizName.Studio.Contracts.Experiences.Conditions;

public class QueryParamCondition : IExperienceCondition
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class QueryParamConditionValidator : AbstractValidator<QueryParamCondition>
{
    public QueryParamConditionValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("Parameter key is required");

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Parameter value is required");
    }
}
