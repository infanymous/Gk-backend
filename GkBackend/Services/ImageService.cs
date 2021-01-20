using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GkBackend.Services
{
    public interface IImageService
    {
        //public string InvertColorsToBase64(byte[] imgArray);
        //public string GrayscaleToBase64(byte[] imgArray);
        public string InvertColorsToBase64(Stream img);
        public string GrayscaleToBase64(Stream img);
        public string EdgeRecognitionToBase64(Stream img);
    }


    public class ImageService : IImageService
    {
        private readonly int[,] _xMatrix =
        {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
        };

        private readonly int[,] _yMatrix =
        {
            {  1,  2,  1 },
            {  0,  0,  0 },
            { -1, -2, -1 }
        };

        public string InvertColorsToBase64(Stream img)
        {
            try
            {
                using var resultStream = new MemoryStream();
                var imgBitmap = new Bitmap(img);
                for (int i = 0; i < imgBitmap.Width; i++)
                {
                    for (int j = 0; j < imgBitmap.Height; j++)
                    {
                        Color p = imgBitmap.GetPixel(i, j);
                        int a = p.A, r = p.R, g = p.G, b = p.B;

                        imgBitmap.SetPixel(i, j, Color.FromArgb(a, 255 - r, 255 - g, 255 - b));
                    }
                }

                imgBitmap.Save(resultStream, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(resultStream.GetBuffer());

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string GrayscaleToBase64(Stream img)
        {
            try
            {
                using var resultStream = new MemoryStream();
                var imgBitmap = new Bitmap(img);
                ConvertToGrayScale(imgBitmap);
                imgBitmap.Save(resultStream, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(resultStream.GetBuffer());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public string EdgeRecognitionToBase64(Stream img)
        {
            try
            {
                using var resultStream = new MemoryStream();
                var imgBitmap = new Bitmap(img);

                var result = RecognizeEdges(imgBitmap);

                result.Save(resultStream, ImageFormat.Png);
                return Convert.ToBase64String(resultStream.GetBuffer());
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        private Bitmap RecognizeEdges(Bitmap imgBitmap)
        {
            double xColor, yColor, totalColor;
            xColor = yColor = 0.0;

            ConvertToGrayScale(imgBitmap);
            var resultBitmap = new Bitmap(imgBitmap.Width, imgBitmap.Height);

            for (int i = 1; i < imgBitmap.Height - 1; i++)
            {
                for (int j = 1; j < imgBitmap.Width - 1; j++)
                {
                    for (int filterY = -1; filterY <= 1; filterY++)
                    {
                        for (int filterX = -1; filterX <= 1; filterX++)
                        {
                            Color pixel = imgBitmap.GetPixel(j + filterX, i + filterY);
                            var coeffX = _xMatrix[filterY + 1, filterX + 1];
                            var coeffY = _yMatrix[filterY + 1, filterX + 1];

                            xColor += (double)pixel.R * coeffX;
                            yColor += (double)pixel.R * coeffY;
                        }
                    }

                    totalColor = Math.Sqrt((xColor * xColor) + (yColor * yColor));
                    if (totalColor > 255) totalColor = 255;

                    resultBitmap.SetPixel(j, i, Color.FromArgb(255, (int)totalColor, (int)totalColor, (int)totalColor));
                    xColor = yColor = 0.0;
                }
            }
            return resultBitmap;
        }


        private void ConvertToGrayScale(Bitmap img)
        {
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color p = img.GetPixel(i, j);
                    int a = p.A, r = p.R, g = p.G, b = p.B;
                    int grayScale = (int)((r * 0.3) + (g * 0.59) + (b * 0.11));

                    Color newColor = Color.FromArgb(a, grayScale, grayScale, grayScale);
                    img.SetPixel(i, j, newColor);
                }
            }
        }
    }
}
