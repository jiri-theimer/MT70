using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p91oper
{
    public class isdocViewModel:BaseViewModel
    {
        public string p91ids { get; set; }
        public string tempsubfolder { get; set; }
        public IEnumerable<BO.p91Invoice> lisP91 { get; set; }
        public List<string> FileNames { get; set; }

    }
}
