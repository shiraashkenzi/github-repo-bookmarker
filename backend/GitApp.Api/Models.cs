using System.Collections.Concurrent;
using System.Text.Json;

namespace GitApp.Api;

public record JwtOptions(string Key, string Issuer, int ExpiryMinutes);

public record DemoUser(string Username, string Password);

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, string Username, DateTime ExpiresAt);

public class UserSession
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public required DateTime ExpiresAt { get; init; }

    // The entire GitHub repository object, keyed by its id, so bookmarking the same repo twice is a no-op.
    public ConcurrentDictionary<long, JsonElement> Bookmarks { get; } = new();
}
