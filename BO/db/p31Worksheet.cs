using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public enum p31HoursEntryFlagENUM
    {
        Hodiny = 1,
        Minuty = 2,
        NeniCas = 0
    }

    public class p31Worksheet:BaseBO
    {
        public int j02ID { get; set; }
        public int p41ID { get; set; }
    }
}
