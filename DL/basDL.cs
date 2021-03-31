using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void SaveFreeFields(DL.DbHandler db,int intSourcePID,List<BO.FreeFieldInput> lisFFI)
        {
           if (lisFFI == null)
            {
                return;
            }
            string strTable = lisFFI[0].SourceTableName;
            string strFieldPID = strTable.Substring(0, 3) + "ID";
            int intSavedPID = db.GetIntegerFromSql("select " + strFieldPID + " FROM " + strTable + " WHERE " + strFieldPID + "=" + intSourcePID.ToString());

            
           
            var p = new Params4Dapper();
            p.AddInt("pid", intSourcePID);

            foreach (var c in lisFFI)
            {
                switch (c.TypeName)
                {
                    case "date":
                    case "datetime":
                        p.AddDateTime(c.x28Field, c.DateInput);
                        break;
                    case "decimal":
                    case "integer":
                        p.AddDouble(c.x28Field, c.NumInput);
                        break;
                    case "boolean":
                        p.AddBool(c.x28Field, c.CheckInput);
                        break;
                    default:
                        p.AddString(c.x28Field, c.StringInput);
                        break;
                }
                
            }


            string strSQL = "";

            if (intSavedPID == 0)
            {

                strSQL = "INSERT INTO " + strTable + " ("+strFieldPID+"," + string.Join(",", lisFFI.Select(p => p.x28Field)) + ") VALUES (@pid," + string.Join(",", lisFFI.Select(p => "@" + p.x28Field)) + ")";
            }
            else
            {
                strSQL = "UPDATE " + strTable + " SET " + string.Join(",", lisFFI.Select(p => p.x28Field+" = @"+p.x28Field))+" WHERE "+strFieldPID+" = @pid";
            }
            
            
            db.RunSql(strSQL, p.getDynamicDapperPars());

        }
        
    }
}
