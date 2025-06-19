using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace MatrixWebApp.Extensions
{
    public class SKBitmapRenderer : IBarcodeRenderer<SKBitmap>
    {
        public SKBitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
        {
            int width = matrix.Width;
            int height = matrix.Height;

            var bitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            var paint = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Fill
            };

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (matrix[x, y])
                    {
                        canvas.DrawPoint(x, y, paint);
                    }
                }
            }

            return bitmap;
        }

        // Extra overload voor de interface
        public SKBitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
        {
            return Render(matrix, format, content, new EncodingOptions());
        }
    }
}
