namespace MatrixMobileApp;

public partial class KlantAanwezig : ContentPage
{
    public KlantAanwezig()
    {
        InitializeComponent();
    }

    private async void OnKlantAanwezigClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Klant is aanwezig", "OK");
        await Navigation.PushAsync(new Handtekening());
    }


    private async void OnKlantNietAanwezigClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Plaats producten terug in voertuig", "Bevestiging", "OK");
        // Hier kun je verdere acties toevoegen voor als klant niet aanwezig is
    }
}
