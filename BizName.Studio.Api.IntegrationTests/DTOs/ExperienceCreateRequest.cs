using Bogus;

namespace BizName.Studio.Api.IntegrationTests.DTOs;

public class ExperienceCreateRequest
{
    public Guid SourceId { get; set; }
    public string? Name { get; set; }
    public List<object>? Rules { get; set; } = new();
    public List<object>? Transformations { get; set; } = new();
    public bool IsActive { get; set; }
    public Dictionary<string, string>? Configuration { get; set; } = new();

    public static ExperienceCreateRequest Valid()
    {
        var faker = new Faker();
        return new ExperienceCreateRequest
        {
            SourceId = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Rules = new List<object>(),
            Transformations = new List<object>(),
            IsActive = false,
            Configuration = new Dictionary<string, string>()
        };
    }

    public ExperienceCreateRequest WithInsertHtmlAction()
    {
        Transformations ??= new List<object>();
        Transformations.Add(new Dictionary<string, object>
        {
            { "selector", ".test-element" },
            { "html", "<div class='inserted-content'>Test HTML</div>" },
            { "position", "After" },
            { "$type", "InsertHtmlAction" }
        });
        return this;
    }

    public static ExperienceCreateRequest Empty() => new();

    public ExperienceCreateRequest WithSourceId(Guid sourceId)
    {
        SourceId = sourceId;
        return this;
    }

    public ExperienceCreateRequest WithName(string? name)
    {
        Name = name;
        return this;
    }

    public ExperienceCreateRequest WithIsActive(bool isActive)
    {
        IsActive = isActive;
        return this;
    }

    public ExperienceCreateRequest WithRules(List<object>? rules)
    {
        Rules = rules;
        return this;
    }

    public ExperienceCreateRequest WithTransformations(List<object>? transformations)
    {
        Transformations = transformations;
        return this;
    }

    public ExperienceCreateRequest WithConfiguration(Dictionary<string, string>? configuration)
    {
        Configuration = configuration;
        return this;
    }
}
