using System.Security.Claims;
using GitApp.Api.Services;

namespace GitApp.Api.Endpoints;

public static class RepoEndpoints
{
    public static void MapRepoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/repos").RequireAuthorization();

        group.MapGet("/search", Search);
    }

    private static async Task<IResult> Search(
        string? q, ClaimsPrincipal user, SessionStore sessions, GitHubService gitHub, CancellationToken ct)
    {
        if (sessions.Get(user) is null)
            return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(q))
            return TypedResults.BadRequest("Query parameter 'q' is required.");

        try
        {
            return TypedResults.Ok(await gitHub.SearchAsync(q, ct));
        }
        catch (HttpRequestException)
        {
            return TypedResults.Problem("GitHub API request failed.", statusCode: StatusCodes.Status502BadGateway);
        }
    }
}
