using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    
    public enum SslModeENUM
    {
        _NoSSL = 0,
        Implicit = 1,
        Explicit = 2
    }
    public class o40SmtpAccount : BaseBO
    {

        public int j02ID_Owner { get; set; }
        public bool o40IsGlobalDefault { get; set; }
        public string o40Name { get; set; }
        public string o40EmailAddress { get; set; }
        public string o40Server { get; set; }
        public string o40Login { get; set; }
        public string o40Password { get; set; }
        public int o40Port { get; set; }
        public bool o40IsVerify { get; set; }        
        public bool o40IsUsePersonalReply { get;set;}
        
        public SslModeENUM o40SslModeFlag { get; set; } = SslModeENUM._NoSSL;

    }
}
