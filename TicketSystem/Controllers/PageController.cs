using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Web;
using System;
using System.Text.RegularExpressions;

namespace TicketSystem.Controllers
{
    public class PageController : Controller
    {
        public IActionResult Index(string fileName)
        {
            if (fileName == null)
            {
                return File("index.html", "text/html");
            }

            string fileFormat = Path.GetExtension(fileName);

            if (fileFormat == "")
            {
                fileName += ".html";
                Console.WriteLine(fileName);
            }

            fileFormat = Path.GetExtension(fileName);

            if (System.IO.File.Exists($"wwwroot/{fileName}"))
            {
                string contentType = $"text/{fileFormat[1..]}";

                if (fileFormat == ".jpg")
                    contentType = "image/jpg";

                return File($"{fileName}", contentType);
            }
            else
            {
                if (System.IO.File.Exists($"wwwroot/Files/{fileName}"))
                {
                    string contentType = $"text/{fileFormat[1..]}";

                    if (fileFormat == ".jpg")
                        contentType = "image/jpg";
                    if (fileFormat == ".png")
                        contentType = "image/jpg";

                    return File($"Files/{fileName}", contentType);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        private IResult ReadTextFile(string filePath)
        {
            string fileFormat = Path.GetExtension(filePath)[1..];

            string file = System.IO.File.ReadAllText(filePath);
            return Results.Content(file, $"text/{fileFormat}");
        }

        private IResult ReadImage(string filePath)
        {
            string fileFormat = Path.GetExtension(filePath)[1..];

            var file = System.IO.File.ReadAllBytes(filePath);
            return Results.File(file, $"image/{fileFormat}");
        }
    }
}
