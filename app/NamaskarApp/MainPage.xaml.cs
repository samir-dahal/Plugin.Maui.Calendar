using System.ComponentModel;
using Plugin.Maui.Calendar.Controls;
using Plugin.Maui.Calendar.Nepali;

namespace NamaskarApp;

public partial class MainPage : ContentPage
{
	readonly NepaliCalendarSystem calendarSystem = new(minYear: 2000, maxYear: 2100);

	public MainPage()
	{
		InitializeComponent();

		NepaliCalendar.CalendarSystem = calendarSystem;
		NepaliCalendar.PropertyChanged += OnCalendarPropertyChanged;
	}

	void OnCalendarPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName != nameof(Calendar.SelectedDate) || NepaliCalendar.SelectedDate is not DateTime date)
		{
			return;
		}

		SelectedDateLabel.Text =
			$"{calendarSystem.GetMonthName(date)} {calendarSystem.GetDayOfMonth(date)}, {calendarSystem.GetYear(date)} BS  ·  {date:d MMM yyyy} AD";
	}
}
