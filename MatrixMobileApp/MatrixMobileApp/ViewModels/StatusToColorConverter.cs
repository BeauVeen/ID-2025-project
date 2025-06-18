using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MatrixMobileApp.ViewModels
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "In behandeling" => Color.FromArgb("#FFA500"),   // Orange
                    "Klaar voor bezorging" => Color.FromArgb("#22C55E"), // Green
                    "Onderweg" => Color.FromArgb("#3B82F6"),         // Light blue
                    _ => Color.FromArgb("#2D3A4A")                   // Default
                };
            }
            return Color.FromArgb("#2D3A4A");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
