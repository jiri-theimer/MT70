using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31invoice
{
    public class p91CreateItem:BO.p91Create
    {
        public int p41ID { get; set; }
        public string p41Name { get; set; }
        public string p28Name { get; set; }
        public string p92Name { get; set; }
    }
}
