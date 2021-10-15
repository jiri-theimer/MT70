using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p41
{
    public class SelectProjectViewModel:BaseViewModel
    {
        public string source_prefix { get; set; }
        public int source_pid { get; set; }

        public IEnumerable<BO.p41Project> lisP41 { get; set; }
        public int SelectedPid { get; set; }
        public int leindex { get; set; }
    }
}
