using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface Ix28EntityFieldBL
    {
        public BO.x28EntityField Load(int pid);
        public IEnumerable<BO.x28EntityField> GetList(BO.myQuery mq);
        public int Save(BO.x28EntityField rec,List<BO.x26EntityField_Binding> lisX26);
        public IEnumerable<BO.x26EntityField_Binding> GetList_x26(int x28id);
        public System.Data.DataTable GetFieldsValues(int pid, IEnumerable<BO.x28EntityField> fields);   //vrací hodnoty polí v odpovídající tabulce entity

    }
    class x28EntityFieldBL : BaseBL, Ix28EntityFieldBL
    {
        public x28EntityFieldBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("select a.*,x29.x29name,x27.x27Name,lower(x24.x24Name) as TypeName,x23.x23Name,x23.x23DataSource,x29.x29TableName,");
            sb(_db.GetSQL1_Ocas("x28"));
            sb(" FROM x28EntityField a inner join x29Entity x29 on a.x29id=x29.x29id inner join x24DataType x24 on a.x24id=x24.x24id");
            sb(" LEFT OUTER JOIN x27EntityFieldGroup x27 on a.x27ID=x27.x27ID LEFT OUTER JOIN x23EntityField_Combo x23 ON a.x23ID=x23.x23ID");
            sb(strAppend);
            return sbret();
        }
        public BO.x28EntityField Load(int pid)
        {
            return _db.Load<BO.x28EntityField>(GetSQL1(" WHERE a.x28ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.x28EntityField> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x28EntityField>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.x26EntityField_Binding> GetList_x26(int x28id)
        {
            return _db.GetList<BO.x26EntityField_Binding>("select *,convert(bit,1) as IsChecked from x26EntityField_Binding where x28ID=@pid", new { pid = x28id });
        }

        public System.Data.DataTable GetFieldsValues(int pid, IEnumerable<BO.x28EntityField> fields)
        {
            if (fields.Count() == 0)
            {
                return null;
            }
            string strSQL = "SELECT "+pid.ToString()+" AS pid," + string.Join(",", fields.Select(p => p.x28Field)) + " FROM " + fields.First().SourceTableName + " WHERE " + fields.First().x29TableName.Substring(0, 3) + "ID = "+pid.ToString();

            return _db.GetDataTable(strSQL);
        }

        public int Save(BO.x28EntityField rec, List<BO.x26EntityField_Binding> lisX26)
        {
            if (string.IsNullOrEmpty(rec.x28Field) && rec.x28Flag == BO.x28FlagENUM.UserField)
            {
                rec.x28Field = FindFirstUsableField(BO.BASX29.GetPrefix(rec.x29ID), rec.x24ID, rec.x23ID);
            }
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.pid);
            p.AddString("x28Name", rec.x28Name);
            p.AddInt("x28Ordinary", rec.x28Ordinary);
            p.AddString("x28Field", rec.x28Field);
            p.AddEnumInt("x24id", rec.x24ID);
            p.AddEnumInt("x29id", rec.x29ID);
            p.AddInt("x27ID", rec.x27ID,true);
            p.AddInt("x23ID", rec.x23ID, true);
            p.AddString("x28datasource", rec.x28DataSource);
            p.AddBool("x28IsFixedDataSource", rec.x28IsFixedDataSource);
            p.AddBool("x28IsRequired", rec.x28IsRequired);            
            p.AddBool("x28IsPublic", rec.x28IsPublic);
            if (rec.x28IsPublic)
            {
                rec.x28NotPublic_j04IDs = null; rec.x28NotPublic_j07IDs = null;
            }
            p.AddString("x28NotPublic_j04IDs", rec.x28NotPublic_j04IDs);
            p.AddString("x28NotPublic_j07IDs", rec.x28NotPublic_j07IDs);


            p.AddString("x28Grid_Field", rec.x28Grid_Field);
            p.AddString("x28Grid_SqlSyntax", rec.x28Grid_SqlSyntax);
            p.AddString("x28Grid_SqlFrom", rec.x28Grid_SqlFrom);
            p.AddString("x28Pivot_SelectSql", rec.x28Pivot_SelectSql);
            p.AddString("x28Pivot_GroupBySql", rec.x28Pivot_GroupBySql);

            p.AddString("x28Query_SqlSyntax", rec.x28Query_SqlSyntax);
            p.AddString("x28Query_Field", rec.x28Query_Field);
            p.AddString("x28Query_sqlComboSource", rec.x28Query_sqlComboSource);

            p.AddInt("x28TextboxHeight", rec.x28TextboxHeight);
            p.AddInt("x28TextboxWidth", rec.x28TextboxWidth);

            p.AddString("x28HelpText", rec.x28HelpText);
            p.AddBool("x28IsAllEntityTypes", rec.x28IsAllEntityTypes);

            int intPID = _db.SaveRecord("x28EntityField", p, rec);
            if (lisX26 != null)
            {
                rec.x28IsAllEntityTypes = true;
                if (rec.pid > 0 && intPID>0)
                {
                    _db.RunSql("DELETE FROM x26EntityField_Binding WHERE x28ID=@pid", new { pid = intPID });
                }
                if (intPID > 0)
                {
                    foreach (var c in lisX26.Where(p => p.IsChecked))
                    {
                        _db.RunSql("INSERT INTO x26EntityField_Binding(x28ID,x26EntityTypePID,x29ID_EntityType,x26IsEntryRequired) VALUES(@pid,@typepid,@x29id,@b)", new { pid = intPID, typepid = c.x26EntityTypePID, x29id = c.x29ID_EntityType, b = c.x26IsEntryRequired });
                        rec.x28IsAllEntityTypes = false;
                    }
                }
                _db.RunSql("UPDATE x28EntityField set x28IsAllEntityTypes=@b WHERE x28ID=@pid", new { pid = intPID, b = rec.x28IsAllEntityTypes });
            }

            return intPID;
        }
        private bool ValidateBeforeSave(BO.x28EntityField rec)
        {
            switch (rec.x28Flag)
            {
                case BO.x28FlagENUM.UserField:
                    if (rec.x29ID == BO.x29IdEnum.o23Doc)
                    {
                        this.AddMessage("Pro entitu [Dokument] je povoleno zakládat pouze GRID sloupec na míru.");return false;
                    }
                    if (rec.x23ID > 0 && rec.x24ID != BO.x24IdENUM.tInteger)
                    {
                        this.AddMessage("Pole s vazbou na combo seznam musí být formátu INTEGER."); return false;
                    }
                    if (string.IsNullOrEmpty(rec.x28Field))
                    {
                        this.AddMessage("Systém nedokázal najít volné fyzické pole pro toto uživatelské pole."); return false;
                    }
                    break;
                case BO.x28FlagENUM.GridField:
                    if (string.IsNullOrEmpty(rec.x28Grid_Field))
                    {
                        Random rnd = new Random();
                        rec.x28Grid_Field = "gridfield_" + rnd.Next(1, 10000).ToString();
                    }
                    if (string.IsNullOrEmpty(rec.x28Grid_Field) && string.IsNullOrEmpty(rec.x28Query_Field))
                    {
                        this.AddMessage("Chybná specifikace pole.");return false;
                    }
                    if (rec.x28Grid_Field.Contains(".") || rec.x28Grid_Field.Contains("[") || rec.x28Grid_Field.Contains("]"))
                    {
                        this.AddMessage("Pole obsahuje zakázané znaky.");return false;
                    }
                    break;

            }
            if (string.IsNullOrEmpty(rec.x28Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (!rec.x28IsPublic)
            {
                if (string.IsNullOrEmpty(rec.x28NotPublic_j04IDs) && string.IsNullOrEmpty(rec.x28NotPublic_j07IDs))
                {
                    this.AddMessage("Pokud pole není dostupné všem uživatelům, zaškrtněte okruh rolí nebo pozic.");return false;
                }
            }
            

            return true;
        }


        public string FindFirstUsableField(string strPrefix, BO.x24IdENUM x24id, int intX23ID)
        {

            string stype = x24id.ToString().ToLower();
            stype = stype.Substring(1, stype.Length-1);

            return _db.Load<BO.GetString>("select dbo.x28_getFirstUsableField('" + strPrefix + "','" + stype + "'," + intX23ID.ToString() + ") as Value").Value;
        }


    }
}
