namespace NamaskarApp.Models;

/// <summary>Today's background photo from the Namaskar daily API, with its credit.</summary>
public sealed record DailyBackground(string ImageUrl, string Photographer, string Source, string SourceUrl);
