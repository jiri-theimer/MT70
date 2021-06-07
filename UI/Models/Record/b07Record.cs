using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models.Record
{
    public class b07Record: BaseRecordViewModel
    {
        public BO.b07Comment Rec { get; set; }
        public string UploadGuid { get; set; }
        public int recpid { get; set; }
        public string  recprefix { get; set; }
        public string ObjectAlias { get; set; }

        public List<o27Repeator> lisO27 { get; set; }


        
    }

    public class o27Repeator : BO.o27Attachment
    {
        public bool IsTempDeleted { get; set; }
        public string TempGuid { get; set; }
        public string CssTempDisplay
        {
            get
            {
                if (this.IsTempDeleted)
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
