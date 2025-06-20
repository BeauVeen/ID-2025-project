//using Android.SE.Omapi;
using MatrixMobileApp.API;
using MatrixMobileApp.API.Services;

namespace MatrixMobileApp;

public partial class ManualContainerCodePage : ContentPage
{

    private readonly ManualContainerCodeService manualContainerService;
    public ManualContainerCodePage()
	{
		InitializeComponent();
        var api = new ApiService();
        manualContainerService = new ManualContainerCodeService(api.Client);
    }

    async void OnManualContainerClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        ErrorLabel.Text = string.Empty;

        var containerCode = ManualContainerEntry.Text?.Trim();

        if (string.IsNullOrEmpty(containerCode))
        {
            ShowError("Voer een containernummer in");
            return;
        }

        try
        {
            if (!int.TryParse(containerCode, out int containerId))
            {
                ShowError("Ongeldig containernummer");
                return;
            }

            var container = await manualContainerService.GetContainerById(containerId);

            if (container == null)
            {
                ShowError("Geen container met dit containernummer gevonden");
                return;
            }

            await Navigation.PushAsync(new ContainerPage(container));
        }
        catch (Exception ex)
        {
            ShowError($"Kan container niet laden: {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }
}