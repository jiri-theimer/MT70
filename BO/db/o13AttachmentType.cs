using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class o13AttachmentType:BaseBO
    {
       
        public string o13Name { get; set; }
        public int x29ID { get; set; }
        public string o13ArchiveFolder { get; set; }
        public bool o13IsArchiveFolderWithPeriodSuffix { get; set; }
        public bool o13IsUniqueArchiveFileName { get; set; }

        public string x29Name { get; set; } //combo

        public string SharpFolder;  //folder s dvojtými backslashi
    }
}
