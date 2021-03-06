using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class m62SettingsViewModel:BaseViewModel
    {
        public int SelectedJ27ID { get; set; }
        public string SelectedJ27Code { get; set; }
        public DateTime SelectedDate { get; set; }

        public IEnumerable<BO.j27Currency> lisJ27 { get; set; }
    }
}
