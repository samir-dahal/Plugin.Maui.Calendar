using NamaskarApp.ViewModels;

namespace NamaskarApp;

public partial class MainPage : ContentPage
{
	public MainPage(CalendarViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
