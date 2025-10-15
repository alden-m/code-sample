using System.Text.Json.Serialization;
using BizName.Studio.Contracts.Common;
using BizName.Studio.Contracts.Experiences.Actions;

namespace BizName.Studio.Contracts.Experiences;

[JsonPolymorphic(TypeDiscriminatorPropertyName = Constants.Experience.ActionTypeDiscriminator)]
[JsonDerivedType(typeof(RedirectAction), nameof(RedirectAction))]
[JsonDerivedType(typeof(UpdateTextAction), nameof(UpdateTextAction))]
[JsonDerivedType(typeof(HideElementAction), nameof(HideElementAction))]
[JsonDerivedType(typeof(ShowElementAction), nameof(ShowElementAction))]
[JsonDerivedType(typeof(InsertHtmlAction), nameof(InsertHtmlAction))]
public interface IExperienceAction
{
}
