using System.ComponentModel;
using NamaskarApp.ViewModels;

namespace NamaskarApp;

public partial class MainPage : ContentPage
{
	const uint TransitionMs = 350;
	const uint SnapBackMs = 200;

	// Dragging the welcome screen up further than this share of the page opens the calendar on release.
	const double OpenThreshold = 0.2;

	// The calendar rises this far into place as the welcome screen lifts away.
	const double CalendarRiseDp = 60;

	readonly CalendarViewModel viewModel;
	double dragProgress;

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
			await FinishOpeningCalendarAsync();
		}
	}

	async void OnWelcomePanUpdated(object? sender, PanUpdatedEventArgs e)
	{
		switch (e.StatusType)
		{
			case GestureStatus.Started:
				CalendarContent.IsVisible = true;
				break;
			case GestureStatus.Running:
				dragProgress = Math.Clamp(-e.TotalY / Height, 0, 1);
				ApplyOpenProgress(dragProgress);
				break;
			// Android reports TotalY = 0 on Completed, so the decision uses the last Running value.
			case GestureStatus.Completed or GestureStatus.Canceled:
				if (dragProgress >= OpenThreshold)
				{
					viewModel.Welcome.OpenCalendarCommand.Execute(null);
				}
				else
				{
					await SnapBackAsync();
				}
				break;
		}
	}

	// 0 = welcome screen fully shown, 1 = calendar fully shown. The fades are staggered (welcome gone by 40%,
	// calendar appearing from 20%) so the two layers' text never overlaps at half opacity.
	void ApplyOpenProgress(double progress)
	{
		WelcomeLayer.TranslationY = -progress * Height;
		WelcomeLayer.Opacity = 1 - Math.Clamp(progress / 0.4, 0, 1);
		BlurredPhoto.Opacity = progress;
		CalendarContent.Opacity = Math.Clamp((progress - 0.2) / 0.8, 0, 1);
		CalendarContent.TranslationY = (1 - progress) * CalendarRiseDp;
	}

	// Continues from wherever a drag left off; a tap starts from the beginning.
	async Task FinishOpeningCalendarAsync()
	{
		CalendarContent.IsVisible = true;
		WelcomeLayer.InputTransparent = true;

		await Task.WhenAll(
			WelcomeLayer.TranslateToAsync(0, -Height, TransitionMs, Easing.CubicOut),
			WelcomeLayer.FadeToAsync(0, TransitionMs / 2, Easing.CubicOut),
			BlurredPhoto.FadeToAsync(1, TransitionMs),
			CalendarContent.FadeToAsync(1, TransitionMs, Easing.CubicOut),
			CalendarContent.TranslateToAsync(0, 0, TransitionMs, Easing.CubicOut));

		WelcomeLayer.IsVisible = false;
	}

	async Task SnapBackAsync()
	{
		await Task.WhenAll(
			WelcomeLayer.TranslateToAsync(0, 0, SnapBackMs, Easing.CubicOut),
			WelcomeLayer.FadeToAsync(1, SnapBackMs),
			BlurredPhoto.FadeToAsync(0, SnapBackMs),
			CalendarContent.FadeToAsync(0, SnapBackMs));

		CalendarContent.IsVisible = false;
		dragProgress = 0;
	}
}
