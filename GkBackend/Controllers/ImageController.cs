using GkBackend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GkBackend.Controllers
{
    public class ImageController : Controller
    {
        private IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("[controller]/Process")]
        public string InvertImageColors([FromForm] IFormCollection formData)
        {
            var img = formData.Files.First();
            if (!(formData.TryGetValue("operation", out var op)))
            {
                throw new InvalidOperationException("no op specified");
            }

            using var stream = new MemoryStream();
            img.OpenReadStream().CopyTo(stream);

            return op.FirstOrDefault() switch
            {
                "invert" => _imageService.InvertColorsToBase64(stream),
                "grayscale" => _imageService.GrayscaleToBase64(stream),
                "edge" => _imageService.EdgeRecognitionToBase64(stream),
                _ => throw new InvalidOperationException("wrong op specified")
            };
        }

    }
}
