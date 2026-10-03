using NepDate;

namespace Plugin.Maui.Calendar.Nepali;

/// <summary>
/// Tithi, public-holiday flag and events of one day, from NepDate's calendar data (BS 2001–2089).
/// Days outside that range have an empty tithi, no holiday and no events.
/// </summary>
public sealed record NepaliDayInfo(string Tithi, bool IsPublicHoliday, IReadOnlyList<string> Events)
{
	public static NepaliDayInfo For(DateTime date)
	{
		var nepaliDate = new NepaliDate(date.Date);
		return new NepaliDayInfo(nepaliDate.TithiEn ?? string.Empty, nepaliDate.IsPublicHoliday, nepaliDate.EventsEn ?? []);
	}
}
