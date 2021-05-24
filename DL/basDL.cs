﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

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

        public static bool SaveX69(DL.DbHandler db,string recprefix,int recpid, List<BO.x69EntityRole_Assign> lisX69)
        {
            string guid = BO.BAS.GetGuid();
            foreach(var c in lisX69)
            {
                db.RunSql("INSERT INTO p85TempBox(p85GUID,p85DataPID,p85OtherKey1,p85OtherKey2,p85prefix) VALUES(@guid,@x67id,@j02id,@j11id,@prefix)", new { guid = guid, x67id=c.x67ID,j02id=BO.BAS.TestIntAsDbKey(c.j02ID),j11id=BO.BAS.TestIntAsDbKey(c.j11ID),prefix=recprefix });
            }

            var pars = new Dapper.DynamicParameters();

            pars.Add("recordpid", recpid, DbType.Int32);
            pars.Add("x29id", BO.BASX29.GetInt(recprefix), DbType.Int32);
            pars.Add("guid", guid, DbType.String);
            pars.Add("x67id_only_onerole_update", 0, DbType.Int32);


            if (db.RunSp("x69_save_one_record", ref pars,false) == "1")
            {
                return true;
            }

            return false;
            
        }

        public static bool SaveFreeFields(DL.DbHandler db, int intSourcePID,List<BO.FreeFieldInput> lisFFI)
        {           
           if (lisFFI == null)
            {
                return true;
            }
            string strTable = lisFFI[0].SourceTableName;
            string strFieldPID = strTable.Substring(0, 3) + "ID";
            int intSavedPID = db.GetIntegerFromSql("select " + strFieldPID + " FROM " + strTable + " WHERE " + strFieldPID + "=" + intSourcePID.ToString());
            
            if (lisFFI.Where(p => p.IsVisible == false).Count() > 0)
            {
                lisFFI = lisFFI.Where(p => p.IsVisible == true).ToList();   //ukládat pouze viditelná pole na formuláři
            }
            var p = new Params4Dapper();
            p.AddInt("pid", intSourcePID);

            foreach (var c in lisFFI)
            {
                
                switch (c.TypeName)
                {
                    case "date":
                    case "datetime":
                        if (c.x28IsRequired && c.DateInput==null)
                        {
                            db.CurrentUser.AddMessage($"Pole ** {c.x28Name} ** je povinné k vyplnění. ");return false;
                        }
                        p.AddDateTime(c.x28Field, c.DateInput);                        
                            break;
                    case "decimal":
                    case "integer":
                        if (c.x28IsRequired && c.NumInput == 0)
                        {
                            db.CurrentUser.AddMessage($"Pole ** {c.x28Name} ** je povinné k vyplnění. ");return false;
                        }
                        p.AddDouble(c.x28Field, c.NumInput);
                        break;
                    case "boolean":
                        p.AddBool(c.x28Field, c.CheckInput);
                        break;
                    default:
                        if (c.x28IsRequired && string.IsNullOrEmpty(c.StringInput))
                        {
                            db.CurrentUser.AddMessage($"Pole ** {c.x28Name} ** je povinné k vyplnění. ");return false;
                        }
                        if (c.StringInput !=null && c.StringInput.Length > 20)
                        {
                            //kontrola velikosti obsahu string polí
                            int intMaxSize = db.Load<BO.GetInteger>("select dbo.getfieldsize(@fld, @tbl) as Value", new { fld = c.x28Field, tbl = strTable }).Value;
                            if (intMaxSize > 0 && c.StringInput.Length > intMaxSize)
                            {
                                db.CurrentUser.AddMessage($"Délka pole ** {c.x28Name} ** může být maximálně {intMaxSize} znaků. Vy posíláte {c.StringInput.Length} znaků.");return false;
                            }
                        }

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


            return db.RunSql(strSQL, p.getDynamicDapperPars());
           
        }
        
    }
}
