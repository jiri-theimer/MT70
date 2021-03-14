using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class SendMailViewModel:BaseViewModel
    {
        public BO.x40MailQueue Rec { get; set; }
        public string SelectedO40Name { get; set; }
        public int SelectedJ61ID { get; set; }
        public string SelectedJ61Name { get; set; }
        
        public string UploadGuid { get; set; }
        
        public int ActiveTabIndex { get; set; } = 1;

       
        public bool IsTest { get; set; }
    }
}
