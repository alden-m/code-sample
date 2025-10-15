using Bogus;

namespace BizName.Studio.Api.IntegrationTests.DTOs;

public class WebsiteUpdateRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Endpoint { get; set; }

    // Builder methods
    public static WebsiteUpdateRequest Empty() => new();

    public static WebsiteUpdateRequest Valid(Guid? id = null)
    {
        var faker = new Faker();
        return new WebsiteUpdateRequest
        {
            Id = id ?? Guid.NewGuid(),
            Name = faker.Company.CompanyName(),
            Endpoint = faker.Internet.Url()
        };
    }

    public WebsiteUpdateRequest WithId(Guid id)
    {
        Id = id;
        return this;
    }

    public WebsiteUpdateRequest WithName(string name)
    {
        Name = name;
        return this;
    }

    public WebsiteUpdateRequest WithEndpoint(string? endpoint)
    {
        Endpoint = endpoint;
        return this;
    }
}
