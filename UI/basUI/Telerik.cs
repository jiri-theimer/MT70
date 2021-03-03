using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelerikReportingBAS
{
    public static class Class1  //je nutné dodržet Class1 kvůli kompatibilitě s předchozí MT verzí 5+6
    {
        public static string ShowAsHHMM(double dblHours)
        {
            return "HH:HH hodiny vole";
        }
        public static string GetDateFormat(DateTime d, string format)
        {
            return "hovado GetDateFormat";
        }

        public static string tra(string vyraz, string langindex, string langsource)  //překlad popisků
        {
            int intLangIndex = BO.BAS.InInt(langindex);
            if (intLangIndex == 0)
            {
                return vyraz;
            }
            var lis = BO.BAS.ConvertString2List(langsource, "&#xD;&#xA;");
            foreach (string s in lis)
            {
                if (s.IndexOf(vyraz + "|") > -1)
                {
                    var arr = BO.BAS.ConvertString2List(s, "|");
                    return arr[intLangIndex];

                }
            }
            return vyraz + "?";
        }
    }
}
namespace UI
{
   
    
}
