using Bogus;

namespace BizName.Studio.Api.IntegrationTests.DTOs;

public class ExperienceUpdateRequest
{
    public Guid Id { get; set; }
    public Guid WebsiteId { get; set; }
    public string? Name { get; set; }
    public List<object>? Conditions { get; set; } = new();
    public List<object>? Actions { get; set; } = new();
    public bool IsPublished { get; set; }
    public Dictionary<string, string>? Metadata { get; set; } = new();

    public static ExperienceUpdateRequest Valid(Guid experienceId)
    {
        var faker = new Faker();
        return new ExperienceUpdateRequest
        {
            Id = experienceId,
            WebsiteId = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Conditions = new List<object>(),
            Actions = new List<object>(),
            IsPublished = false,
            Metadata = new Dictionary<string, string>()
        };
    }

    public static ExperienceUpdateRequest Empty() => new();

    public ExperienceUpdateRequest WithId(Guid id)
    {
        Id = id;
        return this;
    }

    public ExperienceUpdateRequest WithWebsiteId(Guid websiteId)
    {
        WebsiteId = websiteId;
        return this;
    }

    public ExperienceUpdateRequest WithName(string? name)
    {
        Name = name;
        return this;
    }

    public ExperienceUpdateRequest WithIsPublished(bool isPublished)
    {
        IsPublished = isPublished;
        return this;
    }

    public ExperienceUpdateRequest WithConditions(List<object>? conditions)
    {
        Conditions = conditions;
        return this;
    }

    public ExperienceUpdateRequest WithActions(List<object>? actions)
    {
        Actions = actions;
        return this;
    }

    public ExperienceUpdateRequest WithMetadata(Dictionary<string, string>? metadata)
    {
        Metadata = metadata;
        return this;
    }
}
