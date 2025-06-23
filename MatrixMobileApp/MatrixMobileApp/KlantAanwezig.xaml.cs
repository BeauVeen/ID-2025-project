using System;
using MatrixMobileApp.API.Services;
using System.Net.Http;

namespace MatrixMobileApp;

public partial class KlantAanwezig : ContentPage
{
    private readonly ContainerService _containerService;

    // Constructor, initializeert ContainerService met HTTP client
    public KlantAanwezig()
    {
        InitializeComponent();

        var httpClient = new HttpClient { BaseAddress = new Uri("http://20.86.128.95") };
        _containerService = new ContainerService(httpClient);
    }

    // Handler voor knop 'Klant Aanwezig', status wordt op 'Afgeleverd' gezet
    private async void OnKlantAanwezigClicked(object sender, EventArgs e)
    {
        if (AppData.ContainerId.HasValue)
        {
            try
            {
                await _containerService.PatchContainerStatusAsync(AppData.ContainerId.Value, "Afgeleverd");
                await DisplayAlert("Info", "Klant is aanwezig, status bijgewerkt.", "OK");
                await Navigation.PushAsync(new Handtekening());
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

    // Handler voor knop 'Klant Niet Aanwezig', status wordt op 'Niet aanwezig' gezet
    private async void OnKlantNietAanwezigClicked(object sender, EventArgs e)
    {
        if (AppData.ContainerId.HasValue)
        {
            try
            {
                await _containerService.PatchContainerStatusAsync(AppData.ContainerId.Value, "Niet aanwezig");
                await DisplayAlert("Bevestiging", "Plaats producten terug in voertuig", "OK");
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
