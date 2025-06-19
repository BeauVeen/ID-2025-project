using Microsoft.Maui.Networking;

namespace MatrixMobileApp;

public partial class OfflinePage : ContentPage
{
    public OfflinePage()
    {
        InitializeComponent();
    }

    private async void OnRetryClicked(object sender, EventArgs e)
    {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            var authToken = Preferences.Get("auth_token", string.Empty);
            if (!string.IsNullOrEmpty(authToken))
            {

                await Shell.Current.GoToAsync("//HomePage");
            }
            else
            {
                await DisplayAlert("Niet ingelogd", "Je bent niet ingelogd. Log opnieuw in om verder te gaan.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
        else
        {
            await DisplayAlert("Geen internetverbinding", "Er is nog steeds geen netwerkverbinding gedetecteerd. Controleer uw verbinding en probeer het opnieuw.", "OK");
        }
    }

}