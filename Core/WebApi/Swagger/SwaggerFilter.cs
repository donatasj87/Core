using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Donatas.Core.WebApi.Swagger
{
    public class SwaggerFilter : IDocumentFilter
    {
        private const string versionSegment = @"/v\d+(\.\d+)?/";

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var uniqueUrls = new List<string> ();

            uniqueUrls.AddRange(
                swaggerDoc?.Paths
                    .Select(_ => _.Key)
                    .Where(_ => Regex.IsMatch(_, versionSegment)));

            uniqueUrls.AddRange(
                swaggerDoc?.Paths
                    .Select(_ => _.Key)
                    .Where(_ => !Regex.IsMatch(_, versionSegment))
                    .Where(x => !uniqueUrls.Select(_ => Regex.Replace(_, versionSegment, "/")).Contains(x))
            );

            swaggerDoc?.Paths.Where(_ => !uniqueUrls.Contains(_.Key)).ToList().ForEach(_ => swaggerDoc?.Paths.Remove(_.Key));
        }
    }
}
