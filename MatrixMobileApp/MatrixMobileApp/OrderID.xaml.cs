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
                OnPropertyChanged(nameof(Orders));
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

    public bool CanConfirmOrder => ContainerData != null && IsOrderChecked;
    public bool IsOrderCheckedVisible => ContainerData != null;

    public string DisplayContainer => ContainerData == null
        ? "Geen container gevonden"
        : $"Container ID: {ContainerData.ContainerId} - Status: {ContainerData.Status}";

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
