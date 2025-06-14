using MatrixMobileApp.API;
using MatrixMobileApp.API.Services;
using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;


namespace MatrixMobileApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        private readonly UserService userService; 
        public MainPage()
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();

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
