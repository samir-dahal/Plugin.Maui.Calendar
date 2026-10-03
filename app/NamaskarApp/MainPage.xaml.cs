using System.ComponentModel;
using Plugin.Maui.Calendar.Controls;
using Plugin.Maui.Calendar.Models;
using Plugin.Maui.Calendar.Nepali;

namespace NamaskarApp;

public partial class MainPage : ContentPage
{
	readonly NepaliCalendarSystem calendarSystem = new(minYear: 2000, maxYear: 2100);

	public MainPage()
	{
		InitializeComponent();

		NepaliCalendar.CalendarSystem = calendarSystem;
		NepaliCalendar.SelectedDate = DateTime.Today;
		UpdateHeader();
		UpdateSelectedDay(DateTime.Today);
	}

	void OnTodayClicked(object? sender, EventArgs e)
	{
		NepaliCalendar.ShownDate = DateTime.Today;
		NepaliCalendar.SelectedDate = DateTime.Today;
	}

	void OnShownDatesChanged(object? sender, ShownDatesChangedEventArgs e) => UpdateHeader();

	void OnCalendarPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Calendar.SelectedDate) && NepaliCalendar.SelectedDate is DateTime date)
		{
			UpdateSelectedDay(date);
		}
	}

	void UpdateHeader()
	{
		var shownDate = NepaliCalendar.ShownDate;
		var monthStart = calendarSystem.GetMonthStart(shownDate);
		var monthEnd = calendarSystem.GetMonthEnd(shownDate);

		MonthSpan.Text = calendarSystem.GetMonthName(shownDate);
		YearSpan.Text = calendarSystem.GetYear(shownDate).ToString();
		EnglishSpanLabel.Text = FormatEnglishSpan(monthStart, monthEnd);
	}

	void UpdateSelectedDay(DateTime date)
	{
		SelectedNepaliLabel.Text = calendarSystem.GetLongDate(date);
		SelectedEnglishLabel.Text = date.ToString("dddd, d MMMM yyyy");
	}

	// A BS month always spans two AD months, e.g. "Sep – Oct 2026" or "Dec 2026 – Jan 2027".
	static string FormatEnglishSpan(DateTime start, DateTime end) => start.Year == end.Year
		? $"{start:MMM} – {end:MMM yyyy}"
		: $"{start:MMM yyyy} – {end:MMM yyyy}";
}
