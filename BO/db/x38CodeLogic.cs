using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public enum x38EditModeFlagENUM
    {
        NotEditable = 1,
        RecordOwnerOnly = 2,
        AdminOnly = 3
    }

    public class x38CodeLogic:BaseBO
    {
        public x29IdEnum x29ID { get; set; }
        public string x38Name { get; set; }
        public x38EditModeFlagENUM x38EditModeFlag { get; set; } = x38EditModeFlagENUM.AdminOnly;
        public string x38MaskSyntax { get; set; }
        public string x38ConstantBeforeValue { get; set; }
        public string x38ConstantAfterValue { get; set; }
        public int x38Scale { get; set; }
        public bool x38IsDraft { get; set; }
        public string x38Description { get; set; }
        public int x38ExplicitIncrementStart { get; set; }
        public bool x38IsUseDbPID { get; set; }


        public string CodeLogicInfo
        {
            get
            {
                if (this.x38IsUseDbPID)
                    return "";

                if (this.x38MaskSyntax == "")
                    return this.x38ConstantBeforeValue + BO.BAS.RightString("000000001", this.x38Scale) + " - " + this.x38ConstantBeforeValue + BO.BAS.RightString("99999999999", this.x38Scale);
                else
                    return "Specifické pravidlo pro generování kódu záznamu";
            }
        }
    }
}
