using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace GitApp.Api.Services;

// Custom in-memory session store. The session id travels inside the JWT as the "sid" claim.
public class SessionStore(JwtOptions options)
{
    private readonly ConcurrentDictionary<string, UserSession> _sessions = new();

    public UserSession Create(string username)
    {
        var session = new UserSession
        {
            Id = Guid.NewGuid().ToString("N"),
            Username = username,
            ExpiresAt = DateTime.UtcNow.AddMinutes(options.ExpiryMinutes)
        };

        _sessions[session.Id] = session;
        return session;
    }

    public UserSession? Get(ClaimsPrincipal user)
    {
        var id = GetSessionId(user);
        if (id is null || !_sessions.TryGetValue(id, out var session))
            return null;

        if (session.ExpiresAt > DateTime.UtcNow)
            return session;

        _sessions.TryRemove(id, out _);
        return null;
    }

    public void Remove(ClaimsPrincipal user)
    {
        var id = GetSessionId(user);
        if (id is not null)
            _sessions.TryRemove(id, out _);
    }

    private static string? GetSessionId(ClaimsPrincipal user) =>
        user.FindFirstValue(JwtRegisteredClaimNames.Sid);
}
