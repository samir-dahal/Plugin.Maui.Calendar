using System.Globalization;

namespace NamaskarApp.Converters;

/// <summary>True when the bound string is not null or empty.</summary>
public sealed class StringHasTextConverter : IValueConverter
{
	public static StringHasTextConverter Instance { get; } = new();

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is string text && text.Length > 0;

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		throw new NotSupportedException();
}
