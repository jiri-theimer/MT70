﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public class ThePeriod
    {
        public int pid { get; set; }        
        public string PeriodName { get; set; }
        public string PeriodInterval { get; set; }
        public DateTime? d1 { get; set; }
        public DateTime? d2 { get; set; }

        public bool IsUserPeriod { get; set; }

        public string Header
        {
            get
            {
                if (string.IsNullOrEmpty(this.PeriodInterval))
                {
                    return this.PeriodName;
                }
                else
                {
                    return this.PeriodName + ": " + this.PeriodInterval;
                }
               
            }
        }
    }
}
