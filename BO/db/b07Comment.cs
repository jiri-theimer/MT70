using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class b07Comment:BaseBO
    {
        public BO.x29IdEnum x29ID { get; set; }
        public int b07RecordPID { get; set; }
        public int j02ID_Owner { get; set; }
        public string b07Value { get; set; }
        public string b07WorkflowInfo { get; set; }
        public string b07LinkUrl { get; set; }
        public string b07LinkName { get; set; }
        public DateTime? b07Date { get; set; }
        public string b07ExternalPID { get; set; }
        public int b07ID_Parent { get; set; }
        public DateTime? b07ReminderDate { get; set; }


        private string Author { get; }
        
        
       
        
    }
}
