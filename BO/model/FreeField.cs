using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class FreeField : x28EntityField
    {
        public object DBValue { get; set; }
        public string ComboText { get; set; }
        public bool IsExternalDataSource { get; set; }
        private string _TypeName;
        public void SetTypeFromName(string strTypeName)
        {
            _TypeName = strTypeName;
            switch (_TypeName.ToLower())
            {
                case "boolean":
                    {
                        this.x24ID = x24IdENUM.tBoolean;
                        break;
                    }

                case "date":
                    {
                        this.x24ID = x24IdENUM.tDate;
                        break;
                    }

                case "datetime":
                    {
                        this.x24ID = x24IdENUM.tDateTime;
                        break;
                    }

                case "decimal":
                    {
                        this.x24ID = x24IdENUM.tDecimal;
                        break;
                    }

                case "integer":
                    {
                        this.x24ID = x24IdENUM.tInteger;
                        break;
                    }

                default:
                    {
                        this.x24ID = x24IdENUM.tString;
                        break;
                    }
            }
        }
    }
}
