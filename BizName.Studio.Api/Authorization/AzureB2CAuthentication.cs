using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace BizName.Studio.Api.Authorization
{
    public static class AzureB2CAuthentication
    {
        public static void AddB2CAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(options =>
                {
                    builder.Configuration.Bind("AzureAd", options);
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var allowedClientApp = builder.Configuration.GetValue<string>("AzureAd:AllowedClientApp");

                            var clientAppId = context?.Principal?.Claims
                                .FirstOrDefault(x => x.Type is "azp" or "appid")?.Value;

                            if (allowedClientApp is not null && !allowedClientApp.Equals(clientAppId))
                            {
                                throw new UnauthorizedAccessException("This client is not authorized");
                            }

                            return Task.CompletedTask;
                        }
                    };
                }, options => { builder.Configuration.Bind("AzureAd", options); });

            builder.Services.AddAuthorization(options =>
            {
                options
                    .FallbackPolicy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new ScopeAuthorizationRequirement([builder.Configuration["AzureAd:Scopes:components.read"]]))
                    .Build();

                options.AddPolicy("ComponentsWrite", policy =>
                    policy.Requirements.Add(new ScopeAuthorizationRequirement([builder.Configuration["AzureAd:Scopes:components.write"]])));
            });
        }
    }
}
