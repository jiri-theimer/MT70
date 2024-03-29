﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.p31approve
{
    public class GatewayViewModel:BaseViewModel
    {
        public string tempguid { get; set; }
        public string prefix { get; set; }
        public string pidsinline { get; set; }
        public BO.p72IdENUM p72id { get; set; }
        
        public int approvinglevel { get; set; }

        public IEnumerable<BO.p31Worksheet> lisP31 { get; set; }

        public bool DoDefaultApproveState { get; set; } = true;


        public TheGridInput gridinput { get; set; }
        public int j72ID { get; set; }

        public int Rec_Pid { get; set; }
        public int Rec_p71ID { get; set; }
        public int Rec_p72ID { get; set; }
        public int Rec_p33ID { get; set; }
        public string Rec_HodinyKFakturaci { get; set; }
        public string Rec_HodinyVPausaulu { get; set; }
        public double Rec_SazbaKFakturaci { get; set; }
        public double Rec_BezDph { get; set; }
        public int Rec_UrovenSchvalovani { get; set; }

        public double Rec_DphSazba { get; set; }

        public string Rec_InterniHodiny { get; set; }

        public string SelectedTab { get; set; }
        public List<NavTab> OverGridTabs { get; set; }
        
        public string P31StateQueryCssClass { get; set; }
        public string P31StateQueryAlias { get; set; }

        public string batchpids { get; set; }
        public double BatchInvoiceRate { get; set; }
        public int BatchApproveLevel { get; set; }


        public int p91id { get; set; }
        public BO.p91Invoice RecP91_Append2Invoice { get; set; }
    }
}
