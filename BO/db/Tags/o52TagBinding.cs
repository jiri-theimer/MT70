using System.ComponentModel.DataAnnotations;
namespace BO
{
    public class o52TagBinding
    {
        [Key]
        public int o52ID { get; set; }
        public int o51ID { get; set; }
        public int o52RecordPID { get; set; }
        public int x29ID { get; set; }

        public string o51Name { get; }

    }
}
