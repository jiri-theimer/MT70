using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class p56Task:BaseBO
    {
        public int p41ID { get; set; }
        public int p57ID { get; set; }
        public int b02ID { get; set; }
        public int j02ID_Owner { get; set; }
       
        public int o25ID { get; set; }

        public string p56Name { get; set; }
        public string p56NameShort { get; set; }
        public string p56Code { get; set; }
        public string p56Description { get; set; }

        public DateTime? p56PlanFrom { get; set; }
        public DateTime? p56PlanUntil { get; set; }

        public int p56Ordinary { get; set; }
        public double p56Plan_Hours { get; set; }
        public double p56Plan_Expenses { get; set; }
        public bool p56IsPlan_Hours_Ceiling { get; set; }  // zastropovat plán hodin (nelze překročit)
        public bool p56IsPlan_Expenses_Ceiling { get; set; }   // zastropovat plán výdajů (nelze překročit)

        public bool p56IsHtml { get; set; }    // zadání/popis úkolu je v HTML
        public bool p56IsNoNotify { get; set; }


        public int p56CompletePercent { get; set; }
        public int? p56RatingValue { get; set; }
        public string p56ExternalPID { get; set; }

        public int p65ID { get; set; }
        public string p56RecurNameMask { get; set; }
        public DateTime? p56RecurBaseDate { get; set; }
        public int p56RecurMotherID { get; set; }
        public bool p56IsStopRecurrence { get; set; }

        
        public string ReceiversInLine { get; }
        

        
        public string Owner { get; }
       
        public string p57Name { get; }
      
       
        public int b01ID { get; }
        
        public string b02Name { get; }
      
        public string b02Color { get; }
       
        public string p41Name { get; }
       
        public string p41Code { get; }

      
        public string ProjectCodeAndName
        {
            get
            {
                return this.p41Code + " - " + this.p41Name;
            }
        }
        public string ProjectWithClient
        {
            get
            {
                if (this.Client == null)
                    return this.p41Name;
                else
                    return this.Client + " - " + this.p41Name;
            }
        }

        public string Client { get; }
        
        public string FullName
        {
            get
            {
                if (this.Client != null)
                    return this.p56Name + " [" + this.Client + " - " + this.p41Name + "]";
                else
                    return this.p56Name + " [" + this.p41Name + "]";
            }
        }
        public string NameWithTypeAndCode
        {
            get
            {
                return this.p57Name + ": " + this.p56Name + " (" + this.p56Code + ")";
            }
        }
        public string RecurNameMaskWIthTypeAndCode
        {
            get
            {
                return this.p57Name + ": " + this.p56RecurNameMask + " (" + this.p56Code + ")";
            }
        }
    }
}
