using Bogus;
using BizName.Studio.Contracts.Tenants;

namespace BizName.Studio.Api.IntegrationTests
{
    public static class TestData
    {
        private static readonly Faker _faker = new();
        
        public static Guid NewUniqueTenantId => Guid.NewGuid();
        
        public static Tenant NewValidTenant => Tenant.New(Guid.NewGuid(), _faker.Company.CompanyName());
    }
}
