using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
//using Newtonsoft.Json;

namespace UI
{
    public static class basUI
    {
       
        public static string render_select_option(string strValue,string strText,string strSelectedValue)
        {
            if (strValue == strSelectedValue)
            {
                return string.Format("<option value='{0}' selected>{1}</option>", strValue, strText);
            }
            else
            {
                return string.Format("<option value='{0}'>{1}</option>", strValue, strText);
            }
            
        }





        public static string getPandulakImage(string strFolderFullPath)
        {
            var files = System.IO.Directory.GetFiles(strFolderFullPath);

            var r = new Random();
            var x = r.Next(1, files.Count());
            return files[x - 1].Split("\\").Last();

        }

        public static bool ResizeImage(string strSourceFullPath, string strDestFullPath,int intMaxWidth, int intMaxHeight)
        {

            System.Drawing.Image imgSource = System.Drawing.Image.FromFile(strSourceFullPath);
            System.Drawing.Imaging.ImageFormat destFormat = imgSource.RawFormat;

            int intW = imgSource.Width;
            int intH = imgSource.Height;
            if (intMaxWidth > 0 && intW > intMaxWidth)
            {
                double dbl = intMaxWidth / System.Convert.ToDouble(intW);
                intW = System.Convert.ToInt32(intW * dbl);
                intH = System.Convert.ToInt32(intH * dbl);
            }
            if (intMaxHeight > 0 && intH > intMaxHeight)
            {
                double dbl = intMaxHeight / System.Convert.ToDouble(intH);
                intW = System.Convert.ToInt32(intW * dbl);
                intH = System.Convert.ToInt32(intH * dbl);
            }

            System.Drawing.Bitmap imgDest = new System.Drawing.Bitmap(intW, intH);

            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(imgDest))
            {
                graphics.DrawImage(imgSource, 0, 0, intW, intH);
            }
            imgDest.Save(strDestFullPath, destFormat);
            return true;
        }

        public static string GetGridUrl(BL.Factory f,string prefix,int pid)
        {            
            if (f.CBL.LoadUserParamBool($"grid-{prefix}-show11", true))
            {
                return $"/TheGrid/MasterView?prefix={prefix}&go2pid={pid}";
            }
            else
            {
                return $"/TheGrid/FlatView?prefix={prefix}&go2pid={pid}";
            }
        }
    }

    
}
