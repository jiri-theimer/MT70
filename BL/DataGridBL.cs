using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace BL
{
    public interface IDataGridBL
    {
        public DataTable GetList(BO.baseQuery mq, bool bolGetTotalsRow = false);
        public DataTable GetList4MailMerge(string prefix, int pid);
        public DataTable GetList4MailMerge(int pid, string individual_sql_source);
        public DataTable GetListFromPureSql(string sql);
    }
    class DataGridBL:BaseBL,IDataGridBL
    {
       
        public DataGridBL(BL.Factory mother):base(mother)
        {
            
        }

        

        public DataTable GetList4MailMerge(int pid, string individual_sql_source)
        {
            individual_sql_source = individual_sql_source.Replace("@pid", pid.ToString()).Replace("#pid#", pid.ToString());
            return _db.GetDataTable(individual_sql_source);
        }

        public DataTable GetList4MailMerge(string prefix,int pid)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("SELECT ");
            switch (prefix)
            {                
                case "j02":
                    sb.Append("a.*,j07.j07Name,j03.j03Login,j03.j03LangIndex,j04.*");
                    sb.Append(" FROM j02Person a LEFT OUTER JOIN j07Position j07 on a.j07ID=j07.j07ID LEFT OUTER JOIN j03User j03 ON a.j02ID=j03.j02ID LEFT OUTER JOIN j04UserRole j04 ON j03.j04ID=j04.j04ID");
                    break;
                case "j03":
                    sb.Append("a.*,j07.j07Name,j02.*,j04.*");
                    sb.Append(" FROM j03User a INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID LEFT OUTER JOIN j07Position j07 on j02.j07ID=j07.j07ID");
                    break;
            }
            sb.Append(" WHERE a." + prefix + "ID=" + pid.ToString());
            return _db.GetDataTable(sb.ToString());           
        }
      
        public DataTable GetListFromPureSql(string sql)
        {
            sql = BO.BAS.OcistitSQL(sql);            
            return _db.GetDataTable(sql);
        }
        public DataTable GetList(BO.baseQuery mq,bool bolGetTotalsRow=false)
        {            
            var sb = new System.Text.StringBuilder();
            sb.Append("SELECT ");
            if (mq.TopRecordsOnly > 0)
            {
                sb.Append("TOP "+mq.TopRecordsOnly.ToString()+" ");
            }
          
            if (mq.explicit_columns == null || mq.explicit_columns.Count()==0)
            {
                
                mq.explicit_columns = new BL.TheColumnsProvider(_mother.App,_mother.EProvider,_mother.Translator).getDefaultPallete(false,mq);    //na vstupu není přesný výčet sloupců -> pracovat s default sadou
            }
            if (bolGetTotalsRow)
            {
                sb.Append(string.Join(",", mq.explicit_columns.Select(p => p.getFinalSqlSyntax_SUM())));   //součtová řádka gridu
            }
            else
            {
                sb.Append(string.Join(",", mq.explicit_columns.Select(p => p.getFinalSqlSyntax_SELECT())));    //grid sloupce               
            }
            BO.TheEntity ce = _mother.EProvider.ByPrefix(mq.Prefix);
            
            if (bolGetTotalsRow == true)
            {
                sb.Append(string.Format(",COUNT(a.{0}ID) as RowsCount", mq.Prefix));     //sumační dotaz gridu
            }
            else
            {
                sb.Append(",");                
                sb.Append(_db.GetSQL1_Ocas(mq.Prefix, true, !ce.IsWithoutValidity));    //select dotaz gridu

            }
            
            if (mq.explicit_selectsql != null){
                sb.Append("," + mq.explicit_selectsql);
            }

            sb.Append(" FROM ");
            
            sb.Append(ce.SqlFromGrid);    //úvodní FROM klauzule s primární "a" tabulkou            
            
            List<string> relSqls = new List<string>();
            foreach (BO.TheGridColumn col in mq.explicit_columns.Where(x => x.RelName != null && x.RelName != "a"))
            {
                if (col.RelSqlDependOn != null && relSqls.Exists(p=>p==col.RelSqlDependOn)==false)
                {
                    relSqls.Add(col.RelSqlDependOn);
                    sb.Append(" ");
                    sb.Append(col.RelSqlDependOn);
                }
                if (relSqls.Exists(p => p == col.RelSql) == false)
                {
                    relSqls.Add(col.RelSql);
                    sb.Append(" ");
                    sb.Append(col.RelSql);
                }

            }
            
            
            //vždy musí být nějaké výchozí třídění v ce.SqlOrderBy!!
            if (bolGetTotalsRow == false && String.IsNullOrEmpty(mq.explicit_orderby)) mq.explicit_orderby = ce.SqlOrderBy;



            //parametrický dotaz s WHERE klauzulí
            
            DL.FinalSqlCommand q = DL.basQuery.GetFinalSql(sb.ToString(), mq, _mother.CurrentUser, true);    //závěrečné vygenerování WHERE a ORDERBY klauzule

            if (bolGetTotalsRow == false && mq.OFFSET_PageSize > 0)
            {
                q.FinalSql += " OFFSET @pagesize*@pagenum ROWS FETCH NEXT @pagesize ROWS ONLY";
                if (q.Parameters4DT == null) q.Parameters4DT = new List<DL.Param4DT>();
                q.Parameters4DT.Add(new DL.Param4DT() { ParamType = "int", ParName = "pagesize", ParValue = mq.OFFSET_PageSize });
                q.Parameters4DT.Add(new DL.Param4DT() { ParamType = "int", ParName = "pagenum", ParValue = mq.OFFSET_PageNum });

            }
            
            return _db.GetDataTable(q.FinalSql, q.Parameters4DT);
            
        }


        
    }
}
