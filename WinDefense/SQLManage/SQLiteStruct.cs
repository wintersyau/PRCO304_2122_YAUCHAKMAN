using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinDefense.ConvertManage;

namespace WinDefense.SQLManage
{
    public static class FileCodeSCanItemExtend
    {
        public static bool InsertDateToDB(this FileCodeSCanItem OneItem)
        {
            string SqlOrder = string.Empty;
            int State = 0;

            int GetCurrentRowid = ConvertHelper.ObjToInt(SQLiteHelper.ExecuteScalar(string.Format("Select Rowid From FileCodeSCan Where KeyStr = '{0}'", OneItem.KeyStr)));

            if (GetCurrentRowid > 0)
            {
                SqlOrder = "UPDate FileCodeSCan Set KeyStr = '{1}',DangerPoint = {2},Describe = '{3}',ShortTittle = '{4}' Where Rowid = {0}";

                State = SQLiteHelper.ExecuteNonQuery(string.Format(SqlOrder, GetCurrentRowid, OneItem.KeyStr, OneItem.DangerPoint, OneItem.Describe, OneItem.ShortTittle));

                if (State == 0 == false)
                {
                    return true;
                }
            }
            else
            {
                SqlOrder = "Insert Into FileCodeSCan(KeyStr,DangerPoint,Describe,ShortTittle)Values('{0}',{1},'{2}','{3}')";

                State = SQLiteHelper.ExecuteNonQuery(string.Format(SqlOrder, OneItem.KeyStr,OneItem.DangerPoint,OneItem.Describe,OneItem.ShortTittle));

                if (State == 0 == false)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool DeleteDateToDB(this FileCodeSCanItem OneItem)
        {
            string SqlOrder = "Delete From FileCodeSCan Where Rowid = {0}";

            int State = SQLiteHelper.ExecuteNonQuery(string.Format(SqlOrder,OneItem.Rowid));

            if (State == 0 == false)
            {
                return true;
            } 

            return false;
        }
    }
    public class SQLiteStruct
    {
        public static List<FileCodeSCanItem> SCanFindBlock(string KeyStr)
        {
            try 
            {

            List<FileCodeSCanItem> FileCodeSCanItems = new List<FileCodeSCanItem>();

            string SqlOrder = "Select * From FileCodeSCan Where KeyStr = '{0}'";

            DataTable NTable = SQLiteHelper.ExecuteQuery(string.Format(SqlOrder, KeyStr));

            if (NTable.Rows.Count > 0)
            {
                for (int i = 0; i < NTable.Rows.Count; i++)
                {
                    FileCodeSCanItems.Add(new FileCodeSCanItem(NTable.Rows[i]["Rowid"], NTable.Rows[i]["KeyStr"], NTable.Rows[i]["DangerPoint"], NTable.Rows[i]["Describe"], NTable.Rows[i]["ShortTittle"]));
                }
            }

            return FileCodeSCanItems;

            }
            catch
            { 
                return new List<FileCodeSCanItem>();
            }
        }

        public static List<FileCodeSCanItem> GetAllBlock()
        {
            try
            {

                List<FileCodeSCanItem> FileCodeSCanItems = new List<FileCodeSCanItem>();

                string SqlOrder = "Select * From FileCodeSCan";

                DataTable NTable = SQLiteHelper.ExecuteQuery(SqlOrder);

                if (NTable.Rows.Count > 0)
                {
                    for (int i = 0; i < NTable.Rows.Count; i++)
                    {
                        FileCodeSCanItems.Add(new FileCodeSCanItem(NTable.Rows[i]["Rowid"], NTable.Rows[i]["KeyStr"], NTable.Rows[i]["DangerPoint"], NTable.Rows[i]["Describe"], NTable.Rows[i]["ShortTittle"]));
                    }
                }

                return FileCodeSCanItems;

            }
            catch
            {
                return new List<FileCodeSCanItem>();
            }
        }
        public static bool AddWhiteList(WhiteListItem OneItem)
        {
            int Rowid = ConvertHelper.ObjToInt(SQLiteHelper.ExecuteScalar(string.Format("Select Rowid From WhiteList Where CRC = '{0}'",OneItem.CRC)));

            if (Rowid > 0)
            {
                string SqlOrder = "UPDate WhiteList Set CRC = '{1}',FileName = '{2}',IsUserPass = '{3}',MD5 = '{4}',FileSize = '{5}' Where Rowid = {0}";

                int State = SQLiteHelper.ExecuteNonQuery(string.Format(SqlOrder, Rowid, OneItem.CRC,OneItem.FileName,OneItem.IsUserPass,OneItem.MD5,OneItem.FileSize));

                if (State == 0 == false)
                {
                    return true;
                }
            }
            else
            {
                string SqlOrder = "Insert Into WhiteList(CRC,FileName,IsUserPass,MD5,FileSize)Values('{0}','{1}','{2}','{3}','{4}')";

                int State = SQLiteHelper.ExecuteNonQuery(string.Format(SqlOrder,OneItem.CRC,OneItem.FileName,OneItem.IsUserPass,OneItem.MD5,OneItem.FileSize));

                if (State == 0 == false)
                {
                    return true;
                }
            }



            return false;
        }
        public static bool DeleteWhiteList(WhiteListItem OneItem)
        {
            string SqlOrder = "Delete From WhiteList Where Rowid = {0}";

            int State = SQLiteHelper.ExecuteNonQuery(string.Format(SqlOrder,OneItem.Rowid));

            if (State == 0 == false)
            {
                return true;
            }

            return false;
        }
        public static WhiteListItem FindWhiteList(string CRC)
        {
            string SqlOrder = "Select * From WhiteList Where CRC = '{0}'";

            DataTable NTable = SQLiteHelper.ExecuteQuery(string.Format(SqlOrder,CRC));

            if (NTable.Rows.Count > 0)
            {
                return new WhiteListItem(NTable.Rows[0]["Rowid"], NTable.Rows[0]["CRC"], NTable.Rows[0]["FileName"], NTable.Rows[0]["IsUserPass"], NTable.Rows[0]["MD5"], NTable.Rows[0]["FileSize"]);
            }

            return null;
        }
    }

    public class WhiteListItem
    {
        public int Rowid = 0;
        public string CRC = "";
        public string FileName = "";
        public string IsUserPass = "";
        public string MD5 = "";
        public string FileSize = "";

        public WhiteListItem(string CRC,string FileName,string IsUserPass,string MD5,string FileSize)
        {
            this.CRC = CRC;
            this.FileName = FileName;
            this.IsUserPass = IsUserPass;
            this.MD5 = MD5;
            this.FileSize = FileSize;
        }
        public WhiteListItem(object Rowid,object CRC,object FileName,object IsUserPass,object MD5,object FileSize)
        {
            this.Rowid = ConvertHelper.ObjToInt(Rowid);
            this.CRC = ConvertHelper.ObjToStr(CRC);
            this.FileName = ConvertHelper.ObjToStr(FileName);
            this.IsUserPass = ConvertHelper.ObjToStr(IsUserPass);
            this.MD5 = ConvertHelper.ObjToStr(MD5);
            this.FileSize = ConvertHelper.ObjToStr(FileSize);
        }


    }
    public class FileCodeSCanItem
    {
        public int Rowid = 0;
        public string KeyStr = "";
        public int DangerPoint = 0;
        public string Describe = "";
        public string ShortTittle = "";

        public FileCodeSCanItem(string KeyStr, int DangerPoint,string Describe,string ShortTittle)
        {
            this.KeyStr = KeyStr;
            this.DangerPoint = DangerPoint;
            this.Describe = Describe;

            this.ShortTittle = ShortTittle;
        }
        public FileCodeSCanItem(object Rowid,object KeyStr,object DangerPoint,object Describe, object ShortTittle)
        {
            this.Rowid = ConvertHelper.ObjToInt(Rowid);
            this.KeyStr = ConvertHelper.ObjToStr(KeyStr);
            this.DangerPoint = ConvertHelper.ObjToInt(DangerPoint);
            this.Describe = ConvertHelper.ObjToStr(Describe);
            this.ShortTittle = ConvertHelper.ObjToStr(ShortTittle);
        }
    }
}
