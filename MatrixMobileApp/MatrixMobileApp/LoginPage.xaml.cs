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
        private const string RememberMeKey = "remember_me";
        private const string RememberedEmailKey = "remembered_email";

        public LoginPage()
        {
            InitializeComponent();

            var api = new ApiService();
            _jwtService = new JwtService(api.Client);

            if (Preferences.Get(RememberMeKey, false))
            {
                RememberMeCheckBox.IsChecked = true;
                EmailEntry.Text = Preferences.Get(RememberedEmailKey, string.Empty);

                PasswordEntry.Focus();
            }
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

                // Basisvalidatie
                if (string.IsNullOrWhiteSpace(loginRequest.Email))
                    throw new Exception("E-mailadres is verplicht");
                if (string.IsNullOrWhiteSpace(loginRequest.Password))
                    throw new Exception("Wachtwoord is verplicht");

                var response = await _jwtService.AuthenticateAsync(loginRequest);

                if (response.RoleId != 2)
                    throw new Exception("Alleen bezorgers kunnen inloggen op deze app");

                // Opslaan van gebruikersgegevens
                Preferences.Set("auth_token", response.Token);
                Preferences.Set("user_id", response.UserId.ToString());
                Preferences.Set("user_email", response.Email ?? string.Empty);

                // "Remember e-mail" functionaliteit
                Preferences.Set(RememberMeKey, RememberMeCheckBox.IsChecked);
                if (RememberMeCheckBox.IsChecked)
                {
                    Preferences.Set(RememberedEmailKey, response.Email ?? string.Empty);
                }
                else
                {
                    Preferences.Remove(RememberedEmailKey);
                }

                var userName = response.Email?.Split('@')[0] ?? "Bezorger";
                Preferences.Set("user_name", userName);
                Preferences.Set("user_role", response.RoleId.ToString());

                await Shell.Current.GoToAsync("//HomePage");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex}");
                ErrorLabel.Text = ex.Message; // Toon de specifieke foutmelding
                ErrorLabel.IsVisible = true;
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                LoginButton.IsEnabled = true;
            }
        }

        public void ResetLoginFields()
        {
            // Reset wachtwoord veld altijd
            PasswordEntry.Text = string.Empty;

            // Behoud e-mail alleen als "Remember me" eerder was aangevinkt
            if (!Preferences.Get(RememberMeKey, false))
            {
                EmailEntry.Text = string.Empty;
            }

            RememberMeCheckBox.IsChecked = Preferences.Get(RememberMeKey, false);
        }
    }
}