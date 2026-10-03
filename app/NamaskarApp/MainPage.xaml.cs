using System.ComponentModel;
using NamaskarApp.ViewModels;

namespace NamaskarApp;

/// <summary>
/// The welcome screen and the calendar share one page and one photo. <see cref="openProgress"/> runs from
/// 0 (welcome screen) to 1 (calendar): dragging the welcome screen up or the calendar header down moves it with
/// the finger, and releasing (or a tap) animates it the rest of the way.
/// </summary>
public partial class MainPage : ContentPage
{
	const uint TransitionMs = 350;

	// Dragging further than this share of the page switches screens on release; otherwise it springs back.
	const double SwitchThreshold = 0.2;

	// The calendar rises this far into place as the welcome screen lifts away.
	const double CalendarRiseDp = 60;

	readonly CalendarViewModel viewModel;
	double openProgress;

	public MainPage(CalendarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = this.viewModel = viewModel;
		viewModel.Welcome.PropertyChanged += OnWelcomePropertyChanged;
		viewModel.PropertyChanged += OnCalendarPropertyChanged;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		var screenWidthPx = (int)DeviceDisplay.Current.MainDisplayInfo.Width;
		await viewModel.LoadBackgroundAsync(screenWidthPx);
	}

	async void OnWelcomePropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(WelcomeViewModel.IsVisible))
		{
			await SettleAsync(calendarOpen: !viewModel.Welcome.IsVisible);
		}
	}

	// A tapped day's details may be below the fold; MakeVisible only scrolls when they are not on screen.
	async void OnCalendarPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(CalendarViewModel.SelectedDate) && CalendarContent.IsVisible)
		{
			await CalendarScroll.ScrollToAsync(SelectedDayCard, ScrollToPosition.MakeVisible, animated: true);
		}
	}

	async void OnWelcomePanUpdated(object? sender, PanUpdatedEventArgs e) =>
		await TrackDragAsync(e, progressFromDrag: totalY => -totalY / Height, startsOpen: false);

	async void OnCalendarHeaderPanUpdated(object? sender, PanUpdatedEventArgs e) =>
		await TrackDragAsync(e, progressFromDrag: totalY => 1 - totalY / Height, startsOpen: true);

	async Task TrackDragAsync(PanUpdatedEventArgs e, Func<double, double> progressFromDrag, bool startsOpen)
	{
		switch (e.StatusType)
		{
			case GestureStatus.Started:
				CalendarContent.IsVisible = WelcomeLayer.IsVisible = true;
				break;
			case GestureStatus.Running:
				ApplyOpenProgress(Math.Clamp(progressFromDrag(e.TotalY), 0, 1));
				break;
			// Android reports TotalY = 0 on Completed, so the decision uses the progress of the last Running event.
			case GestureStatus.Completed or GestureStatus.Canceled:
				var draggedShare = startsOpen ? 1 - openProgress : openProgress;
				if (draggedShare < SwitchThreshold)
				{
					await SettleAsync(calendarOpen: startsOpen);
				}
				else if (startsOpen)
				{
					viewModel.Welcome.ShowCommand.Execute(null);
				}
				else
				{
					viewModel.Welcome.OpenCalendarCommand.Execute(null);
				}
				break;
		}
	}

	// Animates from wherever a drag left off to the given screen, then hides the other one.
	async Task SettleAsync(bool calendarOpen)
	{
		CalendarContent.IsVisible = WelcomeLayer.IsVisible = true;
		CalendarContent.InputTransparent = WelcomeLayer.InputTransparent = true;

		await AnimateOpenProgressAsync(calendarOpen ? 1 : 0);

		WelcomeLayer.IsVisible = !calendarOpen;
		CalendarContent.IsVisible = calendarOpen;
		CalendarContent.InputTransparent = WelcomeLayer.InputTransparent = false;
	}

	Task AnimateOpenProgressAsync(double target)
	{
		var finished = new TaskCompletionSource();
		new Animation(ApplyOpenProgress, openProgress, target).Commit(
			this,
			nameof(AnimateOpenProgressAsync),
			length: TransitionMs,
			easing: Easing.CubicOut,
			finished: (_, _) => finished.TrySetResult());
		return finished.Task;
	}

	// Fades are staggered (welcome gone by 40%, calendar appearing from 20%) so the two layers' text never
	// overlaps at half opacity.
	void ApplyOpenProgress(double progress)
	{
		openProgress = progress;
		WelcomeLayer.TranslationY = -progress * Height;
		WelcomeLayer.Opacity = 1 - Math.Clamp(progress / 0.4, 0, 1);
		BlurredPhoto.Opacity = progress;
		CalendarContent.Opacity = Math.Clamp((progress - 0.2) / 0.8, 0, 1);
		CalendarContent.TranslationY = (1 - progress) * CalendarRiseDp;
	}
}
