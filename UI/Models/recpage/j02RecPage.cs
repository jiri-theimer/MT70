using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Recpage
{
    public class j02RecPage: BaseRecPageViewModel
    {
        
        public bool IsHover { get; set; }
        public BO.j02Person Rec { get; set; }
        public BO.j03User RecJ03 { get; set; }

        public string GetTeams()
        {
            if (this.Rec == null) return null;

            var mq = new BO.myQueryJ11() { j02id = this.Rec.pid };
            var lis = Factory.j11TeamBL.GetList(mq).Where(p => p.j11IsAllPersons == false);
            if (lis.Count() == 0) return null;
            return string.Join(", ", lis.Select(p => p.j11Name));
        }
    }
}
