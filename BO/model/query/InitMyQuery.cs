using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace BO
{
    public class InitMyQuery
    {
        private List<string> _eqs { get; set; }
        private string _master_prefix { get; set; }
        private int _master_pid { get; set; }

        private void handle_explicitquery_input(string explicitquery)
        {
            if (string.IsNullOrEmpty(explicitquery))
            {
                return;
            }
            _eqs = BO.BAS.ConvertString2List(explicitquery, "@");
        }
        public BO.baseQuery Load(string prefix, string master_prefix=null, int master_pid=0, string explicitquery = null)
        {
            handle_explicitquery_input(explicitquery);

            _master_prefix = validate_prefix(master_prefix);
            _master_pid = master_pid;

            var ret = new BO.myQuery(prefix.Substring(0, 3));
            switch (prefix.Substring(0, 3))
            {


                case "j02":
                    return handle_myquery_reflexe(new BO.myQueryJ02());
                   
                default:
                    return handle_myquery_reflexe(new BO.myQuery(prefix.Substring(0, 3)));
            }
        }

        private T handle_myquery_reflexe<T>(T mq)
        {
            if (_eqs != null)
            {   //na vstupu je explicitní myquery ve tvaru název@typ@hodnota
                for (int i = 0; i < _eqs.Count; i += 3)
                {
                    switch (_eqs[i + 1])
                    {
                        case "int":
                            BO.Reflexe.SetPropertyValue(mq, _eqs[i], Convert.ToInt32(_eqs[i + 2]));
                            break;
                        case "date":
                            BO.Reflexe.SetPropertyValue(mq, _eqs[i], BO.BAS.String2Date(_eqs[i + 2]));
                            break;
                        case "bool":
                            BO.Reflexe.SetPropertyValue(mq, _eqs[i], BO.BAS.BG(_eqs[i + 2]));
                            break;
                        default:
                            BO.Reflexe.SetPropertyValue(mq, _eqs[i], _eqs[i + 2]);
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
