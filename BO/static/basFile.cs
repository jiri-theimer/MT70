using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;


namespace BO
{
    public class BASFILE
    {
        public static void LogError(string message, string username = null, string procname = null)
        {
            Handle_Log_Write("error", message, username, procname);            
        }
        public static void LogInfo(string message, string username = null, string procname = null)
        {
            Handle_Log_Write("info", message, username, procname);
        }
        private static void Handle_Log_Write (string logname,string message,string username=null,string procname=null)
        {
            var strLogDir = System.IO.Directory.GetCurrentDirectory() + "\\Logs";           
            var strPath = string.Format("{0}\\log-{1}-{2}-{3}.log", strLogDir,logname,username, DateTime.Now.ToString("yyyy.MM.dd"));            
            try
            {
                System.IO.File.AppendAllLines(strPath, new List<string>() { "", "", "------------------------------", DateTime.Now.ToString() });
                if (procname != null)
                {
                    System.IO.File.AppendAllLines(strPath, new List<string>() { "procname: " + procname });
                }
                System.IO.File.AppendAllLines(strPath, new List<string>() { "message: " + message });
            }
            catch
            {
                //nic
            }

        }

        public static FileInfo GetFileInfo(string strFullPath)
        {
            return new FileInfo(strFullPath);
        }

        public static void SaveStream2File(String strDestFullPath, Stream inputStream)
        {
            
            using (FileStream outputFileStream = new FileStream(strDestFullPath, FileMode.Create))
            {
                inputStream.CopyTo(outputFileStream);
            }

            
        }

        public static void AppendText2File(String strDestFullPath, string s)
        {
            File.AppendAllText(strDestFullPath, s);            
        }

        static List<string> GetFileNamesInDir(string strDir, string strPattern,bool getFullPath)
        {
            DirectoryInfo dir = new DirectoryInfo(strDir);
            var lis = new List<string>();
            foreach (FileInfo file in dir.GetFiles(strPattern))
            {
                if (getFullPath == true)
                {
                    lis.Add(file.FullName);
                }
                else
                {
                    lis.Add(file.Name);
                }
                

            }

            return lis;
        }


        public static List<string> GetFileListFromDir(string strDir, string strMask, SearchOption so = SearchOption.TopDirectoryOnly, bool bolRetFullPath = false)
        {
            List<string> lis = new List<string>();
            if (!System.IO.Directory.Exists(strDir))
                return lis;


            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(strDir);

            System.IO.FileInfo[] diar1 = di.GetFiles(strMask, so);
                        
            foreach (System.IO.FileInfo dra in diar1)
            {
                if (bolRetFullPath)
                    lis.Add(dra.FullName);
                else
                    lis.Add(dra.Name);
            }
            return lis;
        }



    }
}
