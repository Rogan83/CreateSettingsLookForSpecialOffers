using CreateSettingsLookForSpecialOffers.MVVM.ViewModels;

namespace CreateSettingsLookForSpecialOffers.MVVM.Views;

public partial class Settings : ContentPage
{
	public Settings()
	{
		InitializeComponent();
		BindingContext = new SettingsModel();
	}
}