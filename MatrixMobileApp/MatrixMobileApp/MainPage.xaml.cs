namespace MatrixMobileApp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private async void OnViewProductsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductsPage());
        }

        private async void OnViewOrdersClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ActiveOrdersPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var token = Preferences.Get("auth_token", string.Empty);
            var roleId = Preferences.Get("user_role", "0");

            if (string.IsNullOrEmpty(token) || roleId != "2")
            {
                Shell.Current.GoToAsync("//LoginPage");
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
