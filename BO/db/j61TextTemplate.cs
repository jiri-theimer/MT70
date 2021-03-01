using System;


namespace BO
{
    public class j61TextTemplate:BaseBO
    {
        public int j02ID_Owner { get; set; }
        public string j61Name { get; set; }
        public BO.x29IdEnum x29ID { get; set; }
        public string j61HtmlBody { get; set; }
        public string j61PlainTextBody { get; set; }
        public string j61MailSubject { get; set; }
        public int j61Ordinary { get; set; }

        public string j61MailTO { get; set; }
        public string j61MailCC { get; set; }
        public string j61MailBCC { get; set; }
        public string j61MessageFields { get; set; }
        public string j61HtmlTemplateFile { get; set; }

        public string x29Name { get; }
        
        public string Owner { get; }
     
    }
}
