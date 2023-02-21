using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestApp.Models;

namespace TestApp.Controllers
{
    public class HomeController : Controller
    {
        
        private SAMPLEContext db = new SAMPLEContext();
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public HomeController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public FileResult Generate()
        {
            FastReport.Utils.Config.WebMode = true;
            FastReport.Report rep = new FastReport.Report();
            string webRootPath = _hostingEnvironment.WebRootPath;
            string contentRootPath = _hostingEnvironment.ContentRootPath;

            string path = "";
            path = Path.Combine(contentRootPath, "TestReport.frx");

            //for .net 4.6.1 etc
            //string path = Server.MapPath("~/TestReport.frx");

            rep.Load(path);
            List<Customer> Customers = new List<Customer>();
            Customers = db.Customers.ToList();

            rep.RegisterData(Customers, "Customers");
            if (rep.Report.Prepare())
            {
                FastReport.Export.PdfSimple.PDFSimpleExport pdfexport = new FastReport.Export.PdfSimple.PDFSimpleExport();
                pdfexport.ShowProgress = true;
                pdfexport.Subject = "";
                pdfexport.Title = "New Test Report";
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                rep.Report.Export(pdfexport, ms);
                rep.Dispose();
                ms.Position = 0;
                return File(ms, "application/pdf", "myReport.pdf");
            }
            else
            {
                return null;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

