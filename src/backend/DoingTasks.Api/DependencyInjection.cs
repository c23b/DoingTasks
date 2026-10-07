using DoingTasks.Api.Extensions;
using DoingTasks.Api.Infrastructure;
using System.Reflection;

namespace DoingTasks.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
       this IServiceCollection services,
       Assembly assembly)
    {
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();
        services.AddAuthorization();
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
        services.AddEndpoints(assembly);

        return services;
    }
}
