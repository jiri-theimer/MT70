using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    public abstract class BaseBO
    {
        [Key]
        public int pid { get; set; }


        public bool isclosed;
        public string entity;
        public string UserInsert { get; set; }
        public DateTime? DateInsert { get; set; }

        public string UserUpdate;
        public DateTime? DateUpdate;

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidUntil { get; set; }


        
    }
}
