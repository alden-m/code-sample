using Bogus;

namespace BizName.Studio.Api.IntegrationTests.DTOs;

public class WebsiteCreateRequest
{
    public string? Name { get; set; }
    public string? Endpoint { get; set; }

    // Builder methods
    public static WebsiteCreateRequest Empty() => new();

    public static WebsiteCreateRequest Valid()
    {
        var faker = new Faker();
        return new WebsiteCreateRequest
        {
            Name = faker.Company.CompanyName(),
            Endpoint = faker.Internet.Url()
        };
    }

    public WebsiteCreateRequest WithName(string name)
    {
        Name = name;
        return this;
    }

    public WebsiteCreateRequest WithEndpoint(string? endpoint)
    {
        Endpoint = endpoint;
        return this;
    }
}
