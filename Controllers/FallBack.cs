using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace IssueLog.API.Controllers
{
    public class FallBack: Controller
    {
        public IActionResult Index(){
            System.Diagnostics.Debug.WriteLine(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","index.html"));
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot","index.html"),"text/HTML");
        }
    }
}