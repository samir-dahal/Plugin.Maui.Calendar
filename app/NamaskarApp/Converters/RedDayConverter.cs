using System.Globalization;
using Plugin.Maui.Calendar.Nepali;

namespace NamaskarApp.Converters;

/// <summary>
/// True for days a Nepali patro prints in red: Saturdays and public holidays.
/// </summary>
public sealed class RedDayConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is DateTime date && (date.DayOfWeek == DayOfWeek.Saturday || NepaliDayInfo.For(date).IsPublicHoliday);

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotSupportedException();
}
