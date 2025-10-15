using FluentValidation;

namespace BizName.Studio.Contracts.Experiences.Conditions;

public class TimeRangeCondition : IExperienceCondition
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class TimeRangeConditionValidator : AbstractValidator<TimeRangeCondition>
{
    public TimeRangeConditionValidator()
    {
        RuleFor(x => x.StartTime)
            .GreaterThanOrEqualTo(TimeSpan.Zero)
            .WithMessage("Start time must be valid (00:00:00 or later)")
            .LessThan(TimeSpan.FromDays(1))
            .WithMessage("Start time must be within a 24-hour range");

        RuleFor(x => x.EndTime)
            .GreaterThanOrEqualTo(TimeSpan.Zero)
            .WithMessage("End time must be valid (00:00:00 or later)")
            .LessThan(TimeSpan.FromDays(1))
            .WithMessage("End time must be within a 24-hour range");

        RuleFor(x => x)
            .Must(x => x.StartTime < x.EndTime)
            .WithMessage("Start time must be earlier than end time");
    }
}
