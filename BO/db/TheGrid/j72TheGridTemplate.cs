using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class j72TheGridTemplate : BaseBO
    {
        [Key]
        public int j72ID { get; set; }
        public int j03ID { get; set; }
        public string j72Name { get; set; }
        public bool j72IsSystem { get; set; }

        public string j72Entity { get; set; }
        public string j72MasterEntity { get; set; }

        public string j72Columns { get; set; }

        public bool j72IsNoWrap { get; set; }


        public bool j72IsPublic{get;set;}
        public int j72SelectableFlag { get; set; } = 1;



        public bool j72HashJ73Query;

    }
}
