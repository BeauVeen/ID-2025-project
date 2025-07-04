using MatrixMobileApp.API.Models;
using MatrixMobileApp.API.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace MatrixMobileApp;

public partial class RoutePage : ContentPage
{
    private List<Container> containers = new();
    private int currentContainerIndex = 0;

    // Property voor de weergave van de eerste container id/status in de UI
    private string firstContainerId = "Laden...";
    public string FirstContainerId
    {
        get => firstContainerId;
        set
        {
            if (firstContainerId != value)
            {
                firstContainerId = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand OptionsCommand { get; }
    public ICommand ScanCommand { get; }

    // Constructor, initialisatie van commands en event subscriptions
    public RoutePage()
    {
        InitializeComponent();

        OptionsCommand = new Command(OnOptionsClicked);
        ScanCommand = new Command(OnScanClicked);

        BindingContext = this;

        // Luisteren naar berichten om naar volgende container te gaan
        MessagingCenter.Subscribe<Handtekening>(this, "NextContainer", (sender) =>
        {
            Device.BeginInvokeOnMainThread(ShowNextContainer);
        });

        MessagingCenter.Subscribe<KlantAanwezig>(this, "NextContainer", (sender) =>
        {
            Device.BeginInvokeOnMainThread(ShowNextContainer);
        });

        // Containers ophalen bij het laden van de pagina
        _ = LoadContainersAsync();
    }

    // Laad alle containers van de API
    private async Task LoadContainersAsync()
    {
        try
        {
            using HttpClient client = new()
            {
                BaseAddress = new Uri("http://20.86.128.95")
            };
            var containerService = new ContainerService(client);

            containers = await containerService.GetContainersAsync();

            if (containers == null || containers.Count == 0)
            {
                FirstContainerId = "Geen containers beschikbaar";
            }
            else
            {
                containers.Sort((a, b) => a.ContainerId.CompareTo(b.ContainerId));
                currentContainerIndex = 0;
                FirstContainerId = $"Bestelling #{containers[currentContainerIndex].ContainerId}";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Fout bij laden containers: {ex}");
            FirstContainerId = "Fout bij laden containers";
        }
    }

    // Laat de volgende container zien in de lijst
    private void ShowNextContainer()
    {
        if (containers == null || containers.Count == 0)
        {
            FirstContainerId = "Geen containers beschikbaar";
            return;
        }

        currentContainerIndex++;

        if (currentContainerIndex >= containers.Count)
        {
            FirstContainerId = "Alle bestellingen afgehandeld";
            return;
        }

        FirstContainerId = $"Bestelling #{containers[currentContainerIndex].ContainerId}";
    }

    // Handler voor opties-knop (bijvoorbeeld status wijzigen)
    private async void OnOptionsClicked()
    {
        await DisplayActionSheet("Meer opties", "Annuleer", null, "Afgeleverd", "Probleem melden");
    }

    // Handler voor scan-knop, opent de scan pagina
    private async void OnScanClicked()
    {
        await Navigation.PushAsync(new OrderID());
    }
}
