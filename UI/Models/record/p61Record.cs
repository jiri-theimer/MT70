﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p61Record:BaseRecordViewModel
    {
        public BO.p61ActivityCluster Rec { get; set; }
        public string p32IDs { get; set; }

        public int SelectedP32ID { get; set; }
        public string SelectedP32Name { get; set; }
        public int SelectedP34ID { get; set; }
        public string SelectedP34Name { get; set; }

        public TheGridInput gridinput { get; set; }
    }
}
