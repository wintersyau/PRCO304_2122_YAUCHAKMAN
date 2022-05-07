using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinDefense.KernelManage
{
    public class DriveLoader
    {

        public static List<DriveItem> DriveItems = new List<DriveItem>();


        public static DriveItem GetDrive(string DriveName)
        {
            foreach (var GetDrive in DriveItems)
            {
                if (GetDrive.DriveName.ToString().Equals(DriveName))
                {
                    return GetDrive;
                }
            }

            return null;
        }

        public static bool NewDrive(string DriveName)
        {
            foreach (var GetDrive in DriveItems)
            {
                if (GetDrive.DriveName.ToString().Equals(DriveName))
                {
                    return false;
                }
            }

            DriveItems.Add(new DriveItem(DriveName));

            return true;
        }


    }


    public class DriveItem
    {

        public StringBuilder OnePath;

        public StringBuilder DriveName;

        public DriveItem(string DriveName)
        {
            this.OnePath = new StringBuilder(DeFine.GetFullPath(DeFine.DrivePath, DriveName));

            this.DriveName = new StringBuilder(DriveName);

            KernelHelper.installDvr(this.OnePath,this.DriveName);
        }

        public bool StartDrive()
        {
            return KernelHelper.startDvr(DriveName);
        }

        public bool StopDrive()
        {
            return KernelHelper.stopDvr(DriveName);
        }

        public bool CloseDrive()
        {
            return KernelHelper.unloadDvr(DriveName);
        }

    }


}
