using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Models.Recpage
{
    public class o23RecPage: BaseRecPageViewModel
    {
        public BO.o23Doc Rec { get; set; }
        public IEnumerable<BO.o27Attachment> lisO27 { get; set; }
        public IEnumerable<BO.x16EntityCategory_FieldSetting> lisX16 { get; set; }
    }
}
