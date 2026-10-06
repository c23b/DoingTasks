namespace DoingTasks.Api.Endpoints.Authentication;

internal sealed class CallBackMicrosoft : IEndpoint
{
    //public void MapEndpoint(IEndpointRouteBuilder app)
    //{
    //    app.MapGet("microsoft-callback", async (
    //        string code,
    //        CancellationToken ct) =>
    //    {
    //        if (string.IsNullOrEmpty(code))
    //            return true;

    //        return true;
    //    })
    //    .WithTags(Tags.Authentication)
    //    .AllowAnonymous();
    //}

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("microsoft-callback", async (
            string? code,
            string? codeVerifier,
            string? error,
            string? error_description,
            CancellationToken ct) =>
        {
            if (!string.IsNullOrEmpty(error))
                return Results.BadRequest(new { error, error_description });

            if (string.IsNullOrEmpty(code))
                return Results.BadRequest("Code não recebido.");

            return Results.Ok(new { code, codeVerifier });
        })
        .WithTags(Tags.Authentication)
        .AllowAnonymous();
    }
}
