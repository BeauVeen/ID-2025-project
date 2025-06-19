using MatrixMobileApp.API;
using MatrixMobileApp.API.Services;
using System.Globalization;
using System.Text.RegularExpressions;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace MatrixMobileApp;

public partial class RoutePage : ContentPage
{
    public string OrderName { get; set; }
    public ICommand OptionsCommand { get; set; }
    public ICommand ScanCommand { get; set; }

    public RoutePage()
    {
        InitializeComponent();

        // Dummy data
        OrderName = "Order #1001";
        OptionsCommand = new Command(OnOptionsClicked);
        ScanCommand = new Command(OnScanClicked); // Koppel de scan-knop aan command

        BindingContext = this;
    }

    private async void OnOptionsClicked()
    {
        await DisplayActionSheet("Meer opties", "Annuleer", null, "Afgeleverd", "Probleem melden");
    }

    private async void OnScanClicked()
    {
        // Navigeer naar de OrderIDPage (zorg dat deze pagina bestaat)
        await Navigation.PushAsync(new OrderID());
    }
}
