using Bogus;

namespace BizName.Studio.Api.IntegrationTests.DTOs;

public class WebsiteCreateRequest
{
    public string? Name { get; set; }
    public string? Url { get; set; }

    // Builder methods
    public static WebsiteCreateRequest Empty() => new();

    public static WebsiteCreateRequest Valid()
    {
        var faker = new Faker();
        return new WebsiteCreateRequest
        {
            Name = faker.Company.CompanyName(),
            Url = faker.Internet.Url()
        };
    }

    public WebsiteCreateRequest WithName(string name)
    {
        Name = name;
        return this;
    }

    public WebsiteCreateRequest WithUrl(string? url)
    {
        Url = url;
        return this;
    }
}
