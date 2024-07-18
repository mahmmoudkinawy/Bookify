using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bookify.Api.OpenApi;

public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureNamedOptions<SwaggerGenOptions>
{
    public void Configure(string? name, SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateVersionInfo(description));
        }
    }

    public void Configure(SwaggerGenOptions options)
    {
        Configure(options);
    }
    private static OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        var openApiInfo = new OpenApiInfo
        {
            Title = $"Bookify.Api v{description.ApiVersion}",
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            openApiInfo.Description += " This API version has been deprecated.";
        }

        return openApiInfo;
    }
}
