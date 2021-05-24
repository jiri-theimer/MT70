using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class x67EntityRole:BaseBO
    {
        public x29IdEnum x29ID { get; set; }
        
        public string x67Name { get; set; }
        public string x67RoleValue { get; set; }
        public int x67Ordinary { get; set; }

        public string NameWithEntity { get
            {
                return this.x67Name + " (" + BO.BASX29.GetAlias((int)this.x29ID) + ")";
            }
        }
        
    }
}
