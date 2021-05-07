using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31view
{
    public class daylineViewModel : BaseViewModel
    {
        public DateTime d0 { get; set; }
        public DateTime d1 { get; set; }
        public DateTime d2 { get; set; }
        public IEnumerable<BO.j02Person> lisJ02 { get; set; }
        
        public IEnumerable<BO.p31Worksheet> lisP31 { get; set; }
        public IEnumerable<BO.p31WorksheetTimelineDay> lisSums { get; set; }
        public string j07IDs { get; set; }
        public string SelectedPositions { get; set; }
        public string j02IDs { get; set; }
        public string SelectedPersons { get; set; }
        public string j11IDs { get; set; }
        public string SelectedTeams { get; set; }

    }
}
