using System;
using MatrixMobileApp.API.Services;
using System.Net.Http;

namespace MatrixMobileApp;

public partial class Handtekening : ContentPage
{
    private readonly ContainerService _containerService;
    private int scannedOrdersCount = 0;

    // Constructor: initialiseert ContainerService en toont aantal gescande orders
    public Handtekening()
    {
        InitializeComponent();

        var httpClient = new HttpClient { BaseAddress = new Uri("http://20.86.128.95") };
        _containerService = new ContainerService(httpClient);

        scannedOrdersCount = AppData.OrdersCount;
        OrdersCountLabel.Text = $"Aantal gescande orders: {scannedOrdersCount}";
    }

    // Handler voor annuleren knop, vraagt om bevestiging en gaat terug naar root pagina
    private async void OnAnnuleerClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Annuleren", "Weet je zeker dat je wilt annuleren?", "Ja", "Nee");
        if (confirm)
        {
            await Navigation.PopToRootAsync();
        }
    }

    // Handler voor verstuur knop, zet status op 'Afgeleverd' en toont bevestiging
    private async void OnVerstuurClicked(object sender, EventArgs e)
    {
        if (AppData.ContainerId.HasValue)
        {
            try
            {
                await _containerService.PatchContainerStatusAsync(AppData.ContainerId.Value, "Afgeleverd");
                await DisplayAlert("Verstuurd", "De orders zijn succesvol geleverd.", "OK");
                await DisplayAlert("Info", "De bevestiging van de bezorging wordt per e-mail verstuurd.", "OK");
                MessagingCenter.Send(this, "NextContainer");
                await Navigation.PopToRootAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Status kon niet worden bijgewerkt:\n{ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Fout", "Geen geldig container ID gevonden.", "OK");
        }
    }
}
