
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using WinDefense.ConvertManage;

namespace WinDefense.DataManage
{
    public class DataHelper
    {
        private static int CurrentOffset = 0;
        public static byte[] ReadByteByLength(byte[] Source,int Length, int Offset = 0)
        {
            CurrentOffset = Offset;

            if (Length > 0)
            {
                byte[] NewByte = new byte[Length];

                for (int i = 0; i < Length; i++)
                {
                    if (Source.Length - 1 >= (CurrentOffset + i))
                    {
                        NewByte[i] = Source[CurrentOffset + i];
                    }
                    else
                    {
                        return NewByte;
                    }
                }

                return NewByte;
            }

            return null;
        }

        public static string GetPathAndFileName(ref string SourcePath)
        {
            string FileName = SourcePath.Substring(SourcePath.LastIndexOf(@"\") + @"\".Length);
            SourcePath = SourcePath.Substring(0, SourcePath.LastIndexOf(@"\") + @"\".Length);
            return FileName;
        }

        public static double DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            TimeSpan A = new TimeSpan(DateTime1.Ticks);
            TimeSpan B = new TimeSpan(DateTime2.Ticks);
            TimeSpan TS = A - B;
            return TS.TotalMilliseconds;
        }
        public static void WriteFile(string TargetPath, byte[] Data)
        {
            FileStream FS = new FileStream(TargetPath, FileMode.Create);
            FS.Write(Data, 0, Data.Length);
            FS.Close();
            FS.Dispose();
        }
        public static string ShowFileDialog(string Filter)
        {
            //"图像文件|*.jpg|图像文件|*.png|图像文件|*.bmp"
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Filter = Filter;
            if (FileDialog.ShowDialog() == true)
            {
                return FileDialog.FileName;
            }
            return string.Empty;
        }

        public static List<string> ReadFile(string filepath, Encoding EnCoding)
        {
            try
            {
                StreamReader rd = new StreamReader(filepath, EnCoding);

                StringBuilder sb = new StringBuilder();
                while (!rd.EndOfStream)
                {
                    string dqstr = rd.ReadLine();
                    sb = sb.Append(dqstr + "\r\n");
                }

                rd.Close();
                rd.Dispose();
                return StrToList(sb.ToString());
            }
            catch
            {
                return new List<string>();
            }
        }

        public static string ReadFileByStr(string filepath, Encoding EnCoding)
        {
            try
            {
                StreamReader rd = new StreamReader(filepath, Encoding.UTF8);

                StringBuilder sb = new StringBuilder();
                while (!rd.EndOfStream)
                {
                    string dqstr = rd.ReadLine();
                    sb = sb.Append(dqstr + "\r\n");
                }

                rd.Close();
                rd.Dispose();
                return sb.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
        public static byte[] StrToStream(string Str)
        {
            return Encoding.UTF8.GetBytes(Str);
        }


        public static byte[] GetBytesByFilePath(string FilePath)
        {
            byte[] FileData = null;

            if (File.Exists(FilePath))
            {
                using (FileStream Stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader Binary = new BinaryReader(Stream))
                    {
                        FileData = Binary.ReadBytes((int)Stream.Length);
                    }
                }
            }
            else
            {
                return new byte[0];
            }

            return FileData;
        }

        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
        public static void WriteFile(string targetpath, string text, Encoding encodingtype)
        {
            try
            {
                StreamWriter FileWriter = new StreamWriter(targetpath, false, encodingtype);
                FileWriter.Write(text);
                FileWriter.Close();
                FileWriter.Dispose();
            }
            catch { }
        }
        public static void WriteFileAppend(string targetpath, string text, Encoding encodingtype)
        {
            try
            {
                StreamWriter FileWriter = new StreamWriter(targetpath, true, encodingtype);
                FileWriter.Write(text);
                FileWriter.Close();
                FileWriter.Dispose();
            }
            catch { }
        }
        public static bool CreatFile(string targetpath)
        {
            bool iserror = false;
            if (File.Exists(targetpath) == false)
            {
                File.Create(targetpath);
            }
            else
            {
                iserror = true;
            }
            return iserror;
        }

        public static System.Text.Encoding GetFileEncodeType(string filename)
        {
            System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
            Byte[] buffer = br.ReadBytes(2);
            if (buffer[0] >= 0xEF)
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                {
                    fs.Close();
                    br.Close();
                    return System.Text.Encoding.UTF8;
                }
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    fs.Close();
                    br.Close();
                    return System.Text.Encoding.BigEndianUnicode;
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    fs.Close();
                    br.Close();
                    return System.Text.Encoding.Unicode;
                }
                else
                {
                    fs.Close();
                    br.Close();
                    return System.Text.Encoding.Default;
                }
            }
            if (buffer[0] == 0x3c)//utf-8无bom格式
            {
                fs.Close();
                br.Close();
                return System.Text.Encoding.UTF8;
            }

            else
            {
                fs.Close();
                br.Close();
                return System.Text.Encoding.Default;
            }
            fs.Close();
            br.Close();
        }

        /// <summary>
        /// 复制文件夹
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="aimPath"></param>
        public static void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                if (aimPath[aimPath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                {
                    aimPath += System.IO.Path.DirectorySeparatorChar;
                }
                if (!System.IO.Directory.Exists(aimPath))
                {
                    System.IO.Directory.CreateDirectory(aimPath);
                }
                string[] fileList = System.IO.Directory.GetFileSystemEntries(srcPath);
                foreach (string file in fileList)
                {
                    if (System.IO.Directory.Exists(file))
                    {
                        CopyDir(file, aimPath + System.IO.Path.GetFileName(file));
                    }
                    else
                    {
                        System.IO.File.Copy(file, aimPath + System.IO.Path.GetFileName(file), true);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public static string ListToSrt(List<string> obj)
        {
            string RichText = "";
            if (obj.Count == 1)
            {
                return obj[0];
            }
            else
            {
                RichText = string.Join("\r\n", obj.ToArray());
                return RichText;
            }

        }
        public static bool StrToBool(string obj)
        {
            bool NextBool = false;
            bool.TryParse(obj, out NextBool);
            return NextBool;
        }
        public static List<string> StrToList(string obj)
        {
            List<string> AllLine = new List<string>();
            if (obj.Contains("\r\n"))
            {
                AllLine = obj.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else
            {
                AllLine.Add(obj);
            }
            return AllLine;
        }
        public static int StrToInt(string Number)
        {
            int NNumber = 0;
            int.TryParse(Number, out NNumber);
            return NNumber;
        }

        public static void DeleteFolder(string strPath)
        {
            if (Directory.GetDirectories(strPath).Length > 0)
            {
                foreach (string fl in Directory.GetDirectories(strPath))
                {
                    Directory.Delete(fl, true);
                }
            }
            if (Directory.GetFiles(strPath).Length > 0)
            {
                foreach (string f in Directory.GetFiles(strPath))
                {
                    System.IO.File.Delete(f);
                }
            }
            Directory.Delete(strPath, true);
        }

        public static List<FileInformation> GetAllFile(string filepath, List<string> filetype = null)
        {
            DirectoryAllFiles.FileList.Clear();
            List<FileInformation> list = DirectoryAllFiles.GetAllFiles(new System.IO.DirectoryInfo(filepath));
            List<FileInformation> nlist = new List<FileInformation>();

            if (filetype == null == false)
            {
                nlist.AddRange(list);
                foreach (var autoget in list)
                {
                    if (filetype.Contains(autoget.Filetype) == false)
                    {
                        nlist.Remove(autoget);
                    }
                }
                return nlist;
            }
            return list;
        }


    }

    public class DirectoryAllFiles
    {
        public static List<FileInformation> FileList = new List<FileInformation>();
        public static List<FileInformation> GetAllFiles(DirectoryInfo dir)
        {

            List<FileInfo> allFile = new List<FileInfo>(); ;

            try
            {
                allFile = dir.GetFiles().ToList();
            }
            catch { }

            foreach (FileInfo fi in allFile)
            {
                FileList.Add(new FileInformation { FileName = fi.Name, FilePath = fi.FullName, Filetype = fi.Extension });
            }

            List<DirectoryInfo> allDir = new List<DirectoryInfo>();

            try
            {
                allDir = dir.GetDirectories().ToList();
            }
            catch { }

            foreach (DirectoryInfo d in allDir)
            {
                GetAllFiles(d);
            }
            return FileList;
        }
    }
    public class DirectoryAllFilesCode
    {
        public static List<FileInformation> FileList = new List<FileInformation>();
        public static List<FileInformation> GetAllFiles(DirectoryInfo dir, string dqpath)
        {
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                List<string> AllCode = new List<string>();
                AllCode = DataHelper.ReadFile(fi.FullName, Encoding.UTF8);
                FileList.Add(new FileInformation { FileName = fi.Name, FilePath = fi.FullName.Replace(dqpath, ""), Filetype = fi.Extension, FileCode = AllCode });
            }
            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                GetAllFiles(d, dqpath);
            }
            return FileList;
        }
    }

    public class FileInformation
    {
        public string Filetype = "";
        public string FileName = "";
        public string FilePath = "";

        public List<string> FileCode = new List<string>();
    }

    public class DataItem : IComparable<DataItem>
    {
        public string DataPath = "";

        public long LocalTime = 0;

        public long Size = 0;

        public string VirtualPath = "";

        public string DataName = "";

        public int CompareTo(DataItem Other)
        {
            if (this.LocalTime != Other.LocalTime)
            {
                return this.LocalTime.CompareTo(Other.LocalTime);
            }

            else return 0;
        }

        public DataItem(string Content)
        {
            string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            AppData = AppData.Substring(0, AppData.LastIndexOf(@"\") + @"\".Length);

            string GetPath = ConvertHelper.StringDivision(Content, "\"", "\"");

            long Number = 0;

            if (long.TryParse(GetPath, out Number))
            {
                Content = Content.Substring(("\"" + Number + "\"").Length);
                GetPath = ConvertHelper.StringDivision(Content, "\"", "\"");
            }

            string GetInFo = Content.Substring(Content.IndexOf(GetPath) + (GetPath + "\"").Length);

            if (GetInFo.Contains("{")) GetInFo = GetInFo.Split('{')[1];

            foreach (var GetLine in GetInFo.Split(new char[2] { '\r', '\n' }))
            {
                if (GetLine.Contains("\""))
                {
                    string Name = "";
                    string Value = "";

                    foreach (var GetParam in GetLine.Split('"'))
                    {
                        if (GetParam.Trim().Replace(" ", "").Length > 0)
                        {
                            if (Name == "")
                            {
                                Name = GetParam;
                            }
                            else
                            {
                                Value = GetParam;
                                break;
                            }
                        }
                    }

                    if (Name == "size")
                    {
                        this.Size = ConvertHelper.ObjToLong(Value);
                    }
                    if (Name == "localtime")
                    {
                        this.LocalTime = ConvertHelper.ObjToLong(Value);
                    }
                }
            }

            string NewAppPath = AppData + @"Local\" + GetPath.Replace("/", @"\");

            if (File.Exists(NewAppPath))
            {
                this.DataPath = NewAppPath;
                this.DataName = NewAppPath.Substring(this.DataPath.LastIndexOf(@"\") + @"\".Length);
                VirtualPath = @"Local\" + GetPath.Replace("/", @"\");
            }

            NewAppPath = AppData + @"Roaming\" + GetPath.Replace("/", @"\");

            if (File.Exists(NewAppPath))
            {
                this.DataPath = NewAppPath;
                this.DataName = NewAppPath.Substring(this.DataPath.LastIndexOf(@"\") + @"\".Length);
                VirtualPath = @"Roaming\" + GetPath.Replace("/", @"\");
            }

            NewAppPath = AppData + @"LocalLow\" + GetPath.Replace("/", @"\");

            if (File.Exists(NewAppPath))
            {
                this.DataPath = NewAppPath;
                this.DataName = NewAppPath.Substring(this.DataPath.LastIndexOf(@"\") + @"\".Length);
                VirtualPath = @"LocalLow\" + GetPath.Replace("/", @"\");
            }

            NewAppPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\" + GetPath.Replace("/", @"\");
            if (File.Exists(NewAppPath))
            {
                this.DataPath = NewAppPath;
                this.DataName = NewAppPath.Substring(this.DataPath.LastIndexOf(@"\") + @"\".Length);
                VirtualPath = @"\" + GetPath.Replace("/", @"\");
            }
        }
    }


 
}
