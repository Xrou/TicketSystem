using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Models;

namespace TicketSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : Controller
    {
        private readonly Database context;

        public FilesController(Database context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFileCollection uploadedFiles)
        {
            List<Models.File> files = new List<Models.File>();

            foreach (var uploadedFile in uploadedFiles)
            {
                string extension = Path.GetExtension(uploadedFile.FileName);

                string uniqueName = $"{Guid.NewGuid()}{extension}";

                string path = "./wwwroot/Files/" + uniqueName;

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                var file = new Models.File() { Path = uniqueName };
                context.Files.Add(file);
                files.Add(file);
            }

            context.SaveChanges();

            return Created("api/Files", files.ToArray());
        }
    }
}
