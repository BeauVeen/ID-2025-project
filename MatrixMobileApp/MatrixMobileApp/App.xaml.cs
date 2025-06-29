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

            Connectivity.ConnectivityChanged += goToOfflinePageAsync;
        }

        private async void goToOfflinePageAsync(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet)
            {
                var currentPage = Shell.Current.CurrentPage;

                if (currentPage is not OfflinePage && currentPage is not LoginPage)
                {
                    await Shell.Current.GoToAsync(nameof(OfflinePage));
                }
            }
        }

    }
}
    