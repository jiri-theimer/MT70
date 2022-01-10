﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BL
{
    public interface Ix55WidgetBL
    {
        public BO.x55Widget Load(int pid);
        public BO.x55Widget LoadByCode(string code, int pid_exclude);
        public IEnumerable<BO.x55Widget> GetList(BO.myQuery mq);
        public int Save(BO.x55Widget rec, List<int> j04ids);

        public BO.x56WidgetBinding LoadState(int j03id,string skin);
        public int SaveState(BO.x56WidgetBinding rec);
        public int Clear2FactoryState(BO.x56WidgetBinding rec);

    }
    class x55WidgetBL : BaseBL, Ix55WidgetBL
    {
        public x55WidgetBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("x55"));
            sb(" FROM x55Widget a");
            sb(strAppend);
            return sbret();
        }
        public BO.x55Widget Load(int pid)
        {
            return _db.Load<BO.x55Widget>(GetSQL1(" WHERE a.x55ID=@pid"), new { pid = pid });
        }
        public BO.x55Widget LoadByCode(string code, int pid_exclude)
        {
            return _db.Load<BO.x55Widget>(GetSQL1(" WHERE a.x55Code LIKE @code AND a.x55ID<>@pid_exclude"), new { code = code, pid_exclude = pid_exclude });
        }

        public IEnumerable<BO.x55Widget> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x55Widget>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.x55Widget rec, List<int> j04ids)
        {
            
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.x55ID);
            p.AddString("x55Name", rec.x55Name);
            p.AddString("x55Code", rec.x55Code.Replace(".","_").Replace(" ",""));
            p.AddString("x55TableSql", rec.x55TableSql);
            p.AddString("x55TableColHeaders", rec.x55TableColHeaders);
            p.AddString("x55TableColTypes", rec.x55TableColTypes);
            p.AddString("x55Content", rec.x55Content);
            p.AddInt("x55Ordinal", rec.x55Ordinal);                        
            p.AddString("x55Image", rec.x55Image);
            p.AddString("x55Description", rec.x55Description);
            p.AddNonBlackColorString("x55BoxBackColor", rec.x55BoxBackColor);
            p.AddNonBlackColorString("x55HeaderBackColor", rec.x55HeaderBackColor);
            p.AddNonBlackColorString("x55HeaderForeColor", rec.x55HeaderForeColor);
            p.AddInt("x55DataTablesLimit", rec.x55DataTablesLimit);
            p.AddEnumInt("x55DataTablesButtons", rec.x55DataTablesButtons);
            p.AddString("x55Help", rec.x55Help);
            p.AddString("x55Skin", rec.x55Skin);
            p.AddString("x55ChartSql", rec.x55ChartSql);
            p.AddString("x55ChartHeaders", rec.x55ChartHeaders);
            p.AddString("x55ChartType", rec.x55ChartType);

            p.AddInt("x55BoxMaxHeight", rec.x55BoxMaxHeight);
            int intPID = _db.SaveRecord("x55Widget", p, rec);
            if (intPID > 0)
            {
                if (j04ids != null)
                {
                    if (rec.pid > 0)
                    {
                        _db.RunSql("DELETE FROM x57WidgetRestriction WHERE x55ID=@pid", new { pid = intPID });
                    }
                    if (j04ids.Count > 0)
                    {
                        _db.RunSql("INSERT INTO x57WidgetRestriction(x55ID,j04ID) SELECT @pid,j04ID FROM j04UserRole WHERE j04ID IN (" + string.Join(",", j04ids) + ")", new { pid = intPID });
                    }
                }
            }


            return intPID;
        }

        public bool ValidateBeforeSave(BO.x55Widget rec)
        {
            if (string.IsNullOrEmpty(rec.x55Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(rec.x55Code))
            {
                this.AddMessage("Chybí vyplnit [Kód]."); return false;
            }
            if (LoadByCode(rec.x55Code, rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("V systému již existuje jiný widget s kódem: {0}."), rec.x55Code)); return false;
            }
            rec.x55Code = Regex.Replace(rec.x55Code, "[^a-zA-Z0-9]", "_"); //kód raději pouze pro alfanumerické znaky

            if (!string.IsNullOrEmpty(rec.x55ChartSql) && rec.x55ChartType == null)
            {
                this.AddMessage("Chybí vyplnit [Typ grafu]."); return false;
            }
            if (!string.IsNullOrEmpty(rec.x55ChartSql) && rec.x55ChartHeaders == null)
            {
                this.AddMessage("Chybí vyplnit [Názvy veličin grafu]."); return false;
            }

            return true;
        }

        public int Clear2FactoryState(BO.x56WidgetBinding rec)
        {
            if (rec == null) return 0;

            _mother.CBL.SetUserParam("Widgets-ColumnsPerPage-" + rec.x56Skin, "2");    //výchozí jsou 2 sloupce

            

            rec.x56DockState = null;
            return SaveState(rec);
           

        }
        public BO.x56WidgetBinding LoadState(int j03id,string skin)
        {
            if (string.IsNullOrEmpty(skin)){
                skin = "index";
            }
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("x56"));
            sb(" FROM x56WidgetBinding a WHERE a.j03ID=@j03id AND a.x56Skin=@skin");
            
            var rec= _db.Load<BO.x56WidgetBinding>(sbret(), new { j03id = j03id,skin=skin });           
            if (rec==null)
            {                
                rec = new BO.x56WidgetBinding() { j03ID = j03id, x56Skin = skin };  //zatím nebyl vytvořený state widgetů
                if (Clear2FactoryState(rec) > 0)
                {
                    return LoadState(j03id, skin);
                }
                
            }
                        

            return rec;
        }

        public int SaveState(BO.x56WidgetBinding rec)
        {            
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddInt("j03ID", rec.j03ID,true);
            p.AddString("x56Skin", rec.x56Skin);
            p.AddString("x56Boxes", rec.x56Boxes);
            p.AddString("x56DockState", rec.x56DockState);
           
            return _db.SaveRecord("x56WidgetBinding", p, rec);

        }
    }
}
