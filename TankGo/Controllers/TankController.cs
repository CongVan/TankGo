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
        static  List<List<int>> Matrix = new List<List<int>>();
        static  int[,] MaTran = null;
        static  Node P = new Node(), G = new Node();
        static  int RowsMaTran = 0, ColsMaTran = 0;
        // GET: Tank
        public ActionResult Index()
        {
            return View();
        }
  
        public ActionResult LoadMap(string mapid)
        {
            Map = null;
            Map = new List<Cell>();
            string[] lines = System.IO.File.ReadAllLines(HttpContext.Server.MapPath("/layouts/" + mapid + ".txt"));
            int rows=0, cols=0;
            cols = lines[0].Length;
            MaTran = new int[lines.Length, cols];
            RowsMaTran = lines.Length;
            ColsMaTran = cols;
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
                        default:cell.Status = 0;break;
                    }
                    MaTran[rows, i] = cell.Status;
                    if (cell.Status == -1)
                    {
                        P.x = rows;
                        P.y = i;
                        P.Status = cell.Status;
                    }
                    else if (cell.Status == 99)
                    {
                        G.x = rows;
                        G.y = i;
                        G.Status = cell.Status;
                    }


                    Map.Add(cell);
                }
                rows++;
            }
            
            return Json(new {Map=Map,Rows= RowsMaTran, Cols= ColsMaTran },JsonRequestBehavior.AllowGet);
        }

        public static void AddNode(Node A, List<Node> X)
        {
            //Nếu X rổng add vào không kiểm tra 
            if (X.Count() == 0)
            {
                X.Add(A);
                return;
            }
            //kiểm tra A có trong X chưa
            for (int i = 0; i < X.Count(); i++)
            {
                if (A.x == X[i].x && A.y == X[i].y)
                {
                    //nếu A không tối ưu hơn thì dừng 
                    if (A.g + A.h >= X[i].g + X[i].h)
                    {
                        return;
                    }
                    //ngược lại yêu cầu cập nhật lại A
                    else
                    {
                        //xóa X[i] khoi X
                        X.RemoveAt(i);
                        break;
                    }
                }
            }
            // cập nhật X
            for (int i = 0; i < X.Count(); i++)
            {
                //Neu A tối ưu hơn thêm A vào i 
                if (A.g + A.h < X[i].g + X[i].h)
                {
                    X.Insert(i, A);
                    break;
                }
                //nếu không tối ưu nhất thì thêm vào cuối
                else if (i == X.Count() - 1)
                {
                    X.Insert(i + 1, A);
                    break;
                }
            }
        }
        [HttpPost]
        public ActionResult LetGo(List<List<int>> map, int pyStart, int pxStart, int pyEnd, int pxEnd)
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    MaTran[i,j] = map[i][j];
                }
            }
            P = null; G=null;
            P = new Node();
            P.x = pyStart;
            P.y = pxStart;
            G = new Node();
            G.x = pyEnd;
            G.y = pxEnd;
            List < Cell > lst= AStart(MaTran, RowsMaTran, ColsMaTran, P, G);
            if (lst != null)
            {
                return Json(new { Map = lst, ret = 1 },JsonRequestBehavior.AllowGet);
            }
            return Json(new { Map = lst, ret = 0 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ReloadMap(List<List<int>> map)
        {
            return Json(0,JsonRequestBehavior.AllowGet);
        }
        public static Double KCach(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(1.0 * (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
        public static List<Cell> AStart(int[,] MaTran, int d, int c, Node P, Node G)
        {
            
            List<Node> X = new List<Node>();
            List<Node> Luu = new List<Node>();
            X.Add(P);
            Node Kt = new Node();
            Kt.IDparent = -1;
            int ID = 0;
            while (X.Count > 0)
            {
                //lấy pt đầu tiên
                var TempX = X.ElementAt(0);
                TempX.ID = ID;
                Luu.Add(TempX);
                ID++;
               
                //if (ID > Map.Count)
                //{
                //    return null;
                //}
                X.RemoveAt(0);
                //nếu là G thì dừng
                if (TempX.x == G.x && TempX.y == G.y)
                {
                    Kt.x = TempX.x;
                    Kt.y = TempX.y;
                    Kt.g = TempX.g;
                    Kt.h = TempX.h;
                    Kt.ID = TempX.ID;
                    Kt.IDparent = TempX.ID;
                    Kt.Status = TempX.Status;
                    break;
                }
                //duyệt các phía
                //đi lên
                if (TempX.x - 1 >= 0)
                    if (MaTran[TempX.x - 1, TempX.y] != 1)
                    {
                        //cập thông tin
                        Node New = new Node();
                        New.x = TempX.x - 1;
                        New.y = TempX.y;
                        New.g = TempX.g + 1;
                        New.h = KCach(TempX.x - 1, TempX.y, G.x, G.y);
                        New.IDparent = TempX.ID;
                        New.Status = MaTran[TempX.x - 1, TempX.y];
                        //thêm vào list
                        AddNode(New, X);
                    }
                //đi xuống
                if (TempX.x + 1 < d)
                    if (MaTran[TempX.x + 1, TempX.y] != 1)
                    {
                        //cập thông tin
                        Node New = new Node();
                        New.x = TempX.x + 1;
                        New.y = TempX.y;
                        New.g = TempX.g + 1;
                        New.h = KCach(TempX.x + 1, TempX.y, G.x, G.y);
                        New.IDparent = TempX.ID;
                        New.Status = MaTran[TempX.x + 1, TempX.y];
                        //thêm vào list
                        AddNode(New, X);
                    }
                //qua trái
                if (TempX.y - 1 >= 0)
                    if (MaTran[TempX.x, TempX.y - 1] != 1)
                    {
                        //cập thông tin
                        Node New = new Node();
                        New.x = TempX.x;
                        New.y = TempX.y - 1;
                        New.g = TempX.g + 1;
                        New.h = KCach(TempX.x, TempX.y - 1, G.x, G.y);
                        New.IDparent = TempX.ID;
                        New.Status = MaTran[TempX.x, TempX.y - 1];
                        //thêm vào list
                        AddNode(New, X);
                    }
                //qua phải
                if (TempX.y + 1 < c)
                    if (MaTran[TempX.x, TempX.y + 1] != 1)
                    {
                        //cập thông tin
                        Node New = new Node();
                        New.x = TempX.x;
                        New.y = TempX.y + 1;
                        New.g = TempX.g + 1;
                        New.h = KCach(TempX.x, TempX.y + 1, G.x, G.y);
                        New.IDparent = TempX.ID;
                        New.Status = MaTran[TempX.x, TempX.y + 1];
                        //thêm vào list
                        AddNode(New, X);
                    }
            }
            /////Lấy  Cells đường đi
            //bắt đầu từ G
            List<Cell> T = new List<Cell>();
            if (Kt.IDparent == -1)
            {
                return T;
                //không có đường đi
                //Console.WriteLine("Không tìm thất đường đi");
            }
            //nếu có đường đi thì duyệt
            //Thêm G và P vào cell

           
            //Hiển thị ra màn hình
            int index = Kt.ID;
            for (int i = 0; i < Luu.Count(); i++)
            {
                Cell TempT = new Cell();
                TempT.Row = Luu[index].x;
                TempT.Col = Luu[index].y;
                TempT.Status = Luu[index].Status;
                T.Insert(0, TempT);
                //  lưu vào ma trận để xem
                //MaTran[TempT.Row, TempT.Row] = Luu[index].Status == 0 ? 2 : Luu[index].Status;
                if (Luu[index].x == P.x && Luu[index].y == P.y)
                    break;
                index = Luu[index].IDparent;
            }
            //////
            //for (int i = 0; i < d; i++)
            //{
            //    Console.WriteLine();
            //    for (int j = 0; j < c; j++)
            //    {
            //        Console.Write("{0} ", MaTran[i, j]);
            //    }
            //}
            return T;
        }



    }
}