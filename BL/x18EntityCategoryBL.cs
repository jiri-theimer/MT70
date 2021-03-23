using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ix18EntityCategoryBL
    {
        public BO.x18EntityCategory Load(int pid);
        public IEnumerable<BO.x18EntityCategory> GetList(BO.myQuery mq);
        public IEnumerable<BO.x20EntiyToCategory> GetList_x20(List<int> x18ids);
        public IEnumerable<BO.x16EntityCategory_FieldSetting> GetList_x16(int x18id);
        public int Save(BO.x18EntityCategory rec, List<BO.x20EntiyToCategory> lisX20, List<BO.x16EntityCategory_FieldSetting> lisX16);

    }
    class x18EntityCategoryBL : BaseBL, Ix18EntityCategoryBL
    {
        public x18EntityCategoryBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*");
            sb(",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=102) then 1 else 0 end) as Is_j02");
            sb(",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=141) then 1 else 0 end) as Is_p41");
            sb(",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=328) then 1 else 0 end) as Is_p28");
            sb(",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=331) then 1 else 0 end) as Is_p31");
            sb(",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=356) then 1 else 0 end) as Is_p56");
            sb(",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=223) then 1 else 0 end) as Is_o23");
            sb(",convert(bit,case when a.x18ID IN (SELECT x18ID FROM x20EntiyToCategory WHERE x29ID=222) then 1 else 0 end) as Is_o22");
            sb(","+_db.GetSQL1_Ocas("x18"));
            sb(" FROM x18EntityCategory a");
            sb(strAppend);
            return sbret();
        }
        public BO.x18EntityCategory Load(int pid)
        {
            return _db.Load<BO.x18EntityCategory>(GetSQL1(" WHERE a.x18ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.x18EntityCategory> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x18EntityCategory>(fq.FinalSql, fq.Parameters);
        }

        public IEnumerable<BO.x20EntiyToCategory> GetList_x20(List<int> x18ids)
        {
            
            return _db.GetList<BO.x20EntiyToCategory>("SELECT * FROM x20EntiyToCategory WHERE x18ID IN (" + String.Join(",", x18ids) + ") ORDER BY x20Ordinary");
        }
        public IEnumerable<BO.x16EntityCategory_FieldSetting> GetList_x16(int x18id)
        {
            return _db.GetList<BO.x16EntityCategory_FieldSetting>("SELECT * FROM x16EntityCategory_FieldSetting WHERE x18ID=@pid ORDER BY x16Ordinary", new { pid = x18id });
        }
        public int Save(BO.x18EntityCategory rec,List<BO.x20EntiyToCategory> lisX20,List<BO.x16EntityCategory_FieldSetting> lisX16)
        {
            if (!ValidateBeforeSave(rec,lisX16))
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddString("x18Name", rec.x18Name);
                p.AddString("x18NameShort", rec.x18NameShort);
                p.AddInt("x18Ordinary", rec.x18Ordinary);
                p.AddBool("x18IsColors", rec.x18IsColors);
                p.AddInt("b01ID", rec.b01ID, true);
                p.AddInt("x38ID", rec.x38ID, true);
                p.AddInt("x38ID", rec.x38ID, true);                
                p.AddString("x18ReportCodes", rec.x18ReportCodes);
                p.AddEnumInt("x18EntryNameFlag", rec.x18EntryNameFlag);
                p.AddEnumInt("x18EntryCodeFlag", rec.x18EntryCodeFlag);
                p.AddEnumInt("x18EntryOrdinaryFlag", rec.x18EntryOrdinaryFlag);
                p.AddEnumInt("x18UploadFlag", rec.x18UploadFlag);
                p.AddInt("x18MaxOneFileSize", rec.x18MaxOneFileSize);
                p.AddString("x18AllowedFileExtensions", rec.x18AllowedFileExtensions);
                p.AddBool("x18IsAllowEncryption", rec.x18IsAllowEncryption);
                p.AddBool("x18IsCalendar", rec.x18IsCalendar);
                p.AddString("x18CalendarFieldStart", rec.x18CalendarFieldStart);
                p.AddString("x18CalendarFieldEnd", rec.x18CalendarFieldEnd);
                p.AddString("x18CalendarFieldSubject", rec.x18CalendarFieldSubject);
                p.AddString("x18CalendarResourceField", rec.x18CalendarResourceField);


                int intX23ID = rec.x23ID;
                if (intX23ID == 0)
                {
                    _db.RunSql("INSERT INTO x23EntityField_Combo(x23Name,x23UserUpdate,x23DateUpdate,x23UserInsert,x23DateInsert) VALUES(@x18name,@login,getdate(),@login,getdate())",new { x18name = rec.x18Name,login=_mother.CurrentUser.j03Login });
                    intX23ID = _db.GetIntegerFromSql("select max(x23ID) FROM x23EntityField_Combo");
                }
                else
                {
                    _db.RunSql("UPDATE x23EntityField_Combo SET x23Name=@x18name,x23UserUpdate=@login,x23DateUpdate=getdate() WHERE x23ID=@pid", new { x18name = rec.x18Name, login = _mother.CurrentUser.j03Login,pid=intX23ID });
                }
                p.AddInt("x23ID", intX23ID, true);

                int intX18ID = _db.SaveRecord("x18EntityCategory", p, rec);
                if (intX18ID > 0)
                {
                    var x18ids = new List<int>();
                    var lisX20Saved = GetList_x20(new List<int>() { intX18ID });
                    foreach(var c in lisX20)
                    {
                        p = new DL.Params4Dapper();
                        if (lisX20Saved.Any(p => p.x20ID == c.x20ID))
                        {
                            p.AddInt("pid", c.x20ID);
                        }
                        else
                        {
                            p.AddInt("pid", 0);
                        }                        
                        p.AddInt("x18ID", intX18ID,true);
                        p.AddInt("x29ID", c.x29ID,true);
                        p.AddEnumInt("x20EntryModeFlag", c.x20EntryModeFlag);                        
                        p.AddEnumInt("x20GridColumnFlag", c.x20GridColumnFlag);
                        p.AddEnumInt("x20EntityPageFlag", c.x20EntityPageFlag);
                        p.AddBool("x20IsMultiSelect", c.x20IsMultiSelect);
                        p.AddBool("x20IsClosed", c.x20IsClosed);
                        p.AddBool("x20IsEntryRequired", c.x20IsEntryRequired);
                        p.AddInt("x20Ordinary", c.x20Ordinary);
                        p.AddInt("x20EntityTypePID", c.x20EntityTypePID);
                        p.AddInt("x29ID_EntityType", c.x29ID_EntityType, true);
                        _db.SaveRecord("x20EntiyToCategory", p, c, false, false);
                    }
                    foreach(var c in lisX20Saved)
                    {
                        if (!lisX20.Any(p => p.x20ID == c.x20ID && p.x20ID>0))
                        {
                            _db.RunSql("DELETE FROM x20EntiyToCategory WHERE x20ID=@pid", new { pid = c.x20ID });
                        }
                    }

                    if (lisX16 != null)
                    {
                        _db.RunSql("UPDATE x16EntityCategory_FieldSetting SET x16FieldGroup=NULL WHERE x18ID=@pid", new { pid = intX18ID });    //příznak, že záznam byl uložen
                        foreach (var c in lisX16)
                        {
                            p = new DL.Params4Dapper();
                            p.AddInt("pid", _db.GetIntegerFromSql("select TOP 1 x16ID as Value FROM x16EntityCategory_FieldSetting WHERE x18ID=" + intX18ID.ToString() + " AND x16Field='" + c.x16Field + "')"));
                            p.AddInt("x18ID", intX18ID, true);
                            p.AddString("x16Field", c.x16Field);
                            p.AddString("x16Name", c.x16Name);
                            p.AddString("x16NameGrid", c.x16NameGrid);
                            p.AddBool("x16IsEntryRequired", c.x16IsEntryRequired);
                            p.AddBool("x16IsGridField", c.x16IsGridField);
                            p.AddBool("x16IsFixedDataSource", c.x16IsFixedDataSource);
                            p.AddString("x16DataSource", c.x16DataSource);
                            p.AddInt("x16TextboxHeight", c.x16TextboxHeight);

                            p.AddInt("x16Ordinary", c.x16Ordinary);
                            p.AddString("x16Format", c.x16Format);
                            p.AddString("x16FieldGroup", "1");  //příznak, že záznam byl uložen
                            int intX16ID = _db.SaveRecord("x16EntityCategory_FieldSetting", p, c, false, false);
                        }
                        _db.RunSql("DELETE FROM x16EntityCategory_FieldSetting WHERE x16FieldGroup IS NULL AND x18ID=@pid", new { pid = intX18ID });
                    }
                    sc.Complete();  //potvrzení transakce
                }


                return intX18ID;
            }
                
        }
        private bool ValidateBeforeSave(BO.x18EntityCategory rec, List<BO.x16EntityCategory_FieldSetting> lisX16)
        {
            if (string.IsNullOrEmpty(rec.x18Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
           if (lisX16 != null)
            {
                if (lisX16.Any(p => string.IsNullOrEmpty(p.x16Name)))
                {
                    this.AddMessage("V nastavení uživatelských polí chybí vyplnit název.");return false;
                }
                if (lisX16.Any(p => string.IsNullOrEmpty(p.x16Field)))
                {
                    this.AddMessage("V nastavení uživatelských polí chybí vyplnit pole."); return false;
                }
            }
            return true;
        }

    }
}
