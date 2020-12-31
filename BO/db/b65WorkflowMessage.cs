using System;
using System.ComponentModel.DataAnnotations;

namespace BO
{
    
    public class b65WorkflowMessage:BaseBO
    {
        [Key]
        public int b65ID { get; set; }
        
        public int b01ID { get; set; }
        public string b01Name { get; }
        public string b65Name { get; set; }
        public string b65MessageSubject { get; set; }
        public string b65MessageBody { get; set; }

        public int x29ID { get; }
        public string x29Name { get; }
        public string Prefix;

    }
}
