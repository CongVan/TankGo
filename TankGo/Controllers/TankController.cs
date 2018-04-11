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
        List<Cell> Map = new List<Cell>();
        List<List<int>> Matrix = new List<List<int>>();
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
            GeneratorMatrixFromMaze();
            return Json(new {Map=Map,Rows=rows,Cols=cols },JsonRequestBehavior.AllowGet);
        }

        public void GeneratorMatrixFromMaze()
        {
            for (int i = 0; i < Map.Count; i++)
            {
                List<int> Temp = new List<int>();
                for (int j = 0; j < Map.Count; j++)
                {
                    Temp.Add(-1);    // Tại điểm = -1 => Ko thể đi tới	
                }
                Matrix.Add(Temp);
            }

            for (int i = 0; i < Map.Count; i++)
            {
                for (int j = 0; j < Map.Count; j++)
                {
                    //Tọa độ cố định
                    int x1 = Map[i].Col; // mygraph.GetArrDinhN(i).GetX();
                    int y1 = Map[i].Row; // mygraph.GetArrDinhN(i).GetY();

                    //Lấy tọa độ điểm cần tới
                    int x2 = Map[j].Col; // mygraph.GetArrDinhN(j).GetX();
                    int y2 = Map[j].Row; // mygraph.GetArrDinhN(j).GetY();

                    if ((x1 + 1 == x2 && y1 == y2) || (x1 - 1 == x2 && y1 == y2) ||
                        (x1 == x2 && y1 + 1 == y2) || (x1 == x2 && y1 - 1 == y2))
                    {
                        
                        Matrix[i][j] = 1;
                    }
                }
            }


        }
    }
}