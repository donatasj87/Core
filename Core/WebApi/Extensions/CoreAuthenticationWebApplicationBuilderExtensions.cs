using System.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Donatas.Core.Authorization;
using Donatas.Core.Configuration;

namespace Donatas.Core.WebApi.Extensions
{
    public static class CoreAuthenticationWebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddCoreAuthentication(this WebApplicationBuilder builder)
        {
            // Makes the roles come in 'role' claim
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            // Set up for OpenId / user based authentication
            builder.Services
                .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(builder.Configuration)
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches();

            // Set up for service principal based authentication
            builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration);

            builder.Services.AddAuthorization();

            // Sets up the claim name which will be used by Authorize attribute for open id / user
            builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
                options.TokenValidationParameters.RoleClaimType = "roles"
            );

            builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                options.TokenValidationParameters.RoleClaimType = "roles"
            );

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Roles.Owner, policy => { policy.RequireRole(Roles.Owner); });
                options.AddPolicy(Roles.Contributor, policy => { policy.RequireRole(Roles.Contributor, Roles.Owner); });
                options.AddPolicy(Roles.Reader, policy => { policy.RequireRole(Roles.Reader, Roles.Contributor, Roles.Owner); });
            });

            builder.Services.AddScoped<ICoreAuthorizationService, CoreAuthorizationService>();

            return builder;
        }

        public static AuthorizationPolicyBuilder GetAuthorizationPolicy(this IConfiguration configuration)
        {
            var authorizationPolicy = new AuthorizationPolicyBuilder();//.RequireAuthenticatedUser();
            CoreApplicationOptions coreApplication;

            try
            {
                coreApplication = configuration.GetSection("CoreApplication").Get<CoreApplicationOptions>();
            }
            catch
            {
                throw new ConfigurationErrorsException("CoreApplication section is missing from application configuration");
            }

            if (coreApplication.Environment == CoreEnvironment.Local)
                authorizationPolicy.AddAuthenticationSchemes(OpenIdConnectDefaults.AuthenticationScheme);
            else
                authorizationPolicy.AddAuthenticationSchemes(AppServicesAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme);

            return authorizationPolicy;
        }
    }
}
