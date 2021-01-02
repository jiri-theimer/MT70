using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class MyMainMenuLinks: BaseViewModel
    {
        public List<MenuItem> lisZdroj { get; set; }
        public List<MenuItem> lisCil { get; set; }
        public string SelectedItems { get; set; }
    }
}
