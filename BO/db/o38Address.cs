using System;

namespace BO
{
    public class o38Address:BaseBO
    {
        public string o38Name { get; set; }
        public string o38Street { get; set; }
        public string o38City { get; set; }
        public string o38ZIP { get; set; }
        public string o38Country { get; set; }
        public string o38Description { get; set; }
        public string o38AresID { get; set; }

        public string FullAddress
        {
            get
            {
                string s = "";
                if (this.o38Street != null)
                    s = this.o38Street + ", " + this.o38City;
                else
                    s = this.o38City;
                if (this.o38ZIP != null)
                    s += ", " + this.o38ZIP;
                if (this.o38Country != null)
                    s += ", " + this.o38Country;
                return s;
            }
        }
        public string FullAddressWithBreaks
        {
            get
            {
                string s = "";
                if (this.o38Street != null)
                    s = this.o38Street + System.Environment.NewLine + this.o38City;
                else
                    s = this.o38City;
                if (this.o38ZIP != null)
                    s += System.Environment.NewLine + this.o38ZIP;
                if (this.o38Country != null)
                    s += System.Environment.NewLine + this.o38Country;
                return s;
            }
        }

        public int o36ID { get; }
        public int p28ID { get; set; }
        public string o36Name
        {
            get
            {
                if (this.o38Description == "1")
                {
                    return "Fakturační adresa";
                }
                if (this.o38Description == "2")
                {
                    return "Poštovní adresa";
                }
                if (this.o38Description == "3")
                {
                    return "Jiné";
                }
                if (this.o36ID==1)
                {
                    return "Fakturační adresa";
                }
                if (this.o36ID==2)
                {
                    return "Poštovní adresa";
                }
               
                return "Jiné";
                
            }
        }
    }
}
