using System.ComponentModel;
using NamaskarApp.ViewModels;

namespace NamaskarApp;

public partial class MainPage : ContentPage
{
	const uint TransitionMs = 350;

	readonly CalendarViewModel viewModel;

	public MainPage(CalendarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = this.viewModel = viewModel;
		viewModel.Welcome.PropertyChanged += OnWelcomePropertyChanged;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		var screenWidthPx = (int)DeviceDisplay.Current.MainDisplayInfo.Width;
		await viewModel.LoadBackgroundAsync(screenWidthPx);
	}

	async void OnWelcomePropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(WelcomeViewModel.IsVisible) && !viewModel.Welcome.IsVisible)
		{
			viewModel.Welcome.PropertyChanged -= OnWelcomePropertyChanged;
			await ShowCalendarAsync();
		}
	}

	// The one orchestrated motion: the greeting lifts away while the photo blurs and the calendar fades in.
	async Task ShowCalendarAsync()
	{
		CalendarContent.IsVisible = true;
		WelcomeLayer.InputTransparent = true;

		await Task.WhenAll(
			WelcomeLayer.FadeToAsync(0, TransitionMs, Easing.CubicIn),
			WelcomeLayer.TranslateToAsync(0, -48, TransitionMs, Easing.CubicIn),
			BlurredPhoto.FadeToAsync(1, TransitionMs),
			CalendarContent.FadeToAsync(1, TransitionMs, Easing.CubicOut));

		WelcomeLayer.IsVisible = false;
	}
}
