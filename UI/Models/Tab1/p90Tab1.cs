﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Tab1
{
    public class p90Tab1:BaseTab1ViewModel
    {
        public BO.p90Proforma Rec { get; set; }
        public IEnumerable<BO.p82Proforma_Payment> lisP82 { get; set; }
        public IEnumerable<BO.p99Invoice_Proforma> lisP99 { get; set; }
    }
}
