using MatrixMobileApp.API;
using MatrixMobileApp.API.Services;
using System.Globalization;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
//using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;


namespace MatrixMobileApp
{
    public partial class HomePage : ContentPage
    {
        int count = 0;

        private readonly UserService userService; 
        public HomePage()
        {
            InitializeComponent();
            var api = new ApiService();
            userService = new UserService(api.Client); 
        }

        private async void OnViewProductsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductsPage());
        }

        private async void OnViewOrdersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ActiveOrdersPage());
        }

        private async void OnScanQrTapped(object sender, EventArgs e)
        { 
            await Navigation.PushAsync(new QRScanPage()); 
        }

        private async void OnInfoTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Hoe werkt het?",
                "1. Scan de QR-code van de container\n\n" +
                "2. Het systeem bevestigt dat dit uw toegewezen container is\n\n" +
                "3. Alle orders in deze container worden gemarkeerd als 'Onderweg' en zijn terug te vinden op de Actieve Orders pagina\n\n" +
                "4. U ontvangt direct uw bezorgroute op de Route pagina.",
                "Begrepen");
        }

        private async void OnAfgeleverdCardTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DetailsPages.AfgeleverdeOrdersDetailsPage());
        }

        private async void OnTeBezorgenCardTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DetailsPages.TeBezorgenDetailsPage());
        }

        private async void OnProblemenCardTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DetailsPages.ProblemenDetailsPage());
        }



        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Laat huidige datum voor dashboard zien 
            var culture = new CultureInfo("nl-NL");
            var date = DateTime.Now.ToString("dddd dd MMMM yyyy", culture);
            DashboardDateLabel.Text = char.ToUpper(date[0], culture) + date.Substring(1);

            var token = Preferences.Get("auth_token", string.Empty);
            var roleId = Preferences.Get("user_role", "0");

            if (string.IsNullOrEmpty(token) || roleId != "2")
            {
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }

            // Haal users op en display de naam van de user als begroeting 
            try
            {
                var users = await userService.GetUsersAsync();
                var userEmail = Preferences.Get("user_email", string.Empty);
                var user = users.FirstOrDefault(u => u.Email == userEmail); // Haal de correcte user op op basis van email 

                if (user != null)
                {
                    UserNameLabel.Text = $"Welkom, {user.Name}!";
                }
                else
                {
                    UserNameLabel.Text = "User niet gevonden";
                }
            }
            catch (Exception ex)
            {
                UserNameLabel.Text = "Error loading user";
  
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Remove("auth_token");
            
            // Navigeer naar login pagina
            await Shell.Current.GoToAsync("//LoginPage");

            if (Shell.Current.CurrentPage is LoginPage loginPage)
            {
                loginPage.ResetLoginFields();
            }
        }
    }


}
