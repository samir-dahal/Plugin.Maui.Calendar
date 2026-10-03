using System.Globalization;

namespace NamaskarApp.Converters;

/// <summary>
/// The small AD label of a day cell: the day number, or "Oct 1" on the first day of an AD month so the
/// month change inside a BS month is visible.
/// </summary>
public sealed class EnglishDayLabelConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is DateTime date
			? date.Day == 1 ? date.ToString("MMM d", CultureInfo.InvariantCulture) : date.Day.ToString(CultureInfo.InvariantCulture)
			: string.Empty;

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotSupportedException();
}
