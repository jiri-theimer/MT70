using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class x35ParamsViewModel:BaseViewModel
    {
        public string AppName { get; set; }             //název databáze
        //public int j27ID_Invoice { get; set; }
        //public string ComboJ27_Invoice { get; set; }

        public int DefMaturityDays { get; set; }        //výchozí splatnost faktury
        public int j27ID_Domestic { get; set; }
        public string ComboJ27_Domestic { get; set; }
        public int Round2Minutes { get; set; }          //na kolik minut 
        public string Upload_Folder { get; set; }       //file-system upload složka
        public string AppHost { get; set; }             //veřejná URL adresa

        public string COUNTRY_CODE { get; set; }        //domácí stát: CZ/SK

        public bool IsAllowPasswordRecovery { get; set; }   //Povolit uživatelům obnovu zapomenutého hesla
        public string cp_odesilatel { get; set; }       //Česká pošta: Název podavatele
        public string cp_podavatel { get; set; }        //Česká pošta: Název odesílatele

        public string BillingLang1 { get; set; }
        public string BillingLang2 { get; set; }
        public string BillingLang3 { get; set; }
        public string BillingLang4 { get; set; }
        public string BillingIcon1 { get; set; }
        public string BillingIcon2 { get; set; }
        public string BillingIcon3 { get; set; }
        public string BillingIcon4 { get; set; }

        public List<string> LangFlags { get; set; }

    }
}
