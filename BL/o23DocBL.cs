using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Io23DocBL
    {
        public BO.o23Doc Load(int pid);
        public IEnumerable<BO.o23Doc> GetList(BO.myQueryO23 mq);
        public int Save(BO.o23Doc rec, int intX18ID, List<BO.x19EntityCategory_Binding> lisX19, string uploadguid,List<int>o27ids_delete,List<BO.x69EntityRole_Assign> lisX69);
        public IEnumerable<BO.x19EntityCategory_Binding> GetList_x19(int o23id);
        public BO.o23RecDisposition InhaleRecDisposition(BO.o23Doc rec);
        public bool SaveHtmlEditor(int o23id, string s);
        public string LoadHtmlEditor(int o23id);

    }
    class o23DocBL : BaseBL, Io23DocBL
    {
        public o23DocBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,x18.x18ID,x18.x18Name,j02owner.j02LastName+' '+j02owner.j02FirstName as Owner,b02.b02Name,b02.b02Color,");
            sb(_db.GetSQL1_Ocas("o23"));
            sb(" FROM o23Doc a");
            sb(" INNER JOIN x23EntityField_Combo x23 ON a.x23ID=x23.x23ID INNER JOIN x18EntityCategory x18 ON x23.x23ID=x18.x23ID LEFT OUTER JOIN j02Person j02owner ON a.j02ID_Owner=j02owner.j02ID LEFT OUTER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID");
            sb(strAppend);
            return sbret();
        }
        public BO.o23Doc Load(int pid)
        {
            return _db.Load<BO.o23Doc>(GetSQL1(" WHERE a.o23ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.o23Doc> GetList(BO.myQueryO23 mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.o23Doc>(fq.FinalSql, fq.Parameters);
        }

        private string GetSQL1_X19(string strAppend = null)
        {
            sb("SELECT a.*,o23.o23Name,x20.x18ID,isnull(x18.x18NameShort,x18.x18Name) as x18Name,");
            sb("o23.o23ForeColor,o23.o23BackColor,x20.x29ID,x20.x20Name,x20.x20IsMultiselect,x20.x20IsMultiselect,");
            sb(_db.GetSQL1_Ocas("x19"));            
            sb(" from x19EntityCategory_Binding a INNER JOIN x20EntiyToCategory x20 ON a.x20ID=x20.x20ID INNER JOIN o23Doc o23 ON a.o23ID=o23.o23ID INNER JOIN x18EntityCategory x18 ON x20.x18ID=x18.x18ID");
            sb(strAppend);
            return sbret();
        }

        public IEnumerable<BO.x19EntityCategory_Binding> GetList_x19(int o23id)
        {
            return _db.GetList<BO.x19EntityCategory_Binding>(GetSQL1_X19(" WHERE a.o23ID=@o23id"),new { o23id = o23id });
        }

        public int Save(BO.o23Doc rec,int intX18ID,List<BO.x19EntityCategory_Binding> lisX19,string uploadguid, List<int> o27ids_delete, List<BO.x69EntityRole_Assign> lisX69)
        {
            var recX18 = _mother.x18EntityCategoryBL.Load(intX18ID);
            var lisX20 = _mother.x18EntityCategoryBL.GetList_x20(intX18ID);
            rec.x23ID = recX18.x23ID;

            if (!ValidateBeforeSave(rec, recX18, lisX19, lisX20))
            {
                return 0;
            }
            using (var sc = new System.Transactions.TransactionScope())
            {
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.pid);
                p.AddInt("x23ID", rec.x23ID, true);
                p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);
                p.AddString("o23GUID", rec.o23GUID);
                p.AddString("o23Name", rec.o23Name);
                p.AddString("o23Code", rec.o23Code);
                p.AddInt("o23Ordinary", rec.o23Ordinary);
                p.AddString("o23ArabicCode", rec.o23ArabicCode);
                p.AddString("o23ForeColor", rec.o23ForeColor);
                p.AddString("o23BackColor", rec.o23BackColor);
                p.AddBool("o23IsDraft", rec.o23IsDraft);
                p.AddBool("o23IsEncrypted", rec.o23IsEncrypted);
                p.AddString("o23Password", rec.o23Password);

                p.AddString("o23ExternalPID", rec.o23ExternalPID);
                p.AddDateTime("o23ReminderDate", rec.o23ReminderDate);

                p.AddString("o23FreeText01", rec.o23FreeText01);
                p.AddString("o23FreeText02", rec.o23FreeText02);
                p.AddString("o23FreeText03", rec.o23FreeText03);
                p.AddString("o23FreeText04", rec.o23FreeText04);
                p.AddString("o23FreeText05", rec.o23FreeText05);
                p.AddString("o23FreeText06", rec.o23FreeText06);
                p.AddString("o23FreeText07", rec.o23FreeText07);
                p.AddString("o23FreeText08", rec.o23FreeText08);
                p.AddString("o23FreeText09", rec.o23FreeText09);
                p.AddString("o23FreeText10", rec.o23FreeText10);
                p.AddString("o23FreeText11", rec.o23FreeText11);
                p.AddString("o23FreeText12", rec.o23FreeText12);
                p.AddString("o23FreeText13", rec.o23FreeText13);
                p.AddString("o23FreeText14", rec.o23FreeText14);
                p.AddString("o23FreeText15", rec.o23FreeText15);
                p.AddString("o23BigText", rec.o23BigText);

                p.AddDouble("o23FreeNumber01", rec.o23FreeNumber01);
                p.AddDouble("o23FreeNumber02", rec.o23FreeNumber02);
                p.AddDouble("o23FreeNumber03", rec.o23FreeNumber03);
                p.AddDouble("o23FreeNumber04", rec.o23FreeNumber04);
                p.AddDouble("o23FreeNumber05", rec.o23FreeNumber05);

                p.AddDateTime("o23FreeDate01", rec.o23FreeDate01);
                p.AddDateTime("o23FreeDate02", rec.o23FreeDate02);
                p.AddDateTime("o23FreeDate03", rec.o23FreeDate03);
                p.AddDateTime("o23FreeDate04", rec.o23FreeDate04);
                p.AddDateTime("o23FreeDate05", rec.o23FreeDate05);

                p.AddBool("o23FreeBoolean01", rec.o23FreeBoolean01);
                p.AddBool("o23FreeBoolean02", rec.o23FreeBoolean02);
                p.AddBool("o23FreeBoolean03", rec.o23FreeBoolean03);
                p.AddBool("o23FreeBoolean04", rec.o23FreeBoolean04);
                p.AddBool("o23FreeBoolean05", rec.o23FreeBoolean05);

                int intPID = _db.SaveRecord("o23Doc", p, rec);
                if (intPID > 0)
                {
                    SaveX19Binding(intPID, lisX19, lisX20);

                    if (lisX69 != null && !DL.BAS.SaveX69(_db, "o23", intPID, lisX69))
                    {
                        this.AddMessageTranslated("Error: DL.BAS.SaveX69");
                        return 0;
                    }
                   
                    if (!string.IsNullOrEmpty(uploadguid))
                    {
                        BO.b07Comment recB07 = null;                        
                        if (rec.pid > 0)
                        {
                            recB07 = _mother.b07CommentBL.LoadByRecord(223, intPID);
                        }
                        if (recB07==null) recB07 = new BO.b07Comment() { b07Value = "upload", x29ID = BO.x29IdEnum.o23Doc, b07RecordPID = intPID };
                        _mother.b07CommentBL.Save(recB07, uploadguid,null);
                    }
                    if (o27ids_delete != null)
                    {
                        foreach (int intO27ID in o27ids_delete)
                        {
                            _mother.o27AttachmentBL.Move2Deleted(_mother.o27AttachmentBL.Load(intO27ID));
                            _mother.CBL.DeleteRecord("o27Attachment", intO27ID);

                        }
                    }

                    if (_db.RunSql("exec dbo.o23_aftersave @o23id,@j03id_sys", new { o23id = intPID, j03id_sys = _mother.CurrentUser.pid }))
                    {
                        sc.Complete();
                        return intPID;
                    }

                     
                }
            }

            return 0;
                
        }

        private void SaveX19Binding(int o23id, List<BO.x19EntityCategory_Binding> lisX19, IEnumerable<BO.x20EntiyToCategory> lisX20)
        {
            var lisSaved = GetList_x19(o23id);
            foreach(var c in lisX19)
            {                
                var rec = new BO.x19EntityCategory_Binding() { o23ID = o23id, x20ID = c.x20ID,x19RecordPID=c.x19RecordPID };
                var cx20 = _mother.x18EntityCategoryBL.LoadX20(c.x20ID);
                if (cx20.x20IsMultiSelect)
                {
                    if (lisSaved.Any(p => p.x20ID == c.x20ID && p.x19RecordPID == c.x19RecordPID))
                    {
                        rec = lisSaved.Where(p => p.x20ID == c.x20ID && p.x19RecordPID == c.x19RecordPID).First();
                    }
                }
                else
                {
                    if (lisSaved.Any(p => p.x20ID == c.x20ID))
                    {
                        rec = lisSaved.Where(p => p.x20ID == c.x20ID).First();
                        rec.x19RecordPID = c.x19RecordPID;
                    }                    
                }
                
                if (c.IsSetAsDeleted)
                {
                    if (c.pid > 0)
                    {
                        _db.RunSql("DELETE FROM x19EntityCategory_Binding WHERE x19ID=@pid", new { pid = c.pid });
                    }
                }
                else
                {
                    var p = new DL.Params4Dapper();
                    p.AddInt("pid", rec.pid);
                    p.AddInt("o23ID", o23id, true);
                    p.AddInt("x20ID", rec.x20ID, true);
                    p.AddInt("x19RecordPID", rec.x19RecordPID, true);

                    _db.SaveRecord("x19EntityCategory_Binding", p, rec);
                }
                
            }
            //foreach(var c in lisSaved)
            //{
            //    if (lisX19.Where(p => p.x20ID == c.x20ID && p.x19RecordPID == c.x19RecordPID).Count()==0)
            //    {
            //        _db.RunSql("DELETE FROM x19EntityCategory_Binding WHERE x20ID=@x20id AND x19RecordPID=@recpid AND o23ID=@o23id", new { x20id = c.x20ID, recpid = c.x19RecordPID, o23id = o23id });
            //    }
            //}
        }
        private bool ValidateBeforeSave(BO.o23Doc rec,BO.x18EntityCategory recX18, List<BO.x19EntityCategory_Binding> lisX19,IEnumerable<BO.x20EntiyToCategory> lisX20)
        {
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _mother.CurrentUser.j02ID;
            
            if (recX18 == null)
            {
                this.AddMessage("Chybí vyplnit [Typ dokumentu]."); return false;
            }
            if (string.IsNullOrEmpty(rec.o23Name) && recX18.x18EntryNameFlag == BO.x18EntryNameENUM.Manual)
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            foreach(var c in lisX20.Where(p => p.x20IsEntryRequired))
            {
                if (lisX19.Where(p => p.x20ID == c.x20ID).Count() == 0)
                {
                    var cx = _mother.x18EntityCategoryBL.Load(c.x18ID);
                    this.AddMessageTranslated(string.Format("V záznamu [{0}] chybí povinná vazba.",cx.x18Name));
                    return false;
                }
            }
            if (rec.o23ReminderDate !=null && Convert.ToDateTime(rec.o23ReminderDate) < DateTime.Now.AddMinutes(-5))
            {
                this.AddMessage("Zadaný čas připomenutí už nemá smysl."); return false;
            }
         
            
            return true;
        }

        public BO.o23RecDisposition InhaleRecDisposition(BO.o23Doc rec)
        {
            var c = new BO.o23RecDisposition();

            if (rec.j02ID_Owner == _mother.CurrentUser.j02ID || _mother.CurrentUser.IsAdmin)
            {
                c.OwnerAccess = true; c.ReadAccess = true;
                return c;
            }
            

            return c;
        }

        public bool SaveHtmlEditor(int o23id,string s)
        {
            var rec = _db.Load<BO.GetInteger>("select o23ID as Value FROM o23BigData WHERE o23ID=@pid",new { pid = o23id });
            if (rec == null)
            {
                return _db.RunSql("INSERT INTO o23BigData(o23ID,o23HtmlContent) VALUES(@pid,@s)", new { pid = o23id, s = s });
            }
            else
            {
               return _db.RunSql("UPDATE o23BigData set o23HtmlContent=@s WHERE o23ID=@pid", new { pid = o23id, s = s });
            }
        }

        public string LoadHtmlEditor(int o23id)
        {
            var rec = _db.Load<BO.GetString>("select o23HtmlContent as Value FROM o23BigData WHERE o23ID=@pid", new { pid = o23id });
            if (rec == null)
            {
                return null;
            }
            else
            {
                return rec.Value;
            }
        }

    }
}
