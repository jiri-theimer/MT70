using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class SelectDocTypeViewModel: BaseViewModel
    {
        public IEnumerable<BO.x18EntityCategory> lisX18 { get; set; }

        public int SelectedPid { get; set; }
    }
}
