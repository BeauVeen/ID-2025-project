using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using ZXing.QrCode;
using ZXing.SkiaSharp;
using MatrixWebApp.Extensions;

namespace MatrixWebApp.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class BarcodeCreatorModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnGetGenerateBarcode(string productCode)
        {
            if (string.IsNullOrWhiteSpace(productCode))
                return BadRequest("Geen productcode opgegeven.");

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,
                    Height = 100,
                    Margin = 2
                },
                Renderer = new SKBitmapRenderer() // SkiaSharp renderer
            };

            using var bitmap = writer.Write(productCode);
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return File(data.ToArray(), "image/png");
        }
    }
}
