﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DL
{
    public static class BAS
    {
        public static string ParseMergeSQL(string strSQL,string strPidValue,string par1=null,string par2 = null)
        {
            strSQL = strSQL.Replace("#pid#", strPidValue, StringComparison.OrdinalIgnoreCase);
            strSQL = strSQL.Replace("[%pid%]", strPidValue, StringComparison.OrdinalIgnoreCase);
            strSQL = strSQL.Replace("@pid", strPidValue, StringComparison.OrdinalIgnoreCase);
            par1 = BO.BAS.OcistitSQL(par1);
            par2 = BO.BAS.OcistitSQL(par2);
         
            strSQL = strSQL.Replace("#par1#", par1, StringComparison.OrdinalIgnoreCase);
            strSQL = strSQL.Replace("@par1", par1, StringComparison.OrdinalIgnoreCase);
            strSQL = strSQL.Replace("#par2#", par1, StringComparison.OrdinalIgnoreCase);
            
            strSQL = strSQL.Replace("@par2", par2, StringComparison.OrdinalIgnoreCase);

            strSQL = strSQL.Replace("delete ", "", StringComparison.OrdinalIgnoreCase);

            return BO.BAS.OcistitSQL(strSQL);
        }
        
    }
}
