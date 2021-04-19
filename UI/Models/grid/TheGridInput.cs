using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Models
{
    public class TheGridInput
    {
        public string entity { get; set; }
        public int j72id { get; set; }
        public int go2pid { get; set; }
        public string master_entity { get; set; }
        public int master_pid { get; set; }
        public BO.baseQuery query { get; set; }
        public string myqueryinline { get; set; }   // explicitní myquery ve tvaru název@typ@hodnota, může být víc hodnot
        public string oncmclick { get; set; } = "tg_cm(event)";
        public string ondblclick { get; set; } = "tg_dblclick";
        public string controllername { get; set; } = "TheGrid"; //název controlleru, přes který se zpracovávají grid události

        private string _fixedcolumns { get; set; }
        public string fixedcolumns
        {
            get
            {
                return _fixedcolumns;
            }
            set
            {
                _fixedcolumns = value;
                if (!string.IsNullOrEmpty(_fixedcolumns))
                {
                    List<string> lis = new List<string>();
                    var arr = _fixedcolumns.Split(",");
                    for(int i = 0; i < arr.Count(); i++)
                    {
                        if (arr[i].Contains("__"))
                        {
                            lis.Add(arr[i]);
                        }
                        else
                        {
                            lis.Add("a__" + entity + "__" + arr[i]);
                        }
                    }
                    _fixedcolumns = string.Join(",", lis);
                }

            }
        }

        public string extendpagerhtml { get; set; }
        public string viewstate { get; set; } 
    }
}
