

namespace BO
{
    public class o58FieldBag:BaseBO
    {
        public BO.x24IdENUM x24ID { get; set; }
        public int o58ParentID { get; set; }
        public string o58Name { get; set; }
        public string o58Code { get; set; }
        public string o58Description { get; set; }
        public int o58Ordinary { get; set; }

        public string ParentName { get; }
        public string ParentCode { get; }
    }
}
