using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NamaskarApp.Models;
using NamaskarApp.Services;
using Plugin.Maui.Calendar.Interfaces;
using Plugin.Maui.Calendar.Nepali;

namespace NamaskarApp.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
	readonly NepaliCalendarSystem calendarSystem;
	readonly DailyBackgroundService backgroundService;
	readonly IBrowser browser;

	public CalendarViewModel(
		NepaliCalendarSystem calendarSystem,
		DailyBackgroundService backgroundService,
		IBrowser browser,
		WelcomeViewModel welcome)
	{
		this.calendarSystem = calendarSystem;
		this.backgroundService = backgroundService;
		this.browser = browser;
		Welcome = welcome;
		UpdateMonth();
		UpdateSelectedDay();
		Background = backgroundService.GetCached();
	}

	public ICalendarSystem CalendarSystem => calendarSystem;

	/// <summary>The today screen shown over the calendar when the app starts.</summary>
	public WelcomeViewModel Welcome { get; }

	/// <summary>Today's background photo, or <see langword="null"/> for the plain flag-blue background.</summary>
	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(PhotoCredit))]
	public partial DailyBackground? Background { get; private set; }

	public string PhotoCredit => Background is { Photographer.Length: > 0 } background
		? $"Photo by {background.Photographer} on {background.Source}"
		: string.Empty;

	public async Task LoadBackgroundAsync(int screenWidthPx)
	{
		// Keep the cached photo when the API cannot be reached.
		if (await backgroundService.FetchAsync(screenWidthPx) is { } background)
		{
			Background = background;
		}
	}

	[RelayCommand]
	Task OpenPhotoSource() => Background is { SourceUrl.Length: > 0 } background
		? browser.OpenAsync(background.SourceUrl, BrowserLaunchMode.SystemPreferred)
		: Task.CompletedTask;

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
				var names = NepaliEventNames.WithoutInternationalObservances(info.Events);
				holidays.Add(new MonthHoliday(calendarSystem.GetDayOfMonth(date), string.Join(", ", names)));
			}
		}
		return holidays;
	}

	// A BS month always spans two AD months, e.g. "Sep – Oct 2026" or "Dec 2026 – Jan 2027".
	static string FormatEnglishSpan(DateTime start, DateTime end) => start.Year == end.Year
		? $"{start:MMM} – {end:MMM yyyy}"
		: $"{start:MMM yyyy} – {end:MMM yyyy}";
}
