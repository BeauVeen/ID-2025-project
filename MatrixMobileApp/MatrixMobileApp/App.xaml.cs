using Microsoft.Maui.Networking;

namespace MatrixMobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet)
            {
                // Toon de OfflinePage als er geen internet is
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    // Controleer of we niet al op de OfflinePage zitten
                    if (Current.MainPage is not OfflinePage)
                    {
                        Current.MainPage = new OfflinePage();
                    }
                });
            }
            else
            {
                // hier hoeft even niks. laat gebruiker zelf op Retry klikken op OfflinePage
            }
        }
    }
}
