using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Calendar.Nepali;

namespace NamaskarApp.ViewModels;

/// <summary>
/// The today screen shown over the calendar on every app start: a greeting, today's BS and AD dates, the tithi
/// and today's festivals. Today's data comes from NepDate so it works offline and always matches the calendar.
/// </summary>
public partial class WelcomeViewModel : ObservableObject
{
	const string UserNameKey = "user_name";
	const string NamePromptDoneKey = "name_prompt_done";
	const int MaxHighlightedEvents = 2;

	readonly IPreferences preferences;
	readonly string timeOfDayGreeting;

	public WelcomeViewModel(NepaliCalendarSystem calendarSystem, IPreferences preferences)
	{
		this.preferences = preferences;

		var now = DateTime.Now;
		var today = now.Date;
		var info = NepaliDayInfo.For(today);

		timeOfDayGreeting = GreetingFor(now.Hour);
		UserName = preferences.Get(UserNameKey, string.Empty);
		IsNamePromptVisible = !preferences.Get(NamePromptDoneKey, false);

		NepaliDate = $"{calendarSystem.GetMonthName(today)} {calendarSystem.GetDayOfMonth(today)}";
		NepaliYear = calendarSystem.GetYear(today).ToString();
		EnglishDate = today.ToString("dddd, d MMMM yyyy");
		Tithi = info.Tithi;
		IsHoliday = info.IsPublicHoliday;
		Highlights = string.Join(", ", NepaliEventNames.WithoutInternationalObservances(info.Events).Take(MaxHighlightedEvents));
	}

	public string NepaliDate { get; }

	public string NepaliYear { get; }

	public string EnglishDate { get; }

	public string Tithi { get; }

	public bool IsHoliday { get; }

	/// <summary>Today's main Nepali events, for example "Fulpati, Dashain Holiday".</summary>
	public string Highlights { get; }

	public string Greeting => UserName.Length > 0 ? $"{timeOfDayGreeting}, {UserName}" : timeOfDayGreeting;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(Greeting))]
	public partial string UserName { get; private set; }

	[ObservableProperty]
	public partial bool IsNamePromptVisible { get; private set; }

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(SaveNameCommand))]
	public partial string NameInput { get; set; } = string.Empty;

	/// <summary>False while the calendar is open.</summary>
	[ObservableProperty]
	public partial bool IsVisible { get; private set; } = true;

	[RelayCommand(CanExecute = nameof(CanSaveName))]
	void SaveName()
	{
		UserName = NameInput.Trim();
		preferences.Set(UserNameKey, UserName);
		CloseNamePrompt();
	}

	bool CanSaveName() => NameInput.Trim().Length > 0;

	[RelayCommand]
	void SkipName() => CloseNamePrompt();

	[RelayCommand]
	void OpenCalendar() => IsVisible = false;

	/// <summary>Brings the welcome screen back over the calendar.</summary>
	[RelayCommand]
	void Show() => IsVisible = true;

	void CloseNamePrompt()
	{
		preferences.Set(NamePromptDoneKey, true);
		IsNamePromptVisible = false;
	}

	static string GreetingFor(int hour) => hour switch
	{
		< 12 => "Good morning",
		< 17 => "Good afternoon",
		_ => "Good evening",
	};
}
