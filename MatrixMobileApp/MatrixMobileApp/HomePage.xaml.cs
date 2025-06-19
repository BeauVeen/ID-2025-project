using MatrixMobileApp.API;
using MatrixMobileApp.API.Services;
using System.Globalization;
using System.Text.RegularExpressions;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;


namespace MatrixMobileApp
{
    public partial class HomePage : ContentPage
    {

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

        private async void OnInfoTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Hoe werkt het?",
                "1. Scan de QR-code van de container\n\n" +
                "2. Het systeem bevestigt dat dit uw toegewezen container is\n\n" +
                "3. Alle orders in deze container worden gemarkeerd als 'Onderweg' en zijn terug te vinden op de Actieve Orders pagina\n\n" +
                "4. U ontvangt direct uw bezorgroute op de Route pagina.",
                "Begrepen");
        }

        private async void OnAfgeleverdCardTapped(object sender, EventArgs e) // Redirect naar de Afgeleverde Orders Details pagina
        {
            await Navigation.PushAsync(new DetailsPages.AfgeleverdeOrdersDetailsPage());
        }

        private async void OnTeBezorgenCardTapped(object sender, EventArgs e) // Redirect naar de Te Bezorging Details pagina
        {
            await Navigation.PushAsync(new DetailsPages.TeBezorgenDetailsPage());
        }

        private async void OnProblemenCardTapped(object sender, EventArgs e) // Redirect naar de Probleem Details pagina
        {
            await Navigation.PushAsync(new DetailsPages.ProblemenDetailsPage());
        }

        async void BarcodesDetected(object sender, BarcodeDetectionEventArgs e) 
        {
            var first = e.Results?.FirstOrDefault();
            if (first is null) return;


            try { Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200)); } // Vibreer de telefoon bij het succesvol scannen van een barcode
            catch { }

            // 2. Bevestig dat het de juiste container is
            var isCorrectContainer = await DisplayAlert("Container Scan",
                $"Container {first.Value} gescand. Is dit uw toegewezen container?",
                "Ja", "Nee");

            if (isCorrectContainer)
            {
                // Container logica hier nog toevoegen
                // zoals alle orders van de container toevoegen aan actieve orders pagina
                // Orders moeten gemarkeerd worden als 'Onderweg' wanneer bezorger container scant en op 'Begin rit' klikt op de Route pagina (Begin Rit 2.2.0 design in Trello)
                // Er moet een optimale route voor deze orders gegenereerd worden op de Route pagina

            }

            else 
            {
                // return als de container niet correct is
                return;
            }
        }

        private async Task RequestCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Warning", "Camera access is required", "OK");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await RequestCameraPermission();

            CameraReset();
            //cameraView.IsDetecting = true;

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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            cameraView.IsDetecting = false; // Zorgt ervoor dat de camera stopt met een code detecteren wanneer de pagina niet zichtbaar is
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


        // deze functie is nodig om de camera werkend te houden bij het navigeren van een TabBar terug naar HomePage (zonder deze functie blijft camera window zwart)
        private async void CameraReset()
        {
            if (cameraView?.CameraLocation == CameraLocation.Rear)
            {
                try
                {
                    // sla huidige staat van de cameraView op
                    var wasDetecting = cameraView.IsDetecting;
                    // zet cameraView uit om te voorkomen dat de camera blijft detecteren tijdens het wisselen van camera
                    cameraView.IsDetecting = false;

                    // zet visibility van camera tijdelijk uit, zodat gebruiker niet merkt dat de cameraview wisselt
                    cameraView.IsVisible = false;
                    cameraView.CameraLocation = CameraLocation.Front;                 

                    cameraView.CameraLocation = CameraLocation.Rear;
                    // zet visibility weer aan nadat de juiste camera wordt gebruikt
                    cameraView.IsVisible = true;

                    // herstel initiele staat
                    cameraView.IsDetecting = wasDetecting;
                }
                catch
                {
                    // fallback naar normale werking
                    cameraView.IsDetecting = true;
                }
            }
            else
            {
                cameraView.IsDetecting = true;
            }
        }
    }
}
