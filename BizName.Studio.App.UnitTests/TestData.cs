using BizName.Studio.Contracts.Experiences;
using BizName.Studio.Contracts.Tenants;
using BizName.Studio.Contracts.Websites;

namespace BizName.Studio.App.UnitTests;

public static class TestData
{
    private static readonly Faker _faker = new();

    public static Experience NewExperience => new()
    {
        Id = _faker.Random.Guid(),
        Name = _faker.Commerce.ProductName(),
        WebsiteId = _faker.Random.Guid(),
        IsPublished = _faker.Random.Bool(),
        Conditions = [],
        Actions = []
    };

    public static Tenant NewTenant => Tenant.New(_faker.Random.Guid(), _faker.Company.CompanyName());

    public static Website NewWebsite => new()
    {
        Id = _faker.Random.Guid(),
        Name = _faker.Internet.DomainName(),
        Url = _faker.Internet.Url()
    };
}
