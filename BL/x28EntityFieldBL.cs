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
        public int Save(BO.x28EntityField rec);

    }
    class x28EntityFieldBL : BaseBL, Ix28EntityFieldBL
    {
        public x28EntityFieldBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("select a.*,x29.x29name,x27.x27Name,lower(x24.x24Name) as TypeName,x23.x23Name,x23.x23DataSource,");
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



        public int Save(BO.x28EntityField rec)
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
            p.AddBool("x28IsAllEntityTypes", rec.x28IsAllEntityTypes);
            p.AddBool("x28IsPublic", rec.x28IsPublic);
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

            return _db.SaveRecord("x28EntityField", p.getDynamicDapperPars(), rec);
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
            
            

            return true;
        }


        public string FindFirstUsableField(string strPrefix, BO.x24IdENUM x24id, int intX23ID)
        {

            string stype = x24id.ToString().ToLower();
            stype = stype.Substring(1, stype.Length);

            return _db.Load<BO.GetString>("select dbo.x28_getFirstUsableField('" + strPrefix + "','" + stype + "'," + intX23ID.ToString() + ") as Value").Value;
        }


    }
}
