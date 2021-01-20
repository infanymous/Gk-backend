using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GkBackend.Services
{
    public interface IImageService
    {
        public string InvertColorsToBase64(byte[] imgArray);
    }

    public class ImageService : IImageService
    {
        public string InvertColorsToBase64(byte[] imgArray)
        {
            try
            {
            string base64Result;
            using (var imgStream = new MemoryStream(imgArray))
            {
                using (var resultStream = new MemoryStream())
                {
                    var imgBitmap = new Bitmap(imgStream);
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
                    base64Result = Convert.ToBase64String(resultStream.GetBuffer());
                }
            }
            return base64Result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GrayscaleToBase64(byte[] imgArray)
        {
            try
            {
                string base64Result;
                using (var imgStream = new MemoryStream(imgArray))
                {
                    using (var resultStream = new MemoryStream())
                    {
                        var imgBitmap = new Bitmap(imgStream);
                        for (int i = 0; i < imgBitmap.Width; i++)
                        {
                            for (int j = 0; j < imgBitmap.Height; j++)
                            {
                                Color p = imgBitmap.GetPixel(i, j);
                                int a = p.A, r = p.R, g = p.G, b = p.B;
                                int grayScale = (int)((r * 0.3) + (g * 0.59) + (b * 0.11));
                                
                                Color newColor = Color.FromArgb(a, grayScale, grayScale, grayScale);
                                imgBitmap.SetPixel(i, j, newColor);
                            }
                        }

                        imgBitmap.Save(resultStream, System.Drawing.Imaging.ImageFormat.Png);
                        base64Result = Convert.ToBase64String(resultStream.GetBuffer());
                    }
                }
                return base64Result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
