using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public enum x24IdENUM
    {
        tInteger = 1,
        tString = 2,
        tDecimal = 3,
        tDate = 4,
        tDateTime = 5,
        tTime = 6,
        tBoolean = 7,
        tNone = 0
    }

    public enum x28FlagENUM
    {
        UserField = 1,
        GridField = 2
    }

    public class x28EntityField : BaseBO
    {
        public x28FlagENUM x28Flag { get; set; } = x28FlagENUM.UserField;
        public x29IdEnum x29ID { get; set; }
        public x24IdENUM x24ID { get; set; }
        public int x27ID { get; set; }
        public int x23ID { get; set; }
        public string x28Name { get; set; }

        public int x28Ordinary { get; set; }
        public bool x28IsAllEntityTypes { get; set; }

        public string x28DataSource { get; set; }
        public bool x28IsFixedDataSource { get; set; }
        public int x28TextboxHeight { get; set; }
        public int x28TextboxWidth { get; set; }
        public bool x28IsRequired { get; set; }

        public string x28Field { get; set; }

        public bool x28IsPublic { get; set; } = true;
        public string x28NotPublic_j04IDs { get; set; }
        public string x28NotPublic_j07IDs { get; set; }

        public string x28Grid_Field { get; set; }
        public string x28Grid_SqlSyntax { get; set; }
        public string x28Grid_SqlFrom { get; set; }
        public bool x28IsGridTotals { get; set; }
        public string x28Pivot_SelectSql { get; set; }
        public string x28Pivot_GroupBySql { get; set; }
        public string x28HelpText { get; set; }
        public string x28Query_Field { get; set; }
        public string x28Query_SqlSyntax { get; set; }
        public string x28Query_sqlComboSource { get; set; }


        protected string _x29Name { get; set; }
        public string x29Name
        {
            get
            {
                return _x29Name;
            }
        }
        protected string _x27Name { get; set; }
        public string x27Name
        {
            get
            {
                return _x27Name;
            }
        }
        protected string _TypeName;
        public string TypeName
        {
            get
            {
                return _TypeName;
            }
        }
        protected string _x23Name { get; set; }
        public string x23Name
        {
            get
            {
                return _x23Name;
            }
        }
        protected string _x23DataSource { get; set; }
        public string x23DataSource
        {
            get
            {
                return _x23DataSource;
            }
        }

        public string SourceTableName
        {
            get
            {
                switch (this.x29ID)
                {
                    case BO.x29IdEnum.p41Project:
                        {
                            return "p41Project_FreeField";
                        }

                    case BO.x29IdEnum.p28Contact:
                        {
                            return "p28Contact_FreeField";
                        }

                    case BO.x29IdEnum.p91Invoice:
                        {
                            return "p91Invoice_FreeField";
                        }

                    case BO.x29IdEnum.j02Person:
                        {
                            return "j02Person_FreeField";
                        }

                    case BO.x29IdEnum.p56Task:
                        {
                            return "p56Task_FreeField";
                        }

                    case x29IdEnum.p31Worksheet:
                        {
                            return "p31WorkSheet_FreeField";
                        }

                    default:
                        {
                            return "";
                        }
                }
            }
        }
    }
}
