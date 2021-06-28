using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SepaInkasoAdd
{
    public class AccountIdentification4CZ
    {

        private string iBANField;

        /// <remarks/>
        public string IBAN
        {
            get
            {
                return this.iBANField;
            }
            set
            {
                this.iBANField = value;
            }
        }
    }
}
