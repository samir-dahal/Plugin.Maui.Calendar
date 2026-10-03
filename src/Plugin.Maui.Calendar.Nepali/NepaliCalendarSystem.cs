using NepDate;
using Plugin.Maui.Calendar.Interfaces;

namespace Plugin.Maui.Calendar.Nepali;

/// <summary>
/// Renders Bikram Sambat (BS) months using NepDate. Month names are English transliterations
/// (Baishakh … Chaitra); day and year numbers are BS values.
/// </summary>
public sealed class NepaliCalendarSystem : ICalendarSystem
{
	/// <summary>
	/// NepDate converts BS 1901–2199. The month grid shows up to six weeks either side of the
	/// configured range, so the range must stay one year inside NepDate's.
	/// </summary>
	public const int MinSupportedYear = 1902;

	/// <inheritdoc cref="MinSupportedYear"/>
	public const int MaxSupportedYear = 2198;

	readonly int minMonthIndex;
	readonly int maxMonthIndex;

	/// <param name="minYear">First BS year the calendar shows (Baishakh 1 is the first day).</param>
	/// <param name="maxYear">Last BS year the calendar shows (the last day of Chaitra is the last day).</param>
	public NepaliCalendarSystem(int minYear = 2000, int maxYear = 2100)
	{
		if (minYear < MinSupportedYear || maxYear > MaxSupportedYear || minYear > maxYear)
		{
			throw new ArgumentOutOfRangeException(
				nameof(minYear),
				$"The BS year range {minYear}–{maxYear} must lie within {MinSupportedYear}–{MaxSupportedYear}.");
		}

		MinDate = new NepaliDate(minYear, 1, 1).EnglishDate;
		MaxDate = new NepaliDate(maxYear, 12, 1).MonthEndDate().EnglishDate;
		minMonthIndex = ToMonthIndex(minYear, 1);
		maxMonthIndex = ToMonthIndex(maxYear, 12);
	}

	public DateTime MinDate { get; }

	public DateTime MaxDate { get; }

	public DateTime GetMonthStart(DateTime date)
	{
		var nepaliDate = ToNepali(date);
		return new NepaliDate(nepaliDate.Year, nepaliDate.Month, 1).EnglishDate;
	}

	public DateTime AddMonths(DateTime date, int months)
	{
		var nepaliDate = ToNepali(date);
		var currentIndex = ToMonthIndex(nepaliDate.Year, nepaliDate.Month);
		var targetIndex = Math.Clamp(currentIndex + months, minMonthIndex, maxMonthIndex);

		// NepDate keeps the day of the month and clamps it to the end of shorter months.
		return nepaliDate.AddMonths(targetIndex - currentIndex).EnglishDate;
	}

	public int GetYear(DateTime date) => ToNepali(date).Year;

	public int GetDayOfMonth(DateTime date) => ToNepali(date).Day;

	public string GetMonthName(DateTime date) => ToNepali(date).MonthName.ToString();

	/// <summary>Returns the last day of the BS month that contains <paramref name="date"/>.</summary>
	public DateTime GetMonthEnd(DateTime date) => ToNepali(date).MonthEndDate().EnglishDate;

	/// <summary>Formats <paramref name="date"/> as a BS date, for example "Wednesday, Kartik 18, 2083".</summary>
	public string GetLongDate(DateTime date) =>
		ToNepali(date).ToLongDateString(leadingZeros: false, displayDayName: true, displayYear: true);

	static NepaliDate ToNepali(DateTime date) => new(date.Date);

	static int ToMonthIndex(int year, int month) => year * 12 + month - 1;
}
