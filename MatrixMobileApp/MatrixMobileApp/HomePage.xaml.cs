using MatrixMobileApp.API;
using MatrixMobileApp.API.Models;   
using MatrixMobileApp.API.Services;
using Plugin.SimpleAudioPlayer;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

using static System.Runtime.InteropServices.JavaScript.JSType;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;


namespace MatrixMobileApp
{
    public partial class HomePage : ContentPage
    {
        private readonly ManualContainerCodeService manualContainerService;
        private readonly UserService userService;

        public HomePage()
        {
            InitializeComponent();
            var api = new ApiService();
            userService = new UserService(api.Client);
            manualContainerService = new ManualContainerCodeService(api.Client);
            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var options = new BarcodeReaderOptions
            {
                AutoRotate = true
            };

            cameraView.Options = options;

            await RequestCameraPermission();

            try
            {
                cameraView.IsDetecting = true;

            }
            catch (Exception ex)
            {
                await DisplayAlert("Camera Error", $"Could not start camera: {ex.Message}", "OK");
            }

            CameraReset();

            // Laat huidige datum voor dashboard zien 
            var culture = new CultureInfo("nl-NL");
            var date = DateTime.Now.ToString("dddd dd MMMM", culture); 
            DashboardDateLabel.Text = char.ToUpper(date[0], culture) + date.Substring(1);

            var token = Preferences.Get("auth_token", string.Empty);
            var roleId = Preferences.Get("user_role", "0");

            if (string.IsNullOrEmpty(token) || roleId != "2")
            {
                await Shell.Current.GoToAsync("//LoginPage");
                return;
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


        async void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            var barcode = e.Results?.FirstOrDefault();
            if (barcode is null) return;

            // Stop verdere detectie tijdens verwerking
            cameraView.IsDetecting = false;

            try
            {
                //Vibreer de telefoon bij succesvolle detectie
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));

                //Speel mp3 geluid af bij succesvolle detectie
                var player = CrossSimpleAudioPlayer.Current;
                player.Load("beep.mp3");
                player.Play();


                // Vul de manual entry in en activeer de click handler
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ManualContainerEntry.Text = barcode.Value;
                    OnManualContainerClicked(null, EventArgs.Empty);
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Er ging iets mis: {ex.Message}", "OK");
            }
            finally
            {
                // Herstart detectie na verwerking
                await Task.Delay(1800); 
                cameraView.IsDetecting = true;
            }
        }


        // functie voor manual container code input
        async void OnManualContainerClicked(object sender, EventArgs e)
        {
            ErrorLabel.IsVisible = false;
            ErrorLabel.Text = string.Empty;

            var containerCode = ManualContainerEntry.Text?.Trim();

            if (string.IsNullOrEmpty(containerCode))
            {
                await DisplayAlert("Fout", "Voer een containernummer in", "OK");
                return;
            }

            try
            {
                if (!int.TryParse(containerCode, out int containerId))
                {
                    await DisplayAlert("Fout", "Ongeldig containernummer", "OK");
                    return;
                }

                var container = await manualContainerService.GetContainerById(containerId);

                if (container == null)
                {
                    await DisplayAlert("Fout", "Geen container met dit containernummer gevonden", "OK");
                    return;
                }
                if (container.Status == "Klaar voor verzending")
                {
                    await Navigation.PushAsync(new ContainerPage(container));
                }
                else
                {
                    await DisplayAlert("Fout", "Deze container heeft niet de benodigde status om weergegeven te worden.\n\n" +
                        "Controlleer of de container Klaar voor verzending is", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Kan container niet laden: {ex.Message}", "OK");
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

       


        // Redirect functies

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
                "1. Scan de QR van de container, of voer de code handmatig in\n\n" +
                "2. Klik op 'Bezorgen' op de containerpagina\n\n" +
                "3. U wordt direct doorverwezen naar de Route pagina waar uw bezorgroute staat\n\n" +
                "4. Alle orders in de geselecteerde container worden gemarkeerd als 'Onderweg' en zijn terug te vinden op de Actieve Orders pagina\n\n",
                "Begrepen");
        }

    }
}
