namespace Plugin.Maui.Calendar.Interfaces;

/// <summary>
/// Supplies the month structure a <see cref="Controls.Calendar"/> renders instead of Gregorian months
/// (for example Bikram Sambat). Days are still identified by their <see cref="DateTime"/>; only month
/// boundaries, month/year labels, day numbers and month/year navigation come from the implementation.
/// </summary>
/// <remarks>
/// The month grid shows up to six weeks before <see cref="MinDate"/> and after <see cref="MaxDate"/>,
/// so every member must accept dates in that margin.
/// </remarks>
public interface ICalendarSystem
{
	/// <summary>First day the calendar can show. Navigation stops at its month.</summary>
	DateTime MinDate { get; }

	/// <summary>Last day the calendar can show. Navigation stops at its month.</summary>
	DateTime MaxDate { get; }

	/// <summary>Returns the first day of the month that contains <paramref name="date"/>.</summary>
	DateTime GetMonthStart(DateTime date);

	/// <summary>
	/// Moves <paramref name="date"/> by <paramref name="months"/> (negative moves back), keeping the day
	/// of the month where the target month has it, and staying within the months of
	/// <see cref="MinDate"/> and <see cref="MaxDate"/>.
	/// </summary>
	DateTime AddMonths(DateTime date, int months);

	/// <summary>Returns the year number of <paramref name="date"/>.</summary>
	int GetYear(DateTime date);

	/// <summary>Returns the day-of-month number of <paramref name="date"/>.</summary>
	int GetDayOfMonth(DateTime date);

	/// <summary>Returns the display name of the month that contains <paramref name="date"/>.</summary>
	string GetMonthName(DateTime date);
}
