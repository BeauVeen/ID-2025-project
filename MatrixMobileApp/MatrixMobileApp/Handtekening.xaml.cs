namespace MatrixMobileApp;

public partial class Handtekening : ContentPage
{
    private int scannedOrdersCount = 0;

    public Handtekening(int ordersCount = 0)
    {
        InitializeComponent();
        scannedOrdersCount = ordersCount;
        OrdersCountLabel.Text = $"Aantal gescande orders: {scannedOrdersCount}";
    }

    private async void OnAnnuleerClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Annuleren", "Weet je zeker dat je wilt annuleren?", "Ja", "Nee");
        if (confirm)
        {
            await Navigation.PopToRootAsync();
        }
    }

    private async void OnVerstuurClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Verstuurd", "De orders zijn succesvol verzonden.", "OK");
        await Navigation.PopToRootAsync();
    }
}
