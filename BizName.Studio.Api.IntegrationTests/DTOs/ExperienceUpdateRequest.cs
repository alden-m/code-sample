using Bogus;

namespace BizName.Studio.Api.IntegrationTests.DTOs;

public class ExperienceUpdateRequest
{
    public Guid Id { get; set; }
    public Guid SourceId { get; set; }
    public string? Name { get; set; }
    public List<object>? Rules { get; set; } = new();
    public List<object>? Transformations { get; set; } = new();
    public bool IsActive { get; set; }
    public Dictionary<string, string>? Configuration { get; set; } = new();

    public static ExperienceUpdateRequest Valid(Guid experienceId)
    {
        var faker = new Faker();
        return new ExperienceUpdateRequest
        {
            Id = experienceId,
            SourceId = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            Rules = new List<object>(),
            Transformations = new List<object>(),
            IsActive = false,
            Configuration = new Dictionary<string, string>()
        };
    }

    public static ExperienceUpdateRequest Empty() => new();

    public ExperienceUpdateRequest WithId(Guid id)
    {
        Id = id;
        return this;
    }

    public ExperienceUpdateRequest WithSourceId(Guid sourceId)
    {
        SourceId = sourceId;
        return this;
    }

    public ExperienceUpdateRequest WithName(string? name)
    {
        Name = name;
        return this;
    }

    public ExperienceUpdateRequest WithIsActive(bool isActive)
    {
        IsActive = isActive;
        return this;
    }

    public ExperienceUpdateRequest WithRules(List<object>? rules)
    {
        Rules = rules;
        return this;
    }

    public ExperienceUpdateRequest WithTransformations(List<object>? transformations)
    {
        Transformations = transformations;
        return this;
    }

    public ExperienceUpdateRequest WithConfiguration(Dictionary<string, string>? configuration)
    {
        Configuration = configuration;
        return this;
    }
}
