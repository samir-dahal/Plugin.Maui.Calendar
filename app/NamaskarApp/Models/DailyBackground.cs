namespace NamaskarApp.Models;

/// <summary>Today's background photo from the Namaskar daily API, with its credit.</summary>
/// <param name="BlurImageUrl">
/// A tiny copy of the photo that looks blurred when stretched over the screen (frosted glass behind the
/// calendar without a blur effect). Null in backgrounds cached by older versions.
/// </param>
public sealed record DailyBackground(
	string ImageUrl,
	string Photographer,
	string Source,
	string SourceUrl,
	string? BlurImageUrl = null);
