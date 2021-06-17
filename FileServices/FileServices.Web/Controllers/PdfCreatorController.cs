
using DinkToPdf;
using DinkToPdf.Contracts;
using FileServices.Web.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FileServices.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfCreatorController : ControllerBase
    {
        private IConverter _converter;
        private readonly IRazorLightEngine _razorLightEngine;
        public PdfCreatorController(IConverter converter, IRazorLightEngine engine)
        {
            _converter = converter;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            _razorLightEngine = engine;
        }
        [HttpGet]
        public async Task<IActionResult> CreatePDF()
        {
            try
            {
                var model = DataStorage.GetAllEmployees();
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "assets", "templates","PDFTemplate.cshtml");
                string template = await _razorLightEngine.CompileRenderAsync(templatePath, model);


                Directory.CreateDirectory(@"C:\Temp");
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "PDF Report",
                    Out = @"C:\Temp\Employee_Report.pdf"
                };
                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = template,//TemplateGenerator.GetHTMLString(),
                    WebSettings = { DefaultEncoding = "utf-8"
                    , UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles","styles.css") 
                    },
                    HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                    FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
                };
                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };
                var st = DateTime.Now;
                _converter.Convert(pdf);
                var en = DateTime.Now;
                return Ok($"Successfully created PDF document. Begin: {st}. End: {en}");
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
