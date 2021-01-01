using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
   
    public class o27Attachment:BaseBO
    {
        [Key]
        
        public int o13ID { get; set; }
        public int b07ID { get; set; }
        public int x31ID { get; set; }
        public int x40ID { get; set; }
        public string o27Name { get; set; }
        public string o27OriginalFileName { get; set; }
        public string o27FileExtension { get; set; }
        public string o27ArchiveFileName { get; set; }
        public string o27ArchiveFolder { get; set; }
        public int o27FileSize { get; set; }
        public string o27ContentType { get; set; }
        public string o27GUID { get; set; }

       
       

        public string FullPath { get; set; }



        public string o13Name;
        public int x29ID;
    }
}
