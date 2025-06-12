using System.Text.Json;
using System.Text;
using MatrixMobileApp.API;
using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using Microsoft.Maui.Controls;

namespace MatrixMobileApp
{
    public partial class LoginPage : ContentPage
    {

        private readonly JwtService _jwtService;

        
        public LoginPage()
        {
            InitializeComponent();

            var api = new ApiService();
            _jwtService = new JwtService(api.Client);

        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            LoginButton.IsEnabled = false;
            ErrorLabel.IsVisible = false;

            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = EmailEntry.Text?.Trim(),
                    Password = PasswordEntry.Text
                };

                // Validatie
                if (string.IsNullOrWhiteSpace(loginRequest.Email))
                    throw new Exception("E-mailadres is verplicht");
                if (string.IsNullOrWhiteSpace(loginRequest.Password))
                    throw new Exception("Wachtwoord is verplicht");


                var response = await _jwtService.AuthenticateAsync(loginRequest);


                if (response == null)
                    throw new Exception("Geen response van server");


                if (string.IsNullOrEmpty(response.Token))
                    throw new Exception("Inloggen mislukt, geen token ontvangen");


                if (response.RoleId != 2)
                    throw new Exception("Alleen bezorgers kunnen inloggen op deze app");



                Preferences.Set("auth_token", response.Token);
                Preferences.Set("user_id", response.UserId.ToString());
                Preferences.Set("user_email", response.Email ?? string.Empty);


                // Alternatief voor gebruikersnaam: gebruik eerste deel van email
                var userName = response.Email?.Split('@')[0] ?? "Bezorger";
                Preferences.Set("user_name", userName);
                Preferences.Set("user_role", response.RoleId.ToString());

                await Shell.Current.GoToAsync("//HomePage");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex}");
                ErrorLabel.Text = $"Fout bij inloggen: {ex.Message}";
                ErrorLabel.IsVisible = true;
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                LoginButton.IsEnabled = true;
            }
        }


    }
}