using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public interface Io51TagBL
    {
        public BO.o51Tag Load(int pid);
        public IEnumerable<BO.o51Tag> GetList(BO.myQueryO51 mq);
        public IEnumerable<BO.o51Tag> GetList(string record_entity, int record_pid);
        public BO.TaggingHelper GetTagging(string record_entity, int record_pid);
        public BO.TaggingHelper GetTagging(List<int> o51ids);
        public int Save(BO.o51Tag rec);
        public int SaveTagging(string record_entity, int record_pid, string o51ids, int only_o53id = 0);
    }
    class o51TagBL : BaseBL, Io51TagBL
    {
        public o51TagBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,o51_o53.o53Name,o51_o53.x29IDs,o51_o53.o53IsMultiSelect,o51_o53.o53Ordinary,");
            sb(_db.GetSQL1_Ocas("o51"));
            sb(" FROM o51Tag a LEFT OUTER JOIN o53TagGroup o51_o53 ON a.o53ID=o51_o53.o53ID");
            sb(strAppend);
            return sbret();            
        }
        public BO.o51Tag Load(int pid)
        {
            return _db.Load<BO.o51Tag>(GetSQL1(" WHERE a.o51ID=@pid"), new { pid = pid });
        }
        public IEnumerable<BO.o51Tag> GetList(BO.myQueryO51 mq)
        {
            mq.explicit_orderby = "o51_o53.o53Ordinary,o51_o53.o53Name,a.o51Ordinary,a.o51Name";
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o51Tag>(fq.FinalSql, fq.Parameters);

        }
        
        public IEnumerable<BO.o51Tag> GetList(string record_entity,int record_pid)
        {
            int intX29ID = BO.BASX29.GetInt(record_entity);
            return _db.GetList<BO.o51Tag>(GetSQL1()+ " INNER JOIN o52TagBinding o52 ON a.o51ID=o52.o51ID WHERE o52.o52RecordPID=@pid AND o52.x29ID=@x29id ORDER BY o51_o53.o53Ordinary,o51_o53.o53Name,a.o51Ordinary,a.o51Name", new { pid = record_pid, x29id = intX29ID });

        }


        public BO.TaggingHelper GetTagging(string record_entity, int record_pid)
        {
            record_entity = record_entity.Substring(0, 3);
            return handle_tagging(GetList(record_entity, record_pid));
        }
        public BO.TaggingHelper GetTagging(List<int> o51ids)
        {
            var mq = new BO.myQueryO51() { pids = o51ids };            
            return handle_tagging(GetList(mq));
        }
        private BO.TaggingHelper handle_tagging(IEnumerable<BO.o51Tag> lis)
        {
            string s = String.Join(",", lis.Select(p => p.pid));

            var ret = new BO.TaggingHelper() { Tags = lis };
            if (lis.Count() > 0)
            {
                ret.TagPids = String.Join(",", lis.Select(p => p.pid));
                int intLastGroup = 0;
                ret.TagHtml = "";
                ret.TagNames = "";
                var sb = new System.Text.StringBuilder();
                int x = 0;
                sb.Append("<table>");
                foreach (BO.o51Tag c in lis)
                {
                    if (intLastGroup != c.o53ID)
                    {
                        if (x > 0)
                        {
                            sb.Append("</td>");
                            sb.Append("</tr>");
                        }
                        sb.Append("<tr>");
                        sb.Append(string.Format("<td><span class='material-icons-outlined-nosize' style='color:#4682B4;'>local_offer</span>{0}:</td>", c.o53Name));
                        sb.Append("<td style='padding:4px;'>");
                        if (ret.TagNames == "")
                        {
                            
                            ret.TagNames = c.o53Name + ": ";
                        }
                        else
                        {
                            
                            ret.TagNames += " ♣" + c.o53Name + ": ";
                        }
                        ret.TagNames += c.o51Name;
                    }
                    else
                    {
                        ret.TagNames += ", " + c.o51Name;
                    }
                    
                    sb.Append(c.HtmlText);

                    x += 1;
                    intLastGroup = c.o53ID;
                }
                if (x > 0)
                {
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</table>");
                ret.TagHtml = sb.ToString();
            }
            

            return ret;
        }
        

        public int SaveTagging(string record_entity, int record_pid, string o51ids,int only_one_o53id=0)
        {
            int intX29ID = BO.BASX29.GetInt(record_entity);
            
            if (only_one_o53id == 0)
            {
                _db.RunSql("DELETE FROM o52TagBinding WHERE o52RecordPid=@pid AND x29ID=@x29id", new { pid = record_pid, x29id = intX29ID });
                if (!String.IsNullOrEmpty(o51ids))
                {
                    _db.RunSql("INSERT INTO o52TagBinding(x29ID,o52RecordPid,o51ID) SELECT @x29id,@pid,o51ID FROM o51Tag WHERE o51ID IN (" + o51ids + ")", new { pid = record_pid, x29id = intX29ID });
                }
            }
            else
            {
                _db.RunSql("DELETE FROM o52TagBinding WHERE o52RecordPid=@pid AND x29ID=@x29id AND o51ID IN (select o51ID FROM o51Tag WHERE o53ID=@o53id)", new { pid = record_pid, x29id = intX29ID, o53id = only_one_o53id });
                if (!String.IsNullOrEmpty(o51ids))
                {
                    _db.RunSql("INSERT INTO o52TagBinding(x29ID,o52RecordPid,o51ID) SELECT @x29id,@pid,o51ID FROM o51Tag WHERE o53ID=@o53id AND o51ID IN (" + o51ids + ")", new { pid = record_pid, x29id = intX29ID, o53id = only_one_o53id });
                }
                else
                {
                    //vyčistění záznamu od štítku only_one_o53id: toto nezajistí sqlproc _core_o51_tagging_after_save
                    var recO53 = _mother.o53TagGroupBL.Load(only_one_o53id);
                    _db.RunSql($"UPDATE o54TagBindingInline set {recO53.o53Field}=NULL WHERE o54RecordPid={record_pid} AND x29ID={intX29ID}");
                }
            }
            
            
            var pars = new Dapper.DynamicParameters();
            pars.Add("userid", _db.CurrentUser.pid);
            pars.Add("x29id", intX29ID, System.Data.DbType.Int32);
            pars.Add("record_pid", record_pid, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);
            _db.RunSp("_core_o51_tagging_after_save", ref pars);


            return 1;
        }

        public int Save(BO.o51Tag rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }

            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.pid);
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
            p.AddInt("o53ID", rec.o53ID, true);
            p.AddString("o51Name", rec.o51Name);
            
            p.AddInt("o51Ordinary", rec.o51Ordinary);
            
            p.AddBool("o51IsColor", rec.o51IsColor);
            if (rec.o51IsColor == false)
            {
                rec.o51ForeColor = "";
                rec.o51BackColor = "";
            }            
            p.AddString("o51ForeColor", rec.o51ForeColor);
            p.AddString("o51BackColor", rec.o51BackColor);

            int intPID= _db.SaveRecord("o51Tag", p, rec);

            var pars = new Dapper.DynamicParameters();
            pars.Add("userid", _db.CurrentUser.pid);
            pars.Add("pid", intPID, System.Data.DbType.Int32);
            pars.Add("err_ret", "", System.Data.DbType.String, System.Data.ParameterDirection.Output);
            _db.RunSp("_core_o51_after_save", ref pars);

            return intPID;
        }

        private bool ValidateBeforeSave(BO.o51Tag rec)
        {
            if (rec.o53ID == 0)
            {
                this.AddMessage("Chybí vyplnit štítek (skupinu).");
                return false;
            }
            if (rec.o51Name.Contains(","))
            {
                this.AddMessage("Název položky štítku nesmí obsahovat čárku.");
                return false;
            }
            if (rec.o51Name.Length > 30)
            {
                this.AddMessage("V názvu položky štítku může být maximálně 30 znaků.");
                return false;
            }

            if (GetList(new BO.myQueryO51()).Where(p => p.pid != rec.pid && p.o51Name.ToLower() == rec.o51Name.Trim().ToLower()).Count() > 0)
            {
                this.AddMessage("Položka štítku s tímto názvem již existuje.");
                return false;
            }

            return true;
        }
    }
}
