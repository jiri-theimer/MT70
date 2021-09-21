using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace BO
{
    public class InitMyQuery
    {
        private List<string> _mqi { get; set; }     
        private string _master_prefix { get; set; }
        private int _master_pid { get; set; }
        
        private void handle_myqueryinline_input(string myqueryinline)
        {
            if (string.IsNullOrEmpty(myqueryinline))
            {
                return;
            }
            _mqi = BO.BAS.ConvertString2List(myqueryinline, "|");
            
        }
        public BO.baseQuery Load(string prefix, string master_prefix=null, int master_pid=0, string myqueryinline = null)
        {
            handle_myqueryinline_input(myqueryinline);           
            _master_prefix = validate_prefix(master_prefix);
            _master_pid = master_pid;

            //var ret = new BO.myQuery(prefix.Substring(0, 3));
            switch (prefix.Substring(0, 3))
            {
                case "b07":
                    return handle_myquery_reflexe(new BO.myQueryB07());
                case "c26":
                    return handle_myquery_reflexe(new BO.myQueryC26());
                case "j02":
                    return handle_myquery_reflexe(new BO.myQueryJ02());
                case "j04":
                    return handle_myquery_reflexe(new BO.myQueryJ04());
                case "j11":
                    return handle_myquery_reflexe(new BO.myQueryJ11());
                case "j61":
                    return handle_myquery_reflexe(new BO.myQueryJ61());
                case "j90":
                    return handle_myquery_reflexe(new BO.myQueryJ90());
                case "j92":
                    return handle_myquery_reflexe(new BO.myQueryJ92());
                case "o23":
                    return handle_myquery_reflexe(new BO.myQueryO23());
                case "o27":
                    return handle_myquery_reflexe(new BO.myQueryO27());
                case "o38":
                    return handle_myquery_reflexe(new BO.myQueryO38());
                case "p28":
                    return handle_myquery_reflexe(new BO.myQueryP28());
                case "p31":
                    return handle_myquery_reflexe(new BO.myQueryP31());               
                case "p32":
                    return handle_myquery_reflexe(new BO.myQueryP32());
                case "p34":
                    return handle_myquery_reflexe(new BO.myQueryP34());
                case "p41":
                case "le1":
                case "le2":
                case "le3":
                case "le4":
                case "le5":
                    return handle_myquery_reflexe(new BO.myQueryP41(prefix.Substring(0, 3)));
                case "p56":
                    return handle_myquery_reflexe(new BO.myQueryP56());
                case "o40":
                    return handle_myquery_reflexe(new BO.myQueryO40());
                case "o51":
                    return handle_myquery_reflexe(new BO.myQueryO51());
                case "p30":
                    return handle_myquery_reflexe(new BO.myQueryP30());
                case "p51":
                    return handle_myquery_reflexe(new BO.myQueryP51());
                case "p90":
                    return handle_myquery_reflexe(new BO.myQueryP90());
                case "p91":
                    return handle_myquery_reflexe(new BO.myQueryP91());
                case "p92":
                    return handle_myquery_reflexe(new BO.myQueryP92());
                case "x18":
                    return handle_myquery_reflexe(new BO.myQueryX18());
                case "x31":
                    return handle_myquery_reflexe(new BO.myQueryX31());
               
                case "x40":
                    return handle_myquery_reflexe(new BO.myQueryX40());
                default:
                    return handle_myquery_reflexe(new BO.myQuery(prefix.Substring(0, 3)));
            }
        }

        private T handle_myquery_reflexe<T>(T mq)
        {            
            if (_mqi != null)
            {   //na vstupu je explicitní myquery ve tvaru název|typ|hodnota
                for (int i = 0; i < _mqi.Count; i += 3)
                {
                    
                    switch (_mqi[i + 1])
                    {
                        case "int":
                            BO.Reflexe.SetPropertyValue(mq, _mqi[i], Convert.ToInt32(_mqi[i + 2]));
                            break;
                        case "date":                           
                            BO.Reflexe.SetPropertyValue(mq, _mqi[i], BO.BAS.String2Date(_mqi[i + 2]));
                            break;
                        case "bool":
                            BO.Reflexe.SetPropertyValue(mq, _mqi[i], BO.BAS.BG(_mqi[i + 2]));

                            break;
                        case "list_int":
                            BO.Reflexe.SetPropertyValue(mq, _mqi[i], BO.BAS.ConvertString2ListInt(_mqi[i + 2]));
                            break;
                        default:
                            BO.Reflexe.SetPropertyValue(mq, _mqi[i], _mqi[i + 2]);
                            break;
                    }
                }
            }

            //filtr podle master_prefix+master_pid
            if (_master_pid > 0 && _master_prefix != null)
            {
                switch (_master_prefix)
                {
                    case "le5":
                        BO.Reflexe.SetPropertyValue(mq, "p41id", _master_pid);                       
                        break;
                    case "le4":
                        BO.Reflexe.SetPropertyValue(mq, "leindex", 4);
                        BO.Reflexe.SetPropertyValue(mq, "lepid", _master_pid);
                        break;
                    case "le3":
                        BO.Reflexe.SetPropertyValue(mq, "leindex", 3);
                        BO.Reflexe.SetPropertyValue(mq, "lepid", _master_pid);
                        break;
                    case "le2":
                        BO.Reflexe.SetPropertyValue(mq, "leindex", 2);
                        BO.Reflexe.SetPropertyValue(mq, "lepid", _master_pid);
                        break;
                    case "le1":
                        BO.Reflexe.SetPropertyValue(mq, "leindex", 1);
                        BO.Reflexe.SetPropertyValue(mq, "lepid", _master_pid);
                        break;
                    default:
                        BO.Reflexe.SetPropertyValue(mq, _master_prefix + "id", _master_pid);
                        break;
                }

                BO.Reflexe.SetPropertyValue(mq, "master_prefix", _master_prefix);
            }

           

            return mq;
        }



        




        private string validate_prefix(string s = null)
        {
            if (s != null)
            {
                s = s.Substring(0, 3);
            }

            return s;
        }

    }
}
