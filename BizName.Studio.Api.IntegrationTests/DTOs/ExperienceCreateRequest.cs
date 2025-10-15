using Bogus;

namespace BizName.Studio.Api.IntegrationTests.DTOs;

public class ExperienceCreateRequest
{
    public Guid WebsiteId { get; set; }
    public string? Name { get; set; }
    public List<object>? Conditions { get; set; } = new();
    public List<object>? Actions { get; set; } = new();
    public bool IsPublished { get; set; }
    public Dictionary<string, string>? Metadata { get; set; } = new();

    public static ExperienceCreateRequest Valid()
    {
        var faker = new Faker();
        return new ExperienceCreateRequest
        {
            WebsiteId = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Conditions = new List<object>(),
            Actions = new List<object>(),
            IsPublished = false,
            Metadata = new Dictionary<string, string>()
        };
    }

    public ExperienceCreateRequest WithInsertHtmlAction()
    {
        Actions ??= new List<object>();
        Actions.Add(new Dictionary<string, object>
        {
            { "selector", ".test-element" },
            { "html", "<div class='inserted-content'>Test HTML</div>" },
            { "position", "After" },
            { "$type", "InsertHtmlAction" }
        });
        return this;
    }

    public static ExperienceCreateRequest Empty() => new();

    public ExperienceCreateRequest WithWebsiteId(Guid websiteId)
    {
        WebsiteId = websiteId;
        return this;
    }

    public ExperienceCreateRequest WithName(string? name)
    {
        Name = name;
        return this;
    }

    public ExperienceCreateRequest WithIsPublished(bool isPublished)
    {
        IsPublished = isPublished;
        return this;
    }

    public ExperienceCreateRequest WithConditions(List<object>? conditions)
    {
        Conditions = conditions;
        return this;
    }

    public ExperienceCreateRequest WithActions(List<object>? actions)
    {
        Actions = actions;
        return this;
    }

    public ExperienceCreateRequest WithMetadata(Dictionary<string, string>? metadata)
    {
        Metadata = metadata;
        return this;
    }
}
