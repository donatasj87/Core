using Microsoft.AspNetCore.Builder;
using Donatas.Core.WebApi.Middlewares;

namespace Donatas.Core.WebApi.Extensions
{
    public static class CoreWebApplicationExtensions
    {
        public static WebApplication UseCoreWebApplication(this WebApplication app)
        {
            app.UseCoreSwagger();
            app.UseMiddleware<CoreWebApplicationExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();//.RequireAuthorization(app.Configuration.GetAuthorizationPolicy().Build());

            app.UseCsp(options => options.DefaultSources(s => s.Self()));
            app.UseXXssProtection(options => options.Disabled());
            app.UseReferrerPolicy(opts => opts.StrictOrigin());
            app.UseXContentTypeOptions();
            app.UseXfo(options => options.SameOrigin());

            return app;
        }
    }
}
