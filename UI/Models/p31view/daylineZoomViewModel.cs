using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31view
{
    public class daylineZoomViewModel:BaseViewModel
    {
        public DateTime SelectedDate1 { get; set; }
        public DateTime SelectedDate2 { get; set; }
        public int j02ID { get; set; }
        public BO.j02Person RecJ02 { get; set; }
        public TheGridInput gridinput { get; set; }
    }
}
