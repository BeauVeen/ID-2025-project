using MatrixMobileApp.API.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

using Container = MatrixMobileApp.API.Models.Container;

namespace MatrixMobileApp;

public partial class OrderID : ContentPage, INotifyPropertyChanged
{
    private string searchText;
    private Container containerData;
    private bool isOrderChecked;
    private string userName;
    private string userAddress;

    public event PropertyChangedEventHandler PropertyChanged;

    // Gebruiker invoer container ID
    public string SearchText
    {
        get => searchText;
        set
        {
            if (searchText != value)
            {
                searchText = value;
                OnPropertyChanged();
            }
        }
    }

    // Gegevens van de gevonden container
    public Container ContainerData
    {
        get => containerData;
        set
        {
            if (containerData != value)
            {
                containerData = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayContainer));
                OnPropertyChanged(nameof(IsOrderCheckedVisible));
                OnPropertyChanged(nameof(CanConfirmOrder));
                OnPropertyChanged(nameof(Orders));
            }
        }
    }

    // Of de order checkbox is aangevinkt
    public bool IsOrderChecked
    {
        get => isOrderChecked;
        set
        {
            if (isOrderChecked != value)
            {
                isOrderChecked = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanConfirmOrder));
                OnPropertyChanged(nameof(Orders));

                if (isOrderChecked)
                    _ = LoadUserDetailsAsync();
                else
                {
                    UserName = null;
                    UserAddress = null;
                }
            }
        }
    }

    // Lijst van orders in de container
    public List<Order> Orders
    {
        get
        {
            if (ContainerData != null && IsOrderChecked)
            {
                var orders = new List<Order>();
                foreach (var containerOrder in ContainerData.ContainerOrders)
                {
                    if (containerOrder.Order != null)
                        orders.Add(containerOrder.Order);
                }
                return orders;
            }
            return new List<Order>();
        }
    }

    // Of de bevestig knop beschikbaar is
    public bool CanConfirmOrder => ContainerData != null && IsOrderChecked;

    // Of de order sectie zichtbaar is
    public bool IsOrderCheckedVisible => ContainerData != null;

    // Weergavetekst van container
    public string DisplayContainer => ContainerData == null
        ? "Geen container gevonden"
        : $"Container ID: {ContainerData.ContainerId} - Status: {ContainerData.Status}";

    // Naam van gebruiker van de order
    public string UserName
    {
        get => userName;
        set
        {
            if (userName != value)
            {
                userName = value;
                OnPropertyChanged();
            }
        }
    }

    // Adres van gebruiker van de order
    public string UserAddress
    {
        get => userAddress;
        set
        {
            if (userAddress != value)
            {
                userAddress = value;
                OnPropertyChanged();
            }
        }
    }

    // Command om container te zoeken via API
    public ICommand SearchCommand { get; }

    // Constructor: initialisatie, binding en command aanmaken
    public OrderID()
    {
        InitializeComponent();
        SearchCommand = new Command(async () => await SearchContainerAsync());
        BindingContext = this;
    }

    // Zoekt container via API op basis van SearchText
    private async Task SearchContainerAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await DisplayAlert("Fout", "Vul een container ID in", "OK");
            return;
        }

        try
        {
            string apiUrl = $"http://20.86.128.95/api/Container/{SearchText}";
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var container = JsonConvert.DeserializeObject<Container>(json);

                ContainerData = container;
                AppData.ContainerId = container.ContainerId;
                AppData.OrdersCount = container.ContainerOrders?.Count ?? 0;

                IsOrderChecked = false;
            }
            else
            {
                await DisplayAlert("Fout", $"API-fout: {response.StatusCode}", "OK");
                ContainerData = null;
                AppData.ContainerId = null;
                AppData.OrdersCount = 0;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er ging iets mis: {ex.Message}", "OK");
            ContainerData = null;
            AppData.ContainerId = null;
            AppData.OrdersCount = 0;
        }
    }

    // Laadt gebruikersgegevens via API van de eerste order in de container
    private async Task LoadUserDetailsAsync()
    {
        if (Orders.Count > 0 && Orders[0]?.UserId != null)
        {
            try
            {
                string apiUrl = $"http://20.86.128.95/api/User/{Orders[0].UserId}";
                using HttpClient client = new HttpClient();
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(json);

                    UserName = user?.Name;
                    UserAddress = $"{user?.Address}, {user?.Zipcode} {user?.City}";
                }
                else
                {
                    UserName = "Onbekend";
                    UserAddress = "Onbekend";
                }
            }
            catch (Exception ex)
            {
                UserName = "Fout bij ophalen";
                UserAddress = ex.Message;
            }
        }
        else
        {
            UserName = null;
            UserAddress = null;
        }
    }

    // Handler voor bevestig knop: navigeert naar KlantAanwezig pagina
    private async void OnConfirmOrderClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Succes", "Bestelling compleet!", "OK");
        await Navigation.PushAsync(new KlantAanwezig());
    }

    // OnPropertyChanged implementatie voor data binding
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
