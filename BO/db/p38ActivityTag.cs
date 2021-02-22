using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class p38ActivityTag:BaseBO
    {
        public string p38Name { get; set; }
        public string p38Code { get; set; }
        public string p38FreeText01 { get; set; }
        public string p38FreeText02 { get; set; }
        public int p38Ordinary { get; set; }

        public string CodeWithName
        {
            get
            {
                if (this.p38Code == null)
                    return this.p38Name;
                else
                    return this.p38Code + " " + this.p38Name;
            }
        }
    }
}
