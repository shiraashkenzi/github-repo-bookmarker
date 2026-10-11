using System.Text.Json;

namespace GitApp.Api.Services;

public class GitHubService(HttpClient http)
{
    // Returns GitHub's repository objects exactly as received, so the entire object can be bookmarked.
    public async Task<IReadOnlyList<JsonElement>> SearchAsync(string keyword, CancellationToken ct)
    {
        var url = $"search/repositories?q={Uri.EscapeDataString(keyword)}";
        var response = await http.GetFromJsonAsync<SearchResponse>(url, ct);

        return response?.Items ?? [];
    }

    private record SearchResponse(List<JsonElement> Items);
}
