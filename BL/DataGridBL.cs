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
        public IEnumerable<BO.GetListOfPids> GetListOfFindPid(BO.baseQuery mq, int topscoperecs);
        public DataTable GetList4MailMerge(string prefix, int pid);
        public DataTable GetList4MailMerge(int pid, string individual_sql_source);
        public DataTable GetListFromPureSql(string sql);
        public string GetLastFinalSql();


    }
    class DataGridBL:BaseBL,IDataGridBL
    {
        private DL.FinalSqlCommand _q { get; set; }
        public DataGridBL(BL.Factory mother):base(mother)
        {
            
        }

        

        public DataTable GetList4MailMerge(int pid, string individual_sql_source)
        {
            individual_sql_source = individual_sql_source.Replace("@pid", pid.ToString()).Replace("#pid#", pid.ToString());
            return _db.GetDataTable(individual_sql_source);
        }

        
      
        public DataTable GetListFromPureSql(string sql)
        {
            sql = BO.BAS.OcistitSQL(sql);            
            return _db.GetDataTable(sql);
        }
        public IEnumerable<BO.GetListOfPids> GetListOfFindPid(BO.baseQuery mq,int topscoperecs)     //pro hledání záznamu v grid stránkách
        {
            BO.TheEntity ce = _mother.EProvider.ByPrefix(mq.Prefix);
            var sb = new System.Text.StringBuilder();
            sb.Append("SELECT TOP "+ topscoperecs.ToString());
            sb.Append(" a." + mq.Prefix + "ID as pid");
            sb.Append(",ROW_NUMBER() OVER(ORDER BY " + mq.explicit_orderby + ") as rowindex");
            sb.Append(" FROM ");
            sb.Append(ce.SqlFromGrid);    //úvodní FROM klauzule s primární "a" tabulkou

            List<string> relSqls = new List<string>();
            foreach (BO.TheGridColumn col in mq.explicit_columns.Where(x => x.RelName != null && x.RelName != "a"))
            {
                if (col.RelSqlDependOn != null && relSqls.Exists(p => p == col.RelSqlDependOn) == false)
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


            _q = DL.basQuery.GetFinalSql(sb.ToString(), mq, _mother.CurrentUser, true);
            
            return _db.GetList<BO.GetListOfPids>(_q.FinalSql, _q.Parameters);
          
        }
        public string GetLastFinalSql()
        {
            if (_q == null) return null;
            
            return _q.FinalSql;
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
                
                mq.explicit_columns = new BL.TheColumnsProvider(_mother.EProvider,_mother.Translator).getDefaultPallete(false,mq,_mother);    //na vstupu není přesný výčet sloupců -> pracovat s default sadou
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
            
            if (bolGetTotalsRow)
            {
                sb.Append($",COUNT(a.{mq.PrefixDb}ID) as RowsCount");     //sumační dotaz gridu
                switch (mq.Prefix)
                {
                    case "p31": //součty pro hodnoty záložek hodiny/výdaje/odměny                        
                        sb.Append(",SUM(case when p34x.p33ID=1 then 1 end) as RowsTime");
                        sb.Append(",SUM(case when p34x.p33ID IN (2,5) AND p34x.p34IncomeStatementFlag=1 then 1 end) as RowsExpense");
                        sb.Append(",SUM(case when p34x.p33ID IN (2,5) AND p34x.p34IncomeStatementFlag=2 then 1 end) as RowsFee");
                        sb.Append(",SUM(case when p34x.p33ID=3 then 1 end) as RowsKusovnik");
                        break;
                    case "j02":
                        sb.Append(",SUM(case when a.j02IsIntraPerson=1 then 1 end) as RowsInternal");
                        sb.Append(",SUM(case when a.j02IsIntraPerson=0 then 1 end) as RowsContact");
                        break;
                }                
            }
            else
            {
                sb.Append(",");                
                sb.Append(_db.GetSQL1_Ocas(mq.PrefixDb, true, !ce.IsWithoutValidity));    //select dotaz gridu

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
            foreach (BO.TheGridColumn col in mq.explicit_columns.Where(x => x.RelSqlInCol != null))  //sloupce, které mají na míru RelSqlInCol definovanou přímo ve sloupci
            {               
                if (!relSqls.Exists(p => p == col.RelSqlInCol))
                {
                    if (col.RelName == null)
                    { 
                        relSqls.Add(col.RelSqlInCol);
                        sb.Append(" ");
                        sb.Append(col.RelSqlInCol);
                    }
                    else
                    {   //sloupec s explicitním sql relací a zároveň z jiné relace
                        string upraveno = col.RelSqlInCol.Replace("a.", col.RelName + ".");
                        upraveno = upraveno.Replace("_relname_", col.RelName);
                        if (!relSqls.Exists(p=>p==upraveno))
                        {
                            relSqls.Add(upraveno);

                            sb.Append(" ");
                            sb.Append(upraveno);
                        }
                        
                        
                    }
                    
                }

            }


            //vždy musí být nějaké výchozí třídění v ce.SqlOrderBy!!
            if (bolGetTotalsRow == false && String.IsNullOrEmpty(mq.explicit_orderby))
            {
                mq.explicit_orderby = ce.SqlOrderBy;
            }



            //parametrický dotaz s WHERE klauzulí

            _q = DL.basQuery.GetFinalSql(sb.ToString(), mq, _mother.CurrentUser, true);    //závěrečné vygenerování WHERE a ORDERBY klauzule
            
            if (bolGetTotalsRow == false && mq.OFFSET_PageSize > 0)
            {
                _q.FinalSql += " OFFSET @pagesize*@pagenum ROWS FETCH NEXT @pagesize ROWS ONLY";
                if (_q.Parameters4DT == null) _q.Parameters4DT = new List<DL.Param4DT>();
                _q.Parameters4DT.Add(new DL.Param4DT() { ParamType = "int", ParName = "pagesize", ParValue = mq.OFFSET_PageSize });
                _q.Parameters4DT.Add(new DL.Param4DT() { ParamType = "int", ParName = "pagenum", ParValue = mq.OFFSET_PageNum });

            }
            //BO.BASFILE.LogInfo(q.FinalSql);

            return _db.GetDataTable(_q.FinalSql, _q.Parameters4DT);
            
        }


        public DataTable GetList4MailMerge(string prefix, int pid)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("SELECT ");
            switch (prefix)
            {
                case "j02":
                    sb.Append("a.*,j07.j07Name,j03.j03Login,j03.j03LangIndex,j04.*,j02free.*");
                    sb.Append(" FROM j02Person a LEFT OUTER JOIN j07PersonPosition j07 on a.j07ID=j07.j07ID LEFT OUTER JOIN j03User j03 ON a.j02ID=j03.j02ID LEFT OUTER JOIN j04UserRole j04 ON j03.j04ID=j04.j04ID");
                    sb.Append(" LEFT OUTER JOIN j02Person_FreeField j02free ON a.j02ID=j02free.j02ID");
                    break;
                case "j03":
                    sb.Append("a.*,j07.*,j02.*,j04.*");
                    sb.Append(" FROM j03User a INNER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN j02Person j02 ON a.j02ID=j02.j02ID LEFT OUTER JOIN j07PersonPosition j07 on j02.j07ID=j07.j07ID");
                    break;
                case "p90":
                    sb.Append("a.*,p89.*,p90free.*,p28.*,j27.*");
                    sb.Append(" FROM p90Proforma a LEFT OUTER JOIN p89ProformaType p89 ON a.p89ID=p89.p89ID LEFT OUTER JOIN p28Contact p28 ON a.p28ID=p28.p28ID");
                    sb.Append(" LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID");
                    sb.Append(" LEFT OUTER JOIN p90Proforma_FreeField p90free ON a.p90ID=p90free.p90ID");
                    break;
                case "p91":
                    sb.Append("a.*,p92.*,p93.*,p28.*,j27.*,p91free.*");
                    sb.Append(" FROM p91Invoice a INNER JOIN p92InvoiceType p92 ON a.p92ID=p92.p92ID LEFT OUTER JOIN p28Contact p28 ON a.p28ID=p28.p28ID LEFT OUTER JOIN p93InvoiceHeader p93 on p92.p93ID=p93.p93ID");
                    sb.Append(" LEFT OUTER JOIN j27Currency j27 ON a.j27ID=j27.j27ID");
                    sb.Append(" LEFT OUTER JOIN p91Invoice_FreeField p91free ON a.p91ID=p91free.p91ID");
                    break;
                case "p28":
                    sb.Append("a.*,p29.*,p28free.*");
                    sb.Append(" FROM p28Contact a LEFT OUTER JOIN p29ContactType p29 on a.p29ID=p29.p29ID");
                    sb.Append(" LEFT OUTER JOIN p28Contact_FreeField p28free ON a.p28ID=p28free.p28ID");
                    break;
                case "p41":
                    sb.Append("a.*,p28.*,p42.*,p41free.*");
                    sb.Append(" FROM p41Project a LEFT OUTER JOIN p42ProjectType p42 on a.p42ID=p42.p42ID");
                    sb.Append(" LEFT OUTER JOIN p28Contact p28 a.p28ID_Client=p28.p28ID");
                    sb.Append(" LEFT OUTER JOIN p41Project_FreeField p41free ON a.p41ID=p41free.p41ID");
                    break;
                case "p31":
                    sb.Append("a.*,j02.*,p32.*,p34.*,p28Client.*,p70.*,j27billing_orig.*");
                    sb.Append(" FROM p31Worksheet a");
                    sb.Append(" INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN p32Activity p32 ON a.p32ID=p32.p32ID");
                    sb.Append(" INNER JOIN p34ActivityGroup p34 ON p32.p34ID=p34.p34ID INNER JOIN p41Project p41 ON a.p41ID=p41.p41ID");
                    sb.Append(" LEFT OUTER JOIN p28Contact p28Client ON p41.p28ID_Client=p28Client.p28ID LEFT OUTER JOIN p48ProjectGroup p48 ON p41.p48ID=p48.p48ID");
                    sb.Append(" LEFT OUTER JOIN p70BillingStatus p70 ON a.p70ID=p70.p70ID LEFT OUTER JOIN p71ApproveStatus p71 ON a.p71ID=p71.p71ID LEFT OUTER JOIN p72PreBillingStatus p72trim ON a.p72ID_AfterTrimming=p72trim.p72ID LEFT OUTER JOIN p72PreBillingStatus p72approve ON a.p72ID_AfterApprove=p72approve.p72ID");
                    sb.Append(" LEFT OUTER JOIN j27Currency j27billing_orig ON a.j27ID_Billing_Orig=j27billing_orig.j27ID");

                    break;
            }
            sb.Append(" WHERE a." + prefix + "ID=" + pid.ToString());
            return _db.GetDataTable(sb.ToString());
        }
    }
}
