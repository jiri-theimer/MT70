using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BL;
using BO;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using SQLitePCL;

namespace UI
{
    public class dataExport
    {
        
        public bool ToXLSX(System.Data.DataTable dt, string strFilePath, BO.baseQuery mq)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Grid");
                int row = 1;
                int col = 1;

                foreach (var c in mq.explicit_columns)
                {
                    worksheet.Cell(row, col).Value = c.Header;
                    worksheet.Cell(row, col).Style.Font.Bold = true;

                    col += 1;
                }
                row += 1;
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    col = 1;
                    foreach (var c in mq.explicit_columns)
                    {

                        if (!Convert.IsDBNull(dr[c.UniqueName]))
                        {
                            worksheet.Cell(row, col).Value = dr[c.UniqueName];

                        }
                        col += 1;
                    }

                    row += 1;

                }
                worksheet.Columns(1,col).AdjustToContents();
                

                workbook.SaveAs(strFilePath);
               
            }

            return true;
        }
        public bool ToXLSX(System.Data.DataTable dt, string strFilePath, List<BO.StringPair> cols)
        {
            //key: název pole, value: záhlaví sloupce
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Grid");
                int row = 1;
                int col = 1;                

                foreach (var c in cols)
                {
                    worksheet.Cell(row, col).Value = c.Value;
                    worksheet.Cell(row, col).Style.Font.Bold = true;

                    col += 1;
                }
                var coltypes = new List<StringPair>();
                foreach(System.Data.DataColumn c in dt.Columns)
                {
                    coltypes.Add(new BO.StringPair() { Key = c.ColumnName, Value = c.DataType.Name });                    
                }
                //worksheet.Column(1).CellsUsed().SetDataType(XLDataType.Text);
                row += 1;
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    col = 1;
                    foreach (var c in cols)
                    {
                        
                        if (!Convert.IsDBNull(dr[c.Key]))
                        {
                            worksheet.Cell(row, col).Value = dr[c.Key];
                            
                            if (c.Key.ToUpper().Substring(0, 3) != "COL")    //vynechat v exportu statistiky sloupce colXXXX, které jsou fyzicky stringy!
                            {
                                switch (coltypes.Where(p => p.Key == c.Key).First().Value)
                                {
                                    case "Boolean":
                                        worksheet.Cell(row, col).DataType = XLDataType.Boolean;
                                        break;
                                    case "String":
                                        worksheet.Cell(row, col).DataType = XLDataType.Text;                                        
                                        break;
                                    case "DateTime":
                                        worksheet.Cell(row, col).DataType = XLDataType.DateTime;
                                        break;
                                }
                            }

                        }
                        col += 1;
                    }

                    row += 1;

                }
                worksheet.Columns(1, col).AdjustToContents();
                workbook.SaveAs(strFilePath);

            }

            return true;
        }
        public bool ToCSV(System.Data.DataTable dt, string strFilePath, BO.baseQuery mq)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(strFilePath, false, System.Text.Encoding.UTF8);
            //headers  
            foreach (var col in mq.explicit_columns)
            {
                sw.Write("\"" + col.Header + "\"");
                sw.Write(";");
            }

            sw.Write(sw.NewLine);
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                foreach (var col in mq.explicit_columns)
                {
                    string value = "";

                    if (!Convert.IsDBNull(dr[col.UniqueName]))
                    {
                        value = dr[col.UniqueName].ToString();
                        if (col.FieldType == "string")
                        {
                            value = "\"" + value + "\"";
                        }
                    }
                    sw.Write(value);

                    sw.Write(";");


                }

                sw.Write(sw.NewLine);

            }
            sw.Close();

            return true;
        }
    }
}
