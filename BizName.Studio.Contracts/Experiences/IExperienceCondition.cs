using System.Text.Json.Serialization;
using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Experiences.Conditions;

namespace BizName.Studio.Contracts.Experiences;

[JsonPolymorphic(TypeDiscriminatorPropertyName = Constants.Experience.ConditionTypeDiscriminator)]
[JsonDerivedType(typeof(RelativePathCondition), nameof(RelativePathCondition))]
[JsonDerivedType(typeof(UrlContainsCondition), nameof(UrlContainsCondition))]
[JsonDerivedType(typeof(DeviceTypeCondition), nameof(DeviceTypeCondition))]
[JsonDerivedType(typeof(LanguageCondition), nameof(LanguageCondition))]
[JsonDerivedType(typeof(TimeRangeCondition), nameof(TimeRangeCondition))]
[JsonDerivedType(typeof(QueryParamCondition), nameof(QueryParamCondition))]
public interface IExperienceCondition
{
}
