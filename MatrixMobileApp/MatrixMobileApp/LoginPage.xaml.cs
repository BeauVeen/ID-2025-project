using System.Text.Json;
using System.Text;
using MatrixMobileApp.API;
using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using Microsoft.Maui.Controls;
using MatrixMobileApp.MagazijnMedewerkerPages;
namespace MatrixMobileApp
{
    public partial class LoginPage : ContentPage
    {
        // Gebruik jwt service voor het login proces
        private readonly JwtService _jwtService;
        // string waarin de onthouden email in opgeslagen wordt
        private const string RememberedEmailKey = "remembered_email";
        // wachtwoord zichtbaarheid wordt geinitialiseerd op false, kan door de user veranderd worden
        private bool _isPasswordVisible = false;

        public LoginPage()
        {
            InitializeComponent();

            var api = new ApiService();
            _jwtService = new JwtService(api.Client);

            // haal de opgeslagen e-mail op als die er is, zodat de gebruiker deze niet handmatig meer hoeft in te vullen
            EmailEntry.Text = Preferences.Get(RememberedEmailKey, string.Empty);
            // plaats cursor direct op wachtwoordveld bij openen van pagina
            PasswordEntry.Focus();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // loading icon bij het inloggen om gebruiker te laten zien dat het systeem op de actie reageert (feedback design principe)
            LoadingIndicator.IsVisible = true; 
            LoadingIndicator.IsRunning = true;

            // disable de login button om meerdere inlogpogingen achter elkaar te voorkomen
            LoginButton.IsEnabled = false;
            // initialiseer de errorlabel op false, wordt later op true gezet als een fout voorgekomen is
            ErrorLabel.IsVisible = false;

            
            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = EmailEntry.Text?.Trim(), // gebruik van Trim() om eventuele whitespace te negeren, en alleen de daadwerkelijke string van karakters op te halen
                    Password = PasswordEntry.Text
                };

                // basisvalidatie
                if (string.IsNullOrWhiteSpace(loginRequest.Email))
                    throw new Exception("E-mailadres is verplicht");
                if (string.IsNullOrWhiteSpace(loginRequest.Password))
                    throw new Exception("Wachtwoord is verplicht");

                // gebruik de AuthenticateAsync functie uit jwtService, die via de api een authenticatie doet (/api/User/authenticate post request) om inloggegevens te valideren
                var response = await _jwtService.AuthenticateAsync(loginRequest);

                // alleen als de jwtservice de inlogpoging valideert, gebeurd onderstaande code pas
                if (response.RoleId != 2 && response.RoleId != 4)
                    throw new Exception("Alleen bezorgers of magazijnmedewerkers kunnen inloggen op deze app");

                // opslaan van gebruikersgegevens
                Preferences.Set("auth_token", response.Token);
                Preferences.Set("user_id", response.UserId);
                Preferences.Set("user_email", response.Email ?? string.Empty);

                // het e-mailadres wordt opgeslagen voor sneller inlogproces
                Preferences.Set(RememberedEmailKey, response.Email ?? string.Empty);

                var userName = response.Email?.Split('@')[0] ?? "Gebruiker";
                Preferences.Set("user_name", userName);
                Preferences.Set("user_role", response.RoleId.ToString());

                // initialiseer nieuwe Apshell die de navigatie en tabbars bevat. er worden absolute routes gebruikt om de route te bepalen ipv appshell hierarchie
                Application.Current.MainPage = new AppShell();

                // Redirect naar de juiste homepage bij succesvolle login

                if (response.RoleId == 2) // RoleId 2 = bezorgers
                {
                    Shell.Current.FindByName<TabBar>("MainTabBar").IsVisible = true; // maak de tabbar zichtbaar voor bezorgers
                    await Shell.Current.GoToAsync("//HomePage");  // gebruik absolute route 
                }

                else if (response.RoleId == 4) // RoleId 4 = magazijnmedewerkers
                {
                    Shell.Current.FindByName<TabBar>("MagazijnTabBar").IsVisible = true; // maak de tabbar zichtbaar voor magazijnmedewerkers
                    await Shell.Current.GoToAsync("//HomePageMagazijn"); // gebruik absolute route
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex}");
                ErrorLabel.Text = ex.Message; // laat de specifieke foutmelding zien
                ErrorLabel.IsVisible = true; // toon foutmelding
            }
            finally
            {
                // reset de loading icons na inlogpoging
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;

                // enable de login knop weer na de inlogpoging
                LoginButton.IsEnabled = true;
            }
        }
        


        public void ResetLoginFields()
        {
            // Reset wachtwoord veld altijd 
            PasswordEntry.Text = string.Empty;

            // Behoud e-mail altijd
            EmailEntry.Text = Preferences.Get(RememberedEmailKey, string.Empty);
        }

        private void TogglePasswordVisibility(object sender, EventArgs e)
        {
            // de huidige boolean waarde van isPasswordVisible wordt omgedraait (true wordt false, false wordt true) voor de toggle functionaliteit
            _isPasswordVisible = !_isPasswordVisible;

            // IsPassword bepaalt of het wachtwoord wordt verborgen (true = verborgen, false = zichtbaar)
            // omdat _isPasswordVisible true betekent dat het wachtwoord zichtbaar moet zijn, zetten we IsPassword op het tegengestelde daarvan
            PasswordEntry.IsPassword = !_isPasswordVisible;

            var image = sender as Image;
            if (image != null)
            {
                image.Source = _isPasswordVisible ? "visible.png" : "notvisible.png"; // het icoon verandert afhankelijk van de zichtbaarheid: als het wachtwoord zichtbaar is, wordt "visible.png" getoond, anders "notvisible.png".

            }
        }
    }
}