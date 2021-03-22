using System;

namespace BO
{
    public class x16EntityCategory_FieldSetting:BaseBO
    {
        public int x16ID { get; set; }
        public int x18ID { get; set; }
        public bool x16IsEntryRequired { get; set; }
        public string x16Name { get; set; }
        public string x16NameGrid { get; set; }
        public string x16Field { get; set; }
        public int x16Ordinary { get; set; }
        public string x16DataSource { get; set; }
        public bool x16IsFixedDataSource { get; set; }
        public bool x16IsGridField { get; set; }
        public int x16TextboxHeight { get; set; }
        public int x16TextboxWidth { get; set; }        
        public bool x16IsReportField { get; set; }
        public string x16FieldGroup { get; set; }
        public string x16Format { get; set; }


        public BO.x24IdENUM FieldType
        {
            get
            {
                if (this.x16Field.ToLower().Contains("number"))
                    return x24IdENUM.tDecimal;

                if (this.x16Field.ToLower().Contains("date"))
                    return x24IdENUM.tDateTime;

                if (this.x16Field.ToLower().Contains("boolean"))
                    return x24IdENUM.tBoolean;

                return x24IdENUM.tString;
            }
        }
        
    }
}
