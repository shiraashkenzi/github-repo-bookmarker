using System.Security.Claims;
using GitApp.Api.Services;

namespace GitApp.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/login", Login);
        group.MapPost("/logout", Logout).RequireAuthorization();
    }

    private static IResult Login(
        LoginRequest request, IReadOnlyList<DemoUser> users, SessionStore sessions, TokenService tokens)
    {
        var user = users.FirstOrDefault(u =>
            u.Username == request.Username && u.Password == request.Password);

        if (user is null)
            return TypedResults.Unauthorized();

        var session = sessions.Create(user.Username);
        var token = tokens.CreateToken(session);

        return TypedResults.Ok(new LoginResponse(token, session.Username, session.ExpiresAt));
    }

    private static IResult Logout(ClaimsPrincipal user, SessionStore sessions)
    {
        sessions.Remove(user);
        return TypedResults.NoContent();
    }
}
