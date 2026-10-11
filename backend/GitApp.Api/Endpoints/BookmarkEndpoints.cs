using System.Security.Claims;
using System.Text.Json;
using GitApp.Api.Services;

namespace GitApp.Api.Endpoints;

public static class BookmarkEndpoints
{
    public static void MapBookmarkEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookmarks").RequireAuthorization();

        group.MapGet("", GetAll);
        group.MapPost("", Add);
        group.MapDelete("/{id:long}", Remove);
    }

    private static IResult GetAll(ClaimsPrincipal user, SessionStore sessions)
    {
        var session = sessions.Get(user);
        if (session is null)
            return TypedResults.Unauthorized();

        return TypedResults.Ok(Sorted(session));
    }

    private static IResult Add(JsonElement repo, ClaimsPrincipal user, SessionStore sessions)
    {
        var session = sessions.Get(user);
        if (session is null)
            return TypedResults.Unauthorized();

        if (!TryGetRepoId(repo, out var id))
            return TypedResults.BadRequest("Body must be a GitHub repository object with 'id' and 'full_name'.");

        session.Bookmarks[id] = repo;
        return TypedResults.Ok(Sorted(session));
    }

    private static IResult Remove(long id, ClaimsPrincipal user, SessionStore sessions)
    {
        var session = sessions.Get(user);
        if (session is null)
            return TypedResults.Unauthorized();

        session.Bookmarks.TryRemove(id, out _);
        return TypedResults.NoContent();
    }

    // The object is stored as-is; only the fields needed for keying and sorting are checked.
    private static bool TryGetRepoId(JsonElement repo, out long id)
    {
        id = 0;
        return repo.ValueKind == JsonValueKind.Object
            && repo.TryGetProperty("full_name", out var name) && name.ValueKind == JsonValueKind.String
            && repo.TryGetProperty("id", out var idValue) && idValue.ValueKind == JsonValueKind.Number
            && idValue.TryGetInt64(out id);
    }

    private static List<JsonElement> Sorted(UserSession session) =>
        session.Bookmarks.Values.OrderBy(r => r.GetProperty("full_name").GetString()).ToList();
}
