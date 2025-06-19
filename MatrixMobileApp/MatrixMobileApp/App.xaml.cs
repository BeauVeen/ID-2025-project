using Microsoft.Maui.Networking;

namespace MatrixMobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(OfflinePage), typeof(OfflinePage));
            MainPage = new AppShell();

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        }

        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {

                    if (Shell.Current.CurrentPage is not OfflinePage && Shell.Current.CurrentPage is not LoginPage)
                    {
                        await Shell.Current.GoToAsync(nameof(OfflinePage));
                    }
                });
            }
        }
    }
}
