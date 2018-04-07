using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TankGo.Controllers
{
    public class TankController : Controller
    {
        // GET: Tank
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult LoadMap(string mapid)
        {
            string[] lines = System.IO.File.ReadAllLines(HttpContext.Server.MapPath("/layouts/" + mapid + ".txt"));

            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                Console.WriteLine("\t" + line);
            }
            
            return Json(lines.ToList(),JsonRequestBehavior.AllowGet);
        }
    }
}