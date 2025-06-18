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

                Application.Current.MainPage = new AppShell();
            }
            else
            {
                await DisplayAlert("Niet ingelogd", "Je bent niet ingelogd. Log opnieuw in om verder te gaan.", "OK");
                Application.Current.MainPage = new LoginPage();
            }
        }
        else
        {
            await DisplayAlert("Nog steeds offline", "Er is nog steeds geen internetverbinding.", "OK");
        }
    }
}