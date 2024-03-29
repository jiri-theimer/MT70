﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class p31Record: BaseRecordViewModel
    {
        public BO.p31WorksheetEntryInput Rec { get; set; }
        public BO.p31Worksheet RecP31 { get; set; }
        
        public BO.p41Project RecP41 { get; set; }
        public BO.p34ActivityGroup RecP34 { get; set; }
        public BO.p32Activity RecP32 { get; set; }
        public BO.p56Task RecP56 { get; set; }

        public FreeFieldsViewModel ff1 { get; set; }
        
        public int SelectedLevelIndex { get; set; }
        public List<BO.ListItemValue> lisLevelIndex { get; set; }
        public string ProjectEntity { get; set; } = "p41Project";

        public DateTime? p31Date { get; set; }
        public string SelectedComboProject { get; set; }
        public string SelectedComboPerson { get; set; }
        public string SelectedComboP32Name { get; set; }
        public string SelectedComboP34Name { get; set; }
        public string SelectedComboTask { get; set; }
        public string SelectedComboJ27Code { get; set; }
        public string SelectedComboJ19Name { get; set; }
        public string SelectedComboP35Code { get; set; }
        public string SelectedComboSupplier { get; set; }
        public int ShowTaskComboFlag { get; set; }  //0-default, 1-ano, 2-ne

        public int PiecePriceFlag { get; set; }

        public string CasOdDoIntervals { get; set; }

        public UI.Models.p31oper.hesViewModel Setting { get; set; }     //nastavení vykazování
        
        public bool IsValueTrimming { get; set; }   //zapisuje se korekce

        public string GuidApprove { get; set; }
    }
}
