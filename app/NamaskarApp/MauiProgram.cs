using Microsoft.Extensions.Logging;
using NamaskarApp.Services;
using NamaskarApp.ViewModels;
using Plugin.Maui.Calendar.Nepali;

namespace NamaskarApp;

public static class MauiProgram
{
	const string DailyApiBaseAddress = "https://namaskar.runasp.net/";

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// NepDate's conversion data is trusted up to BS 2100; the calendar stops there.
		builder.Services.AddSingleton(new NepaliCalendarSystem(minYear: 2000, maxYear: 2100));
		builder.Services.AddSingleton(Preferences.Default);
		builder.Services.AddSingleton(Browser.Default);
		builder.Services.AddSingleton(sp => new DailyBackgroundService(
			new HttpClient { BaseAddress = new Uri(DailyApiBaseAddress), Timeout = TimeSpan.FromSeconds(15) },
			sp.GetRequiredService<IPreferences>()));
		builder.Services.AddTransient<WelcomeViewModel>();
		builder.Services.AddTransient<CalendarViewModel>();
		builder.Services.AddTransient<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
