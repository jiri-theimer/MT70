using System;


namespace BO
{
    public enum o36IdEnum
    {
        InvoiceAddress = 1,
        PostalAddress = 2,
        Other = 3
    }
    public class o37Contact_Address:o38Address
    {
        public int p28ID { get; set; }
        public int o38ID { get; set; }
        public o36IdEnum o36ID { get; set; }

        public bool IsSetAsDeleted { get; set; }

        public string o36Name { get; }
        
    }
}
