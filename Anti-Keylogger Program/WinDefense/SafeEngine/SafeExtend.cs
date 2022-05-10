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
            var TempSign = SafeHelper.NewSCan(SourcePath);

            foreach (var Get in TempSign)
            {
                if (!FileCodeItem.Contains(Get))
                {
                    FileCodeItem.Add(Get);
                }
            }
        }

    }
}
