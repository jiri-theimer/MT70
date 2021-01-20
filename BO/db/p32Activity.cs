using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public enum p32AttendanceFlagENUM
    {
        _None = 0,
        HoursOnly = 1,
        TimeInterval = 2,
        PrestavkaVPracovnidobe = 11
    }


    public enum p32SystemFlagENUM
    {
        _None = 0,
        _CreditNote = 1
    }


    public class p32Activity : BaseBO
    {
        public p32SystemFlagENUM p32SystemFlag { get; set; } = p32SystemFlagENUM._None;
        public int p34ID { get; set; }
        public int p95ID { get; set; }
        public int p35ID { get; set; }
        public x15IdEnum x15ID { get; set; } = x15IdEnum.Nic;
        public int p38ID { get; set; }

        public string p32Name { get; set; }
        public string p32Code { get; set; }
        public bool p32IsBillable { get; set; }
        public bool p32IsTextRequired { get; set; }
        public int p32Ordinary { get; set; }
        public string p32Color { get; set; }
        public double p32Value_Default { get; set; }
        public double p32Value_Minimum { get; set; }
        public double p32Value_Maximum { get; set; }
        public string p32DefaultWorksheetText { get; set; }
        public string p32HelpText { get; set; }

        public string p32Name_EntryLang1 { get; set; }
        public string p32Name_EntryLang2 { get; set; }
        public string p32Name_EntryLang3 { get; set; }
        public string p32Name_EntryLang4 { get; set; }
        public string p32Name_BillingLang1 { get; set; }
        public string p32Name_BillingLang2 { get; set; }
        public string p32Name_BillingLang3 { get; set; }
        public string p32Name_BillingLang4 { get; set; }

        public string p32DefaultWorksheetText_Lang1 { get; set; }
        public string p32DefaultWorksheetText_Lang2 { get; set; }
        public string p32DefaultWorksheetText_Lang3 { get; set; }
        public string p32DefaultWorksheetText_Lang4 { get; set; }

        public string p32FreeText01 { get; set; }
        public string p32FreeText02 { get; set; }
        public string p32FreeText03 { get; set; }

        public bool p32IsSystemDefault { get; set; }

        public string p32ExternalPID { get; set; }
        public p32AttendanceFlagENUM p32AttendanceFlag { get; set; } = p32AttendanceFlagENUM._None;
        public int p32ManualFeeFlag { get; set; }
        public double p32ManualFeeDefAmount { get; set; }
        public double p32MarginHidden { get; set; }   // procento skryté přirážky
        public double p32MarginTransparent { get; set; }  // procento přiznané přirážky

        public string p32AccountingIDS { get; set; }  // kód předkontace v účetním IS
        public string p32ActivityIDS { get; set; }    // kód činnosti v účetním IS

        public bool p32IsSupplier { get; set; }    // v peněžním úkonu nabízet vazbu na dodavatele
        public bool p32IsCP { get; set; }          // v úkonu nabízet adresáta zásilky české pošty

        private string p34Name { get; }
        
        private int p33ID { get; }
        
        private BO.p34IncomeStatementFlagENUM p34IncomeStatementFlag { get; }
        

        public string NameWithSheet
        {
            get
            {
                return p32Name + " (" + p34Name + ")";
            }
        }
        public string CodeWithName
        {
            get
            {
                if (p32Code == "")
                    return p32Name;
                else
                    return p32Code + " - " + p32Name;
            }
        }
        public string NameWithCode
        {
            get
            {
                if (p32Code == "")
                    return p32Name;
                else
                    return p32Name + " (" + p32Code + ")";
            }
        }

        private string p95Name { get; }
       
        public string p38Name { get; }

        public int p38Ordinary { get; }


        private string x15Name { get; }
        
    }


}
