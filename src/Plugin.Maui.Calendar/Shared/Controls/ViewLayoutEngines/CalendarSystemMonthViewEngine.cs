using System.Windows.Input;
using Plugin.Maui.Calendar.Interfaces;

namespace Plugin.Maui.Calendar.Controls.ViewLayoutEngines;

/// <summary>
/// Month layout whose month boundaries and navigation come from an <see cref="ICalendarSystem"/>
/// instead of Gregorian months. Navigation never leaves the system's supported range.
/// </summary>
sealed class CalendarSystemMonthViewEngine(DayOfWeek firstDayOfWeek, ICalendarSystem calendarSystem) : ViewLayoutBase(firstDayOfWeek), IViewLayoutEngine
{
	const int monthNumberOfWeeks = 6;

	public void GenerateLayout(
		Grid targetGrid,
		List<DayView> dayViews,
		object bindingContext,
		string daysTitleLabelStyleeBindingName,
		ICommand dayTappedCommand,
		DataTemplate dayViewTemplate
	)
	{
		GenerateWeekLayout(
			targetGrid,
			dayViews,
			bindingContext,
			daysTitleLabelStyleeBindingName,
			dayTappedCommand,
			dayViewTemplate,
			monthNumberOfWeeks
		);
	}

	public DateTime GetFirstDate(DateTime dateToShow)
	{
		return GetFirstDateOfWeek(calendarSystem.GetMonthStart(Clamp(dateToShow)));
	}

	public DateTime GetLastDate(DateTime dateToShow)
	{
		return GetFirstDate(dateToShow).AddDays(monthNumberOfWeeks * numberOfDaysInWeek - 1);
	}

	public DateTime GetNextUnit(DateTime forDate)
	{
		return GetNextUnit(forDate, 1);
	}

	public DateTime GetNextUnit(DateTime forDate, int numberOfUnits)
	{
		return numberOfUnits == 0 ? forDate : calendarSystem.AddMonths(Clamp(forDate), numberOfUnits);
	}

	public DateTime GetPreviousUnit(DateTime forDate)
	{
		return GetPreviousUnit(forDate, 1);
	}

	public DateTime GetPreviousUnit(DateTime forDate, int numberOfUnits)
	{
		return GetNextUnit(forDate, -numberOfUnits);
	}

	DateTime Clamp(DateTime date)
	{
		if (date < calendarSystem.MinDate)
		{
			return calendarSystem.MinDate;
		}
		return date > calendarSystem.MaxDate ? calendarSystem.MaxDate : date;
	}
}
