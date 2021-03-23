using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{    

    public enum x18EntryCodeENUM
    {
        Manual = 1,  // 1-vyplňovat ručně kód
        NotUsed = 2, // 2-nepoužívat kód
        AutoX18 = 3, // 3-automaticky generovat v rámci dokumentu
        AutoP41 = 4, // 4-automaticky generovat v rámci projektu
        X38ID = 5     // podle číselné řady
    }


    public enum x18EntryNameENUM
    {
        Manual = 1,    // 1-vyplňovat ručně název    
        NotUsed = 2     // 2-nevyplňovat název
    }


    public enum x18EntryOrdinaryENUM
    {
        Manual = 1,      // 1-pořadí zadávat ručně   
        NotUsed = 2     // 2-nepracovat s pořadím
    }

    

    public enum x18UploadENUM
    {
        NotUsed = 0,
        FileSystemUpload = 1
    }

    public class x18EntityCategory:BaseBO
    {
        public int x23ID { get; set; }  //propojovací klíč mezi x18 a o23
        public int b01ID { get; set; }
        public int j02ID_Owner { get; set; }
        public int x38ID { get; set; }
        public string x18Name { get; set; }
        public string x18NameShort { get; set; }
        public int x18Ordinary { get; set; }       
        public bool x18IsColors { get; set; }
        
        
        public string x18ReportCodes { get; set; }        
        public x18EntryNameENUM x18EntryNameFlag { get; set; } = x18EntryNameENUM.Manual;
        public x18EntryCodeENUM x18EntryCodeFlag { get; set; } = x18EntryCodeENUM.Manual;
        public x18EntryOrdinaryENUM x18EntryOrdinaryFlag { get; set; } = x18EntryOrdinaryENUM.Manual;
        public bool x18IsCalendar { get; set; }
        public string x18CalendarFieldStart { get; set; }
        public string x18CalendarFieldEnd { get; set; }
        public string x18CalendarFieldSubject { get; set; }
        public string x18CalendarResourceField { get; set; }

        public x18UploadENUM x18UploadFlag { get; set; } = x18UploadENUM.NotUsed;
        public int x18MaxOneFileSize { get; set; }
        public string x18AllowedFileExtensions { get; set; }
        public bool x18IsAllowEncryption { get; set; }
        
        

        public bool Is_p41 { get; }        
        public bool Is_p28 { get; }       
        public bool Is_p31 { get; }      
        public bool Is_j02 { get; }       
        public bool Is_o23 { get; }        
        public bool Is_p91 { get; }        
        public bool Is_p56 { get; }      
        public bool Is_o22 { get; }
       
    }
}
