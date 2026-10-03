using Plugin.Maui.Calendar.Controls.ViewLayoutEngines;
using Plugin.Maui.Calendar.Interfaces;

namespace Plugin.Maui.Calendar.Controls;

/// <summary>
/// Optional non-Gregorian month structure. Every calendar-system decision the rest of the control
/// makes goes through the helpers below, so with <see cref="CalendarSystem"/> unset the control
/// behaves exactly as before.
/// </summary>
public partial class Calendar
{
	/// <summary>
	/// Bindable property for CalendarSystem
	/// </summary>
	public static readonly BindableProperty CalendarSystemProperty = BindableProperty.Create(
		nameof(CalendarSystem),
		typeof(ICalendarSystem),
		typeof(Calendar),
		null,
		propertyChanged: OnCalendarSystemChanged
	);

	/// <summary>
	/// Month structure the month layout renders. <see langword="null"/> (the default) renders Gregorian months.
	/// <see cref="Year"/>, <see cref="Month"/> and <see cref="Day"/> always hold Gregorian values;
	/// use <see cref="ShownDate"/> to navigate when a calendar system is set.
	/// </summary>
	public ICalendarSystem CalendarSystem
	{
		get => (ICalendarSystem)GetValue(CalendarSystemProperty);
		set => SetValue(CalendarSystemProperty, value);
	}

	static void OnCalendarSystemChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is not Calendar calendar)
		{
			return;
		}

		calendar.RenderLayout();
		calendar.UpdateLayoutUnitLabel();
		calendar.OnPropertyChanged(nameof(LocalizedYear));
		((Command)calendar.NextYearCommand)?.ChangeCanExecute();
		((Command)calendar.PrevYearCommand)?.ChangeCanExecute();
	}

	IViewLayoutEngine CreateMonthViewEngine() => CalendarSystem is null
		? new MonthViewEngine(FirstDayOfWeek)
		: new CalendarSystemMonthViewEngine(FirstDayOfWeek, CalendarSystem);

	bool IsInShownMonth(DateTime date) => CalendarSystem is null
		? date.Month == ShownDate.Month && date.Year == ShownDate.Year
		: CalendarSystem.GetMonthStart(date) == CalendarSystem.GetMonthStart(ShownDate);

	DateOnly GetMonthStart(DateTime date) => DateOnly.FromDateTime(
		CalendarSystem?.GetMonthStart(date) ?? new DateTime(date.Year, date.Month, 1));

	int GetDayOfMonth(DateTime date) => CalendarSystem?.GetDayOfMonth(date) ?? date.Day;

	string GetShownMonthName() => CalendarSystem?.GetMonthName(ShownDate)
		?? Culture.DateTimeFormat.MonthNames[ShownDate.Month - 1];

	int ShownYear => CalendarSystem?.GetYear(ShownDate) ?? ShownDate.Year;

	DateTime AddYearsToShownDate(int years) => CalendarSystem?.AddMonths(ShownDate, years * 12)
		?? ShownDate.AddYears(years);

	bool CanMoveShownYear(int years)
	{
		var targetYear = ShownYear + years;
		return targetYear >= CalendarSystem.GetYear(CalendarSystem.MinDate)
			&& targetYear <= CalendarSystem.GetYear(CalendarSystem.MaxDate);
	}

	DateTime EffectiveMinimumDate => CalendarSystem is null || MinimumDate > CalendarSystem.MinDate
		? MinimumDate
		: CalendarSystem.MinDate;

	DateTime EffectiveMaximumDate => CalendarSystem is null || MaximumDate < CalendarSystem.MaxDate
		? MaximumDate
		: CalendarSystem.MaxDate;
}
