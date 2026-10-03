using System.Net.Http.Json;
using System.Text.Json;
using NamaskarApp.Models;

namespace NamaskarApp.Services;

/// <summary>
/// Reads today's background photo from the Namaskar daily API (<c>GET v1/daily</c>) and remembers the last
/// one, so the page has a background immediately on the next launch and while offline.
/// </summary>
public sealed class DailyBackgroundService(HttpClient httpClient, IPreferences preferences)
{
	const string CacheKey = "daily_background";

	// Width of the blurred copy: small enough to look soft when stretched, large enough to avoid blocks.
	const int BlurImageWidthPx = 100;

	static readonly JsonSerializerOptions jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

	public DailyBackground? GetCached()
	{
		var json = preferences.Get(CacheKey, string.Empty);
		return json.Length == 0 ? null : JsonSerializer.Deserialize<DailyBackground>(json);
	}

	/// <returns>Today's background, or <see langword="null"/> when the API cannot be reached or has none.</returns>
	public async Task<DailyBackground?> FetchAsync(int screenWidthPx, CancellationToken cancellationToken = default)
	{
		DailyResponse? response;
		try
		{
			response = await httpClient.GetFromJsonAsync<DailyResponse>("v1/daily", jsonOptions, cancellationToken);
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
		{
			return null;
		}

		if (response?.Background is not { Url.Length: > 0 } background)
		{
			return null;
		}

		var dailyBackground = new DailyBackground(
			Resize(background.Url, Math.Clamp(screenWidthPx, 720, 1600)),
			background.Photographer ?? string.Empty,
			background.Source ?? string.Empty,
			background.SourceUrl ?? string.Empty,
			Resize(background.Url, BlurImageWidthPx));

		preferences.Set(CacheKey, JsonSerializer.Serialize(dailyBackground));
		return dailyBackground;
	}

	// The API returns the original photo (~850 KB). Pexels resizes on request (~50 KB at 1080 px wide).
	static string Resize(string url, int widthPx)
	{
		if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
			|| uri.Host != "images.pexels.com"
			|| uri.Query.Length > 0)
		{
			return url;
		}

		return $"{url}?auto=compress&cs=tinysrgb&w={widthPx}";
	}

	sealed record DailyResponse(BackgroundDto? Background);

	// snake_case JSON ("source_url") maps through SnakeCaseLower.
	sealed record BackgroundDto(string Url, string? Photographer, string? Source, string? SourceUrl);
}
