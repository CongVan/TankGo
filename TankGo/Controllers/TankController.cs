using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TankGo.Models;

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
            int rows=0, cols=0;
            cols = lines[0].Length;
            var Map = new List<Cell>();
            foreach (string line in lines)
            {
                
                for (int i = 0;  i < cols; i++)
                {
                    var cell = new Cell();
                    cell.Row = rows;
                    cell.Col = i;
                    switch (line[i])
                    {
                        case '%':cell.Status = 1;break;//vật cản
                        case ' ':cell.Status = 0;break;//đường đi
                        case 'P':cell.Status = -1;break;//bắt đầu
                        case 'G':cell.Status = 99;break;//kết thúc
                        default:
                            cell.Status = 1;
                            break;
                    }
                    Map.Add(cell);
                }
                rows++;
            }
            
            return Json(new {Map=Map,Rows=rows,Cols=cols },JsonRequestBehavior.AllowGet);
        }
    }
}