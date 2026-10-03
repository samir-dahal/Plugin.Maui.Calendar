using NamaskarApp.ViewModels;

namespace NamaskarApp;

public partial class MainPage : ContentPage
{
	readonly CalendarViewModel viewModel;

	public MainPage(CalendarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = this.viewModel = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		var screenWidthPx = (int)DeviceDisplay.Current.MainDisplayInfo.Width;
		await viewModel.LoadBackgroundAsync(screenWidthPx);
	}
}
