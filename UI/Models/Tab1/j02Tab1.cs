﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab1
{
    public class j02Tab1 : BaseTab1ViewModel
    {
        public BO.j02Person Rec { get; set; }
        public BO.j02PersonSum RecSum { get; set; }
        public BO.j03User RecJ03 { get; set; }

        public IEnumerable<BO.p30Contact_Person> lisP30 { get; set; }

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
