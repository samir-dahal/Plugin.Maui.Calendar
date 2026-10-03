using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NamaskarApp.Models;
using Plugin.Maui.Calendar.Interfaces;
using Plugin.Maui.Calendar.Nepali;

namespace NamaskarApp.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
	readonly NepaliCalendarSystem calendarSystem;

	public CalendarViewModel(NepaliCalendarSystem calendarSystem)
	{
		this.calendarSystem = calendarSystem;
		UpdateMonth();
		UpdateSelectedDay();
	}

	public ICalendarSystem CalendarSystem => calendarSystem;

	[ObservableProperty]
	public partial DateTime ShownDate { get; set; } = DateTime.Today;

	[ObservableProperty]
	public partial DateTime? SelectedDate { get; set; } = DateTime.Today;

	[ObservableProperty]
	public partial string MonthName { get; private set; } = string.Empty;

	[ObservableProperty]
	public partial string Year { get; private set; } = string.Empty;

	[ObservableProperty]
	public partial string EnglishMonthSpan { get; private set; } = string.Empty;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(HasMonthHolidays))]
	public partial IReadOnlyList<MonthHoliday> MonthHolidays { get; private set; } = [];

	public bool HasMonthHolidays => MonthHolidays.Count > 0;

	[ObservableProperty]
	public partial string SelectedNepaliDate { get; private set; } = string.Empty;

	[ObservableProperty]
	public partial string SelectedEnglishDate { get; private set; } = string.Empty;

	[ObservableProperty]
	public partial string SelectedTithi { get; private set; } = string.Empty;

	[ObservableProperty]
	public partial bool IsSelectedDayHoliday { get; private set; }

	[ObservableProperty]
	public partial IReadOnlyList<string> SelectedDayEvents { get; private set; } = [];

	partial void OnShownDateChanged(DateTime value) => UpdateMonth();

	partial void OnSelectedDateChanged(DateTime? value) => UpdateSelectedDay();

	[RelayCommand]
	void GoToToday()
	{
		ShownDate = DateTime.Today;
		SelectedDate = DateTime.Today;
	}

	void UpdateMonth()
	{
		var monthStart = calendarSystem.GetMonthStart(ShownDate);
		var monthEnd = calendarSystem.GetMonthEnd(ShownDate);

		MonthName = calendarSystem.GetMonthName(ShownDate);
		Year = calendarSystem.GetYear(ShownDate).ToString();
		EnglishMonthSpan = FormatEnglishSpan(monthStart, monthEnd);
		MonthHolidays = GetHolidays(monthStart, monthEnd);
	}

	void UpdateSelectedDay()
	{
		if (SelectedDate is not DateTime date)
		{
			SelectedNepaliDate = SelectedEnglishDate = SelectedTithi = string.Empty;
			IsSelectedDayHoliday = false;
			SelectedDayEvents = [];
			return;
		}

		var info = NepaliDayInfo.For(date);
		SelectedNepaliDate = calendarSystem.GetLongDate(date);
		SelectedEnglishDate = date.ToString("dddd, d MMMM yyyy");
		SelectedTithi = info.Tithi;
		IsSelectedDayHoliday = info.IsPublicHoliday;
		SelectedDayEvents = info.Events;
	}

	List<MonthHoliday> GetHolidays(DateTime monthStart, DateTime monthEnd)
	{
		var holidays = new List<MonthHoliday>();
		for (var date = monthStart; date <= monthEnd; date = date.AddDays(1))
		{
			var info = NepaliDayInfo.For(date);
			if (info.IsPublicHoliday)
			{
				holidays.Add(new MonthHoliday(calendarSystem.GetDayOfMonth(date), string.Join(", ", info.Events)));
			}
		}
		return holidays;
	}

	// A BS month always spans two AD months, e.g. "Sep – Oct 2026" or "Dec 2026 – Jan 2027".
	static string FormatEnglishSpan(DateTime start, DateTime end) => start.Year == end.Year
		? $"{start:MMM} – {end:MMM yyyy}"
		: $"{start:MMM yyyy} – {end:MMM yyyy}";
}
