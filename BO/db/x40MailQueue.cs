using System;

namespace BO
{
    public enum x40StateENUM
    {
        _NotSpecified = 0,
        InQueque = 1,
        IsError = 2,
        IsProceeded = 3,
        IsStopped = 4,
        WaitOnConfirm = 5
    }
    public class x40MailQueue:BaseBO
    {
        public BO.x29IdEnum x29ID { get; set; }
        public string x29Name { get; }
        public x40StateENUM x40State { get; set; } = x40StateENUM.InQueque;
        public int x40RecordPID { get; set; }
        public int j03ID_Sys { get; set; }
        public int o40ID { get; set; }
        public string x40Subject { get; set; }
        public string x40Body { get; set; }
        public bool x40IsHtmlBody { get; set; }
        public string x40SenderName { get; set; }
        public string x40SenderAddress { get; set; }
        public string x40Recipient { get; set; }
        public string x40CC { get; set; }
        public string x40BCC { get; set; }
        public string x40Attachments { get; set; }
        public DateTime? x40WhenProceeded { get; set; }
        public string x40ErrorMessage { get; set; }

        public bool x40IsAutoNotification { get; set; }
        public string x40MessageID { get; set; }
        public string x40BatchGuid { get; set; }
        public string x40ArchiveFolder { get; set; }
        public int x40EmlFileSize { get; set; }


        public string StatusAlias
        {
            get
            {
                switch (this.x40State)
                {
                    case x40StateENUM.InQueque:
                        {
                            return "Odesílá se";
                        }

                    case x40StateENUM.IsError:
                        {
                            return "Chyba";
                        }

                    case x40StateENUM.IsProceeded:
                        {
                            return "Odesláno";
                        }

                    case x40StateENUM.IsStopped:
                        {
                            return "Zastaveno";
                        }

                    case x40StateENUM.WaitOnConfirm:
                        {
                            return "Čeká na odeslání";
                        }

                    default:
                        {
                            return "?";
                        }
                }
            }
        }
        public string StatusColor
        {
            get
            {
                switch (this.x40State)
                {
                    case x40StateENUM.InQueque:
                        {
                            return "#996633";
                        }

                    case x40StateENUM.IsError:
                        {
                            return "#ff0000";
                        }

                    case x40StateENUM.IsProceeded:
                        {
                            return "#008000";
                        }

                    case x40StateENUM.IsStopped:
                        {
                            return "#ff66ff";
                        }

                    case x40StateENUM.WaitOnConfirm:
                        {
                            return "#0000ff";
                        }

                    default:
                        {
                            return "?";
                        }
                }
            }
        }
        
    }
}
