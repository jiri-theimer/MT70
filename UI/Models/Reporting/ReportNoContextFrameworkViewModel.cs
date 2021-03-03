using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class ReportNoContextFrameworkViewModel:BaseViewModel
    {
        public IEnumerable<BO.x31Report> lisX31 { get; set; }
        

        public int LastJ25ID { get; set; }
    }
}
