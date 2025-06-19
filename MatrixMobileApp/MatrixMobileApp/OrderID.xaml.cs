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

    public event PropertyChangedEventHandler PropertyChanged;

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
                OnPropertyChanged(nameof(Orders)); // update orders wanneer container wijzigt
            }
        }
    }

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
                OnPropertyChanged(nameof(Orders)); // update orders als checkbox wijzigt
            }
        }
    }

    // Deze property geeft de orders terug, maar alleen als IsOrderChecked true is
    public List<Order> Orders
    {
        get
        {
            if (ContainerData != null && IsOrderChecked)
            {
                // ContainerOrders bevat ContainerOrder-objecten, elk met een Order-property
                var orders = new List<Order>();
                foreach (var containerOrder in ContainerData.ContainerOrders)
                {
                    if (containerOrder.Order != null)
                        orders.Add(containerOrder.Order);
                }
                return orders;
            }
            return new List<Order>(); // lege lijst als niet aangevinkt of container leeg
        }
    }

    public bool CanConfirmOrder => ContainerData != null && IsOrderChecked;

    public bool IsOrderCheckedVisible => ContainerData != null;

    public string DisplayContainer => ContainerData == null
        ? "Geen container gevonden"
        : $"Container ID: {ContainerData.ContainerId} - Status: {ContainerData.Status}";

    public ICommand SearchCommand { get; }

    public OrderID()
    {
        InitializeComponent();
        SearchCommand = new Command(async () => await SearchContainerAsync());
        BindingContext = this;
    }

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
                IsOrderChecked = false; // reset checkbox als nieuwe container geladen is
            }
            else
            {
                await DisplayAlert("Fout", $"API-fout: {response.StatusCode}", "OK");
                ContainerData = null;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", $"Er ging iets mis: {ex.Message}", "OK");
            ContainerData = null;
        }
    }

    private async void OnConfirmOrderClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Succes", "Bestelling compleet!", "OK");
        await Navigation.PushAsync(new KlantAanwezig());
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
