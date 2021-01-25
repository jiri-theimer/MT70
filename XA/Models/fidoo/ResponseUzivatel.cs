using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XA.Models.fidoo
{

    public class ResponseUzivatel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string employeeNumber { get; set; }
        public string userState { get; set; }
        public string kycStatus { get; set; }
        public string language { get; set; }
        public string userId { get; set; }


        public int j02ID { get; set; }  //úmělý atribut kvůli dohledání uživatele v MT
    }

}
