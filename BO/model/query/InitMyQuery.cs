using System;
using System.Collections.Generic;
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

            var ret = new BO.myQuery(prefix.Substring(0, 3));
            switch (prefix.Substring(0, 3))
            {
                case "j02":
                    return handle_myquery_reflexe(new BO.myQueryJ02());
                case "o27":
                    return handle_myquery_reflexe(new BO.myQueryO27());
                case "o38":
                    return handle_myquery_reflexe(new BO.myQueryO38());
                case "p28":
                    return handle_myquery_reflexe(new BO.myQueryP28());
                case "p32":
                    return handle_myquery_reflexe(new BO.myQueryP32());
                case "p34":
                    return handle_myquery_reflexe(new BO.myQueryP34());
                case "p41":
                    return handle_myquery_reflexe(new BO.myQueryP41());
                case "o40":
                    return handle_myquery_reflexe(new BO.myQueryO40());
                case "o51":
                    return handle_myquery_reflexe(new BO.myQueryO51());

                default:
                    return handle_myquery_reflexe(new BO.myQuery(prefix.Substring(0, 3)));
            }
        }

        private T handle_myquery_reflexe<T>(T mq)
        {
            if (_mqi != null)
            {   //na vstupu je explicitní myquery ve tvaru název@typ@hodnota
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
            else
            {   //filtr podle master_prefix+master_pid
                if (_master_pid > 0 && _master_prefix != null)
                {
                    BO.Reflexe.SetPropertyValue(mq, _master_prefix + "id", _master_pid);
                }
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
