﻿using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;
using CommunityToolkit.Maui;

namespace MatrixMobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder

                .UseMauiApp<App>()
                .UseBarcodeReader() // Gebruik de ZXing barcode reader (QR scan functionaliteit op de homepage)
                .UseMauiCommunityToolkit(); // Add CommunityToolkit.Maui
            builder.UseMauiCommunityToolkit();
            builder.ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build(); 
        }
    }
}
