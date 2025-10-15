using System.Net.Http.Json;
using BizName.Studio.Api.IntegrationTests.Common;
using BizName.Studio.Api.IntegrationTests.DTOs;

namespace BizName.Studio.Api.IntegrationTests.Websites;

public abstract class WebsitesTestBase(ApiTestFixture fixture) : IClassFixture<ApiTestFixture>, IAsyncLifetime
{
    protected readonly HttpClient Client = fixture.CreateAuthenticatedClientWithUniqueTenant();
    protected readonly HttpClient AnonymousClient = fixture.CreateAnonymousClient();

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;

    protected async Task<Guid> CreateTestWebsite()
    {
        var website = WebsiteCreateRequest.Valid();
        var response = await Client.PostAsJsonAsync("/api/websites", website);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }

    protected async Task<Guid> CreateTestExperience(Guid websiteId)
    {
        var experience = ExperienceCreateRequest.Valid().WithSourceId(websiteId);
        var response = await Client.PostAsJsonAsync("/api/experiences", experience);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Guid>();
    }
}
