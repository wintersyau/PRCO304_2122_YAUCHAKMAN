using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinDefense.SQLManage;

namespace WinDefense.SafeEngine
{

    public static class StringExtend
    {
        public static string FileNameToFilePath(this string FullFileName)
        {
            if (FullFileName.Contains("."))
            {
                return FullFileName.Substring(0, FullFileName.LastIndexOf(@"\"));
            }

            return string.Empty;
        }

    }
    public class SafeExtend
    {

        public static void ScanFileByProcessPath(string SourcePath, ref List<FileCodeSCanItem> FileCodeItem)
        {
            string GetSourceName = SourcePath.Substring(SourcePath.LastIndexOf(@"\") + @"\".Length);
            GetSourceName = GetSourceName.Split('.')[0];


            foreach (var GetFileItem in Directory.GetFiles(SourcePath.FileNameToFilePath()))
            {
                if (GetFileItem.ToLower().EndsWith(".dll"))
                {
                    if (GetFileItem.Split('.')[0].ToLower().Equals(GetSourceName.ToLower()))
                    {
                        //Scans and detects a dll file inside the project and a file name as EXE

                        var TempSign = SafeHelper.NewSCan(GetFileItem);


                        foreach (var Get in TempSign)
                        {
                            if (!FileCodeItem.Contains(Get))
                            {
                                FileCodeItem.Add(Get);
                            }
                        }

                        //Returns dll feature information to FileCodeItem
                       
                    }
                }
            
            }
   
        
        
        }
            
    }
}
