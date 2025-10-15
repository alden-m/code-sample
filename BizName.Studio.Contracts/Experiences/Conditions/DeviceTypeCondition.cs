using FluentValidation;

namespace BizName.Studio.Contracts.Experiences.Conditions;

/// <summary>
/// Triggers the experience based on the user's device type
/// </summary>
/// <example>
/// {
///   "$type": "DeviceTypeCondition",
///   "desktop": false,
///   "mobile": true,
///   "tablet": false
/// }
/// </example>
public class DeviceTypeCondition : IExperienceCondition
{
        /// <summary>
    /// Whether this condition applies to desktop users
    /// </summary>
    /// <example>false</example>
    public bool Desktop { get; set; }
    
    /// <summary>
    /// Whether this condition applies to mobile users
    /// </summary>
    /// <example>true</example>
    public bool Mobile { get; set; }
    
    /// <summary>
    /// Whether this condition applies to tablet users
    /// </summary>
    /// <example>false</example>
    public bool Tablet { get; set; }
}

public class DeviceTypeConditionValidator : AbstractValidator<DeviceTypeCondition>
{
    public DeviceTypeConditionValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Desktop || x.Mobile || x.Tablet)
            .WithMessage("At least one device type must be selected");
    }
}
