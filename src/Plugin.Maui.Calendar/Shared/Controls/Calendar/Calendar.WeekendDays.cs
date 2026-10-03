namespace Plugin.Maui.Calendar.Controls;

/// <summary>
/// Which weekdays count as the weekend for the weekday titles and the weekend columns, for calendars whose
/// weekend is not Saturday and Sunday (for example Nepal, where only Saturday is a weekend day).
/// </summary>
public partial class Calendar
{
	/// <summary>
	/// Bindable property for WeekendDays
	/// </summary>
	public static readonly BindableProperty WeekendDaysProperty = BindableProperty.Create(
		nameof(WeekendDays),
		typeof(IReadOnlyCollection<DayOfWeek>),
		typeof(Calendar),
		null,
		propertyChanged: OnWeekendDaysChanged
	);

	/// <summary>
	/// Weekdays whose title uses <see cref="WeekendTitleStyle"/> and whose column gets
	/// <see cref="WeekendDayBackgroundColor"/>. <see langword="null"/> (the default) means Saturday and Sunday.
	/// <see cref="Plugin.Maui.Calendar.Interfaces.ICalendarDay.IsWeekend"/> always means Saturday or Sunday.
	/// </summary>
	public IReadOnlyCollection<DayOfWeek> WeekendDays
	{
		get => (IReadOnlyCollection<DayOfWeek>)GetValue(WeekendDaysProperty);
		set => SetValue(WeekendDaysProperty, value);
	}

	static void OnWeekendDaysChanged(BindableObject bindable, object oldValue, object newValue)
	{
		// The titles are rebuilt because UpdateDayTitles only assigns the weekend style, it never removes it.
		((Calendar)bindable).RenderLayout();
	}

	internal bool IsWeekendDay(DayOfWeek day) =>
		WeekendDays?.Contains(day) ?? day is DayOfWeek.Saturday or DayOfWeek.Sunday;
}
