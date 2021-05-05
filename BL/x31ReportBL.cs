using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ix31ReportBL
    {
        public BO.x31Report Load(int pid);
        public BO.x31Report LoadByCode(string code, int pid_exclude);
        public IEnumerable<BO.x31Report> GetList(BO.myQuery mq);
        public int Save(BO.x31Report rec);
        public BO.o27Attachment LoadReportDoc(int x31id);
        public bool IsReportWaiting4Generate(DateTime dNow, BO.x31Report rec);
        public BO.ThePeriod InhalePeriodFilter(BL.ThePeriodProvider pp);
        public string ParseExportFileNameMask(string strExportFileNameMask, string prefix, int pid);
    }
    class x31ReportBL : BaseBL, Ix31ReportBL
    {
        public x31ReportBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,x29.x29Name,j25.j25Name,j25.j25Ordinary,o27.o27ArchiveFileName as ReportFileName,o27.o27ArchiveFolder as ReportFolder,");
            sb(_db.GetSQL1_Ocas("x31"));
            sb(" FROM x31Report a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID LEFT OUTER JOIN j25ReportCategory j25 ON a.j25ID=j25.j25ID");
            sb(" LEFT OUTER JOIN (SELECT x31ID,o27ArchiveFolder,o27ArchiveFileName FROM o27Attachment WHERE x31ID IS NOT NULL) o27 ON a.x31ID=o27.x31ID");
            sb(strAppend);
            return sbret();
        }
        public BO.x31Report Load(int pid)
        {
            return _db.Load<BO.x31Report>(GetSQL1(" WHERE a.x31ID=@pid"), new { pid = pid });
        }
        public BO.x31Report LoadByCode(string code, int pid_exclude)
        {
            return _db.Load<BO.x31Report>(GetSQL1(" WHERE a.x31Code LIKE @code AND a.x31ID<>@pid_exclude"), new { code = code, pid_exclude = pid_exclude });
        }


        public IEnumerable<BO.x31Report> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) mq.explicit_orderby = "j25.j25Ordinary,j25.j25Name,a.x31Ordinary,a.x31Name";
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x31Report>(fq.FinalSql, fq.Parameters);            
        }



        public int Save(BO.x31Report rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }

            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddEnumInt("x29ID", rec.x29ID, true);
                p.AddInt("j25ID", rec.j25ID, true);
                p.AddString("x31Name", rec.x31Name);
                p.AddString("x31Code", rec.x31Code);
                p.AddEnumInt("x31FormatFlag", rec.x31FormatFlag);
                p.AddString("x31Description", rec.x31Description);
                p.AddBool("x31IsUsableAsPersonalPage", rec.x31IsUsableAsPersonalPage);
                p.AddBool("x31IsScheduling", rec.x31IsScheduling);
                p.AddInt("x31Ordinary", rec.x31Ordinary);
                p.AddEnumInt("x31QueryFlag", rec.x31QueryFlag);

                p.AddBool("x31IsRunInDay1", rec.x31IsRunInDay1);
                p.AddBool("x31IsRunInDay2", rec.x31IsRunInDay2);
                p.AddBool("x31IsRunInDay3", rec.x31IsRunInDay3);
                p.AddBool("x31IsRunInDay4", rec.x31IsRunInDay4);
                p.AddBool("x31IsRunInDay5", rec.x31IsRunInDay5);
                p.AddBool("x31IsRunInDay6", rec.x31IsRunInDay6);
                p.AddBool("x31IsRunInDay7", rec.x31IsRunInDay7);
                p.AddString("x31RunInTime", rec.x31RunInTime);
                p.AddString("x31SchedulingReceivers", rec.x31SchedulingReceivers);
                p.AddInt("x21ID_Scheduling", rec.x21ID_Scheduling, true);

                p.AddString("x31DocSqlSource", rec.x31DocSqlSource);
                p.AddString("x31DocSqlSourceTabs", rec.x31DocSqlSourceTabs);

                p.AddString("x31ExportFileNameMask", rec.x31ExportFileNameMask);

                p.AddBool("x31IsPeriodRequired", rec.x31IsPeriodRequired);

                p.AddInt("x31LangIndex", rec.x31LangIndex);

                int intPID = _db.SaveRecord("x31Report", p, rec);


                sc.Complete();

                return intPID;

            }


        }

        public bool ValidateBeforeSave(BO.x31Report rec)
        {
            if (string.IsNullOrEmpty(rec.x31Name))
            {
                this.AddMessage("Chybí vyplnit [Název sestavy]."); return false;
            }
            if (string.IsNullOrEmpty(rec.x31Code))
            {
                this.AddMessage("Chybí vyplnit [Kód sestavy]."); return false;
            }
            
            
           
            if (LoadByCode(rec.x31Code, rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("V systému existuje jiná sestava s kódem: {0}."), rec.x31Code)); return false;
            }


            return true;
        }

        public BO.o27Attachment LoadReportDoc(int x31id)
        {
            var mq = new BO.myQueryO27() { x31id=x31id };            
            var lisO27 = _mother.o27AttachmentBL.GetList(mq);

            if (lisO27.Count() > 0)
            {
                return lisO27.First();
            }

            return null;
        }

        public bool IsReportWaiting4Generate(DateTime dNow,BO.x31Report rec)
        {
            if (!rec.x31IsScheduling) return false;
            bool b = false;
            if (rec.x31IsRunInDay1 && dNow.DayOfWeek == DayOfWeek.Monday) b = true;
            if (rec.x31IsRunInDay2 && dNow.DayOfWeek == DayOfWeek.Tuesday) b = true;
            if (rec.x31IsRunInDay3 && dNow.DayOfWeek == DayOfWeek.Wednesday) b = true;
            if (rec.x31IsRunInDay4 && dNow.DayOfWeek == DayOfWeek.Thursday) b = true;
            if (rec.x31IsRunInDay5 && dNow.DayOfWeek == DayOfWeek.Friday) b = true;
            if (rec.x31IsRunInDay6 && dNow.DayOfWeek == DayOfWeek.Saturday) b = true;
            if (rec.x31IsRunInDay7 && dNow.DayOfWeek == DayOfWeek.Sunday) b = true;
            if (!b) return false;
            
            int secsNow = dNow.Hour * 60 * 60 + dNow.Minute * 60 + dNow.Second;
            if (secsNow >= BO.basTime.ConvertTimeToSeconds(rec.x31RunInTime)){
                if (rec.x31LastScheduledRun == null)
                {
                    return true;//sestava ještě nikdy nebyla generována
                }
                if (Convert.ToDateTime(rec.x31LastScheduledRun).Day == dNow.Day && Convert.ToDateTime(rec.x31LastScheduledRun).Month == dNow.Month && Convert.ToDateTime(rec.x31LastScheduledRun).Year == dNow.Year)
                {
                    return false;   //dnes již byla generována
                }
                return true;
            }
            return false;

        }

        public BO.ThePeriod InhalePeriodFilter(BL.ThePeriodProvider pp)
        {           
            var ret = pp.ByPid(0);
            int x = _mother.CBL.LoadUserParamInt("report-period-value");
            switch (x)
            {
                case 0:
                    ret.d1 = new DateTime(2000, 1, 1);
                    ret.d2 = new DateTime(3000, 1, 1);
                    return ret;
                case 1:
                    ret = pp.ByPid(x);
                    ret.d1 = _mother.CBL.LoadUserParamDate("report-period-d1");
                    ret.d2 = _mother.CBL.LoadUserParamDate("report-period-d2");
                    if (ret.d1 == null) ret.d1 = new DateTime(2000, 1, 1);
                    if (ret.d2 == null) ret.d2 = new DateTime(3000, 1, 1);
                    break;
                default:
                    ret = pp.ByPid(x);
                    break;
            }


            return ret;
        }

        public string ParseExportFileNameMask(string strExportFileNameMask,string prefix,int pid)
        {
            int intX29ID = BO.BASX29.GetInt(prefix);
            string strTab = BO.BASX29.GetEntity((BO.x29IdEnum)intX29ID);
            string s = $"SELECT {strExportFileNameMask} as Value FROM {strTab} WHERE {prefix}ID = {pid}";
            var ret = _db.Load<BO.GetString>(s);
            if (ret != null)
            {
                return ret.Value;
            }

            return null;
        }

    }
}
