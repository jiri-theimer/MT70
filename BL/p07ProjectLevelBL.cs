using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ip07ProjectLevelBL
    {
        public BO.p07ProjectLevel Load(int pid);
        public IEnumerable<BO.p07ProjectLevel> GetList(BO.myQuery mq);
        public int Save(BO.p07ProjectLevel rec);


    }
    class p07ProjectLevelBL : BaseBL, Ip07ProjectLevelBL
    {

        public p07ProjectLevelBL(BL.Factory mother) : base(mother)
        {

        }

        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*," + _db.GetSQL1_Ocas("p07") + " FROM p07ProjectLevel a");
            sb(strAppend);
            return sbret();
        }

        public BO.p07ProjectLevel Load(int pid)
        {
            return _db.Load<BO.p07ProjectLevel>(GetSQL1(" WHERE a.p07ID=@pid"), new { pid = pid });
        }
        public IEnumerable<BO.p07ProjectLevel> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.p07ProjectLevel>(fq.FinalSql, fq.Parameters);
        }


        public int Save(BO.p07ProjectLevel rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.pid);
            p.AddInt("p07Level", rec.p07Level);
            p.AddString("p07NameSingular", rec.p07NameSingular);
            p.AddString("p07NamePlural", rec.p07NamePlural);
            p.AddString("p07NameInflection", rec.p07NameInflection);


            int intPID = _db.SaveRecord("p07ProjectLevel", p.getDynamicDapperPars(), rec);


          

            return intPID;
        }


        private bool ValidateBeforeSave(BO.p07ProjectLevel rec)
        {
           
            if (string.IsNullOrEmpty(rec.p07NameSingular) || string.IsNullOrEmpty(rec.p07NamePlural))
            {
                this.AddMessage("Názvy úrovně pro jednotné a množné číslo jsou povinná pole."); return false;
            }

            if (rec.p07Level<1 || rec.p07Level > 3)
            {
                this.AddMessage("Index úrovně musí být v rozsahu 1-3.");return false;
            }
            var lis = GetList(new BO.myQuery("p07"));
            if (lis.Where(p => p.p07Level == rec.p07Level && p.pid != rec.pid).Any())
            {
                var s = lis.Where(p => p.p07Level == rec.p07Level && p.pid != rec.pid).First().p07NameSingular;
                this.AddMessageTranslated(string.Format(_mother.tra("Pro index úrovně {0} již je založený záznam [{1}]."), rec.p07Level,s));
                return false;
            }

            return true;
        }
    }
}
