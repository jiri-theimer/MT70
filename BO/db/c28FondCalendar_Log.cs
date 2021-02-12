using System;

namespace BO
{
    public class c28FondCalendar_Log
    {
        public int c21ID { get; set; }
        public int c21ID_Log { get; set; }
        public DateTime? c28ValidFrom { get; set; }
        public DateTime? c28ValidUntil { get; set; }

        public string c21Name { get; set; }


        public bool IsTempDeleted { get; set; }
        public string TempGuid { get; set; }
        public string CssTempDisplay
        {
            get
            {
                if (this.IsTempDeleted == true)
                {
                    return "display:none;";
                }
                else
                {
                    return "display:table-row;";
                }
            }
        }

    }
}
