namespace BizName.Studio.Contracts.Common;

/// <summary>
/// Application-wide constants
/// </summary>
public static class Constants
{
    /// <summary>
    /// Experience system related constants
    /// </summary>
    public static class Experience
    {
        /// <summary>
        /// JSON discriminator property name for IExperienceAction polymorphic serialization
        /// </summary>
        public const string ActionTypeDiscriminator = "$type";

        /// <summary>
        /// JSON discriminator property name for IExperienceCondition polymorphic serialization
        /// </summary>
        public const string ConditionTypeDiscriminator = "$type";
    }
}
