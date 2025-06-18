using MatrixMobileApp.API;
using MatrixMobileApp.API.Services;
using System.Globalization;

namespace MatrixMobileApp.MagazijnMedewerkerPages
{
    public partial class HomePageMagazijn : ContentPage
    {
        private readonly UserService userService;

        public HomePageMagazijn()
        {
            InitializeComponent();
            var api = new ApiService();
            userService = new UserService(api.Client);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Show current date
            var culture = new CultureInfo("nl-NL");
            var date = DateTime.Now.ToString("dddd dd MMMM yyyy", culture);
            DashboardDateLabel.Text = char.ToUpper(date[0], culture) + date.Substring(1);

            // Show user name
            try
            {
                var users = await userService.GetUsersAsync();
                var userEmail = Preferences.Get("user_email", string.Empty);
                var user = users.FirstOrDefault(u => u.Email == userEmail);

                if (user != null)
                {
                    UserNameLabel.Text = $"Welkom, {user.Name}!";
                }
                else
                {
                    UserNameLabel.Text = "Gebruiker niet gevonden";
                }
            }
            catch
            {
                UserNameLabel.Text = "Error loading user";
            }
        }

        private async void OnViewContainersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ContainersPage());
        }

        private async void OnViewProductsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductsPage());
        }

        private async void OnViewOrdersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ActiveOrdersPage());
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Remove("auth_token");
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}