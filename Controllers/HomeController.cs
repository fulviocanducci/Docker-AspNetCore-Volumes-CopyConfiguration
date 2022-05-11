using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TestDocker.Models;


namespace TestDocker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
         private IWebHostEnvironment _webHostEnvironment ;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Datas([FromServices]IConfiguration configuration)
        {
            var number = configuration.GetSection("Version").GetValue<int>("Number");
            var status = configuration.GetSection("Version").GetValue<string>("Status");
            return Json(new {
                number,
                status
            });
        }

        public IActionResult Image() 
        { 
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "public");
            string[] items = System.IO.Directory.GetFiles(path);     
            List<string> data = new List<string>(items.Length);
            for(int i = 0; i < items.Length; i++) 
            {
                data.Add(items[i].Split(@"/").LastOrDefault());
            }       
            return View(data);
        }

        [HttpPost]
        public IActionResult Image(IFormFile photo) 
        {
            SaveUpload(photo);
            return RedirectToAction(nameof(Image));
        } 

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        protected string SaveUpload(IFormFile file, string prefix = null)
        {
            if (file is not null)
            {
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "public");
                string nameAndExtension = (prefix ?? "") + Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(path, nameAndExtension);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                return nameAndExtension;
            }
            return null;
        }
    }
}
