using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TankGo.Models
{
    public class Node
    {
        public int ID { get; set; }
        //toa do diem
        public int x { get; set; }
        public int y { get; set; }

        //luu vi tri dinh truoc
        public int IDparent { get; set; }
        //luu gia tri
        public int Status { get; set; }
        //luu uoc luong
        public Double h { get; set; }
        //lưu chi phi so buoc di hien tai
        public Double g { get; set; }
    }
}