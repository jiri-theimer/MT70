using System.ComponentModel.DataAnnotations;

namespace BO
{    
    public class j04UserRole : BaseBO
    {       
        public string j04Name { get; set; }

        public int x67ID { get; set; }
        public bool j04IsMenu_Worksheet { get; set; }
        public bool j04IsMenu_Project { get; set; }
        public bool j04IsMenu_Contact { get; set; }
        public bool j04IsMenu_People { get; set; }
        public bool j04IsMenu_Report { get; set; }
        public bool j04IsMenu_Invoice { get; set; }
        public bool j04IsMenu_Proforma { get; set; }
        
        public bool j04IsMenu_Notepad { get; set; }
        public bool j04IsMenu_Inbox { get; set; }  // nové pole verze 6
        
        private string _x67RoleValue { get; set; }
        public int j04MobileStreamEnum { get; set; }  // výčet povolených dat pro mobilní aplikaci

        public string RoleValue
        {
            get
            {
                return _x67RoleValue;
            }
        }

        
    }
}
