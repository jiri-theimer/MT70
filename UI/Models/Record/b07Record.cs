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
        public string  prefix { get; set; }
        public string ObjectAlias { get; set; }
    }
}
