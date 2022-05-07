using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinDefense.WinApi
{
    public class WinApiHelper
    {
        #region WinApis

        #region MoreApiTargets

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern void RtlMoveMemory(ref IMAGE_DOS_HEADER Destination, IntPtr Source, int Length);
        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern void RtlMoveMemory(ref IMAGE_NT_HEADERS32 Destination, IntPtr Source, int Length);
        //[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        //public static extern void RtlMoveMemory(ref IMAGE_IMPORT_DESCRIPTOR Destination, ref byte[] Source, int Length);
        //[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        //public static extern void RtlMoveMemory(ref IMAGE_EXPORT_DIRECTORY Destination, ref byte[] Source, int Length);
        //[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        //public static extern void RtlMoveMemory(ref IMAGE_TLS_DIRECTORY32 Destination, ref byte[] Source, int Length);
        //[DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        //public static extern void RtlMoveMemory(ref IMAGE_SECTION_HEADER Destination, ref byte[] Source, int Length);

        #endregion

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern IntPtr GetModuleHandleA(StringBuilder lpModuleName);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(int falg, int id);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int lstrlenA(StringBuilder Ptr);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int Process32First(IntPtr Hwnd, PInfo Pi);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int Process32Next(IntPtr Hwnd, PInfo Pi);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int LoadLibraryA(StringBuilder LibraryName);

        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern int GetProcAddress(IntPtr hModule, StringBuilder lpProcName);

        #endregion


        #region ApiFunctionExtend

        [HandleProcessCorruptedStateExceptions]
        public static object AnyRtlMoveMemoryCall<T>(T Destination, object Source, int Length)
        {
            object ReturnObj = null;

            try
            {
                if (Source is byte[])
                {
                    byte[] GetSource = (byte[])Source;

                    if (Destination is IMAGE_DOS_HEADER)
                    {
                        IMAGE_DOS_HEADER GetType = new IMAGE_DOS_HEADER();

                        unsafe
                        {
                            fixed (byte* OneAdd = GetSource)
                            {
                                RtlMoveMemory(ref GetType,(IntPtr)OneAdd,Length);
                            }
                        }

                        ReturnObj = GetType;
                    }
                    else
                    if (Destination is IMAGE_NT_HEADERS32)
                    {
                        IMAGE_NT_HEADERS32 GetType = new IMAGE_NT_HEADERS32();

                        unsafe
                        {
                            fixed (byte* OneAdd = GetSource)
                            {
                                RtlMoveMemory(ref GetType, (IntPtr)OneAdd, Length);
                            }
                        }
                       
                        ReturnObj = GetType;
                    }
                    //else
                    //if (Destination is IMAGE_IMPORT_DESCRIPTOR)
                    //{
                    //    var GetType = Destination as IMAGE_IMPORT_DESCRIPTOR;
                    //    RtlMoveMemory(ref GetType, ref GetSource, Length);
                    //    ReturnObj = GetType;
                    //}
                    //else
                    //if (Destination is IMAGE_EXPORT_DIRECTORY)
                    //{
                    //    var GetType = Destination as IMAGE_EXPORT_DIRECTORY;
                    //    RtlMoveMemory(ref GetType, ref GetSource, Length);
                    //    ReturnObj = GetType;
                    //}
                    //else
                    //if (Destination is IMAGE_TLS_DIRECTORY32)
                    //{
                    //    var GetType = Destination as IMAGE_TLS_DIRECTORY32;
                    //    RtlMoveMemory(ref GetType, ref GetSource, Length);
                    //    ReturnObj = GetType;
                    //}
                    //else
                    //if (Destination is IMAGE_SECTION_HEADER)
                    //{
                    //    var GetType = Destination as IMAGE_SECTION_HEADER;
                    //    RtlMoveMemory(ref GetType, ref GetSource, Length);
                    //    ReturnObj = GetType;
                    //}

                }
                else
                {

                }
            }
            catch (Exception Ex)
            {

            }

            return ReturnObj;
        }


        public static object RtlMoveMemory<T>(object Source, int Length) where T : new()
        {
            return AnyRtlMoveMemoryCall(new T(), Source, Length);
        }


        #endregion

        public enum RtlType
        {
            Null = 0, IMAGE_DOS_HEADER = 1, IMAGE_NT_HEADERS32 = 2, IMAGE_IMPORT_DESCRIPTOR = 3, IMAGE_EXPORT_DIRECTORY = 4, IMAGE_TLS_DIRECTORY32 = 5, IMAGE_DEBUG_DIRECTORY = 6, IMAGE_SECTION_HEADER = 7
        }

        public class OneRtl
        {
            public RtlType CurrentRtlType;

            public OneRtl(RtlType CurrentRtlType)
            {
                this.CurrentRtlType = CurrentRtlType;
            }
            public object GetInstance(object Source, int Length)
            {
                object GetReturnMsg = null;

                if (this.CurrentRtlType == RtlType.IMAGE_DOS_HEADER)
                {
                    GetReturnMsg = RtlMoveMemory<IMAGE_DOS_HEADER>(Source, Length);
                }
                else
                if (this.CurrentRtlType == RtlType.IMAGE_NT_HEADERS32)
                {
                    GetReturnMsg = RtlMoveMemory<IMAGE_NT_HEADERS32>(Source, Length);
                }
                //else
                //if (this.CurrentRtlType == RtlType.IMAGE_IMPORT_DESCRIPTOR)
                //{
                //    GetReturnMsg = RtlMoveMemory<IMAGE_IMPORT_DESCRIPTOR>(Source, Length);
                //}
                //else
                //if (this.CurrentRtlType == RtlType.IMAGE_EXPORT_DIRECTORY)
                //{
                //    GetReturnMsg = RtlMoveMemory<IMAGE_EXPORT_DIRECTORY>(Source, Length);
                //}
                //else
                //if (this.CurrentRtlType == RtlType.IMAGE_TLS_DIRECTORY32)
                //{
                //    GetReturnMsg = RtlMoveMemory<IMAGE_TLS_DIRECTORY32>(Source, Length);
                //}
                //else
                //if (this.CurrentRtlType == RtlType.IMAGE_DEBUG_DIRECTORY)
                //{
                //    GetReturnMsg = RtlMoveMemory<IMAGE_DEBUG_DIRECTORY>(Source, Length);
                //}
                //else
                //if (this.CurrentRtlType == RtlType.IMAGE_SECTION_HEADER)
                //{
                //    GetReturnMsg = RtlMoveMemory<IMAGE_SECTION_HEADER>(Source, Length);
                //}

                return GetReturnMsg;
            }

        }

        public class PInfo
        {
            public int dwSize;
            public int cntUsage;
            public int Pid;
            public int th32DefaultHeapID;
            public int th32ModuleID;
            public int cntThreads;
            public int th32ParentProcessID;
            public int pcPriClassBase;
            public int dwFlags;
            public StringBuilder PName = new StringBuilder(255);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MODULEENTRY32
        {
            public int dwSize;
            public int th32ModuleID;
            public int th32ProcessID;
            public int GlblcntUsage;
            public int ProccntUsage;
            public IntPtr modBaseAddr;
            public int modBaseSize;
            public IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] szModule;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] szExePath;
            public int dwFlags;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DOS_HEADER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public char[] c_magic;       // Magic number
            public ushort c_cblp;    // Bytes on last page of file
            public ushort c_cp;      // Pages in file
            public ushort c_crlc;    // Relocations
            public ushort c_cparhdr;     // Size of header in paragraphs
            public ushort c_minalloc;    // Minimum extra paragraphs needed
            public ushort c_maxalloc;    // Maximum extra paragraphs needed
            public ushort c_ss;      // Initial (relative) SS value
            public ushort c_sp;      // Initial SP value
            public ushort c_csum;    // Checksum
            public ushort c_ip;      // Initial IP value
            public ushort c_cs;      // Initial (relative) CS value
            public ushort c_lfarlc;      // File address of relocation table
            public ushort c_ovno;    // Overlay number
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public ushort[] c_res1;    // Reserved words
            public ushort c_oemid;       // OEM identifier (for e_oeminfo)
            public ushort c_oeminfo;     // OEM information; e_oemid specific
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public ushort[] c_res2;    // Reserved words
            public int c_lfanew;      // File address of new exe header

            private string _c_magic
            {
                get { return new string(c_magic); }
            }

            public bool isValid
            {
                get { return _c_magic == "MZ"; }
            }

        }


      

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_NT_HEADERS32
        {
            public uint Signature;       // Magic number
            public IMAGE_FILE_HEADER FileHeader;    // Bytes on last page of file
            public IMAGE_OPTIONAL_HEADER32 OptionalHeader;      // Pages in file
        }

        [StructLayout(LayoutKind.Explicit)]
        public class IMAGE_OPTIONAL_HEADERS
        {
            [FieldOffset(0)]
            public MagicType Magic;

            [FieldOffset(2)]
            public byte MajorLinkerVersion;

            [FieldOffset(3)]
            public byte MinorLinkerVersion;

            [FieldOffset(4)]
            public uint SizeOfCode;

            [FieldOffset(8)]
            public uint SizeOfInitializedData;

            [FieldOffset(12)]
            public uint SizeOfUninitializedData;

            [FieldOffset(16)]
            public uint AddressOfEntryPoint;

            [FieldOffset(20)]
            public uint BaseOfCode;

            // PE32 contains this additional field
            [FieldOffset(24)]
            public uint BaseOfData;

            [FieldOffset(28)]
            public uint ImageBase;

            [FieldOffset(32)]
            public uint SectionAlignment;

            [FieldOffset(36)]
            public uint FileAlignment;

            [FieldOffset(40)]
            public ushort MajorOperatingSystemVersion;

            [FieldOffset(42)]
            public ushort MinorOperatingSystemVersion;

            [FieldOffset(44)]
            public ushort MajorImageVersion;

            [FieldOffset(46)]
            public ushort MinorImageVersion;

            [FieldOffset(48)]
            public ushort MajorSubsystemVersion;

            [FieldOffset(50)]
            public ushort MinorSubsystemVersion;

            [FieldOffset(52)]
            public uint Win32VersionValue;

            [FieldOffset(56)]
            public uint SizeOfImage;

            [FieldOffset(60)]
            public uint SizeOfHeaders;

            [FieldOffset(64)]
            public uint CheckSum;

            [FieldOffset(68)]
            public SubSystemType Subsystem;

            [FieldOffset(70)]
            public DllCharacteristicsType DllCharacteristics;

            [FieldOffset(72)]
            public uint SizeOfStackReserve;

            [FieldOffset(76)]
            public uint SizeOfStackCommit;

            [FieldOffset(80)]
            public uint SizeOfHeapReserve;

            [FieldOffset(84)]
            public uint SizeOfHeapCommit;

            [FieldOffset(88)]
            public uint LoaderFlags;

            [FieldOffset(92)]
            public uint NumberOfRvaAndSizes;

            [FieldOffset(96)]
            public IMAGE_DATA_DIRECTORY ExportTable;

            [FieldOffset(104)]
            public IMAGE_DATA_DIRECTORY ImportTable;

            [FieldOffset(112)]
            public IMAGE_DATA_DIRECTORY ResourceTable;

            [FieldOffset(120)]
            public IMAGE_DATA_DIRECTORY ExceptionTable;

            [FieldOffset(128)]
            public IMAGE_DATA_DIRECTORY CertificateTable;

            [FieldOffset(136)]
            public IMAGE_DATA_DIRECTORY BaseRelocationTable;

            [FieldOffset(144)]
            public IMAGE_DATA_DIRECTORY Debug;

            [FieldOffset(152)]
            public IMAGE_DATA_DIRECTORY Architecture;

            [FieldOffset(160)]
            public IMAGE_DATA_DIRECTORY GlobalPtr;

            [FieldOffset(168)]
            public IMAGE_DATA_DIRECTORY TLSTable;

            [FieldOffset(176)]
            public IMAGE_DATA_DIRECTORY LoadConfigTable;

            [FieldOffset(184)]
            public IMAGE_DATA_DIRECTORY BoundImport;

            [FieldOffset(192)]
            public IMAGE_DATA_DIRECTORY IAT;

            [FieldOffset(200)]
            public IMAGE_DATA_DIRECTORY DelayImportDescriptor;

            [FieldOffset(208)]
            public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;

            [FieldOffset(216)]
            public IMAGE_DATA_DIRECTORY Reserved;
        }

        
        public struct IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        }

        public struct IMAGE_OPTIONAL_HEADER32
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public uint BaseOfData;
            public uint ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public uint SizeOfStackReserve;
            public uint SizeOfStackCommit;
            public uint SizeOfHeapReserve;
            public uint SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public IMAGE_DATA_DIRECTORY[] DataDirectory;
        }


        public enum MachineType : ushort
        {
            Native = 0,
            I386 = 0x014c,
            Itanium = 0x0200,
            x64 = 0x8664
        }
        public enum MagicType : ushort
        {
            IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b,
            IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b
        }
        public enum SubSystemType : ushort
        {
            IMAGE_SUBSYSTEM_UNKNOWN = 0,
            IMAGE_SUBSYSTEM_NATIVE = 1,
            IMAGE_SUBSYSTEM_WINDOWS_GUI = 2,
            IMAGE_SUBSYSTEM_WINDOWS_CUI = 3,
            IMAGE_SUBSYSTEM_POSIX_CUI = 7,
            IMAGE_SUBSYSTEM_WINDOWS_CE_GUI = 9,
            IMAGE_SUBSYSTEM_EFI_APPLICATION = 10,
            IMAGE_SUBSYSTEM_EFI_BOOT_SERVICE_DRIVER = 11,
            IMAGE_SUBSYSTEM_EFI_RUNTIME_DRIVER = 12,
            IMAGE_SUBSYSTEM_EFI_ROM = 13,
            IMAGE_SUBSYSTEM_XBOX = 14

        }
        public enum DllCharacteristicsType : ushort
        {
            RES_0 = 0x0001,
            RES_1 = 0x0002,
            RES_2 = 0x0004,
            RES_3 = 0x0008,
            IMAGE_DLL_CHARACTERISTICS_DYNAMIC_BASE = 0x0040,
            IMAGE_DLL_CHARACTERISTICS_FORCE_INTEGRITY = 0x0080,
            IMAGE_DLL_CHARACTERISTICS_NX_COMPAT = 0x0100,
            IMAGE_DLLCHARACTERISTICS_NO_ISOLATION = 0x0200,
            IMAGE_DLLCHARACTERISTICS_NO_SEH = 0x0400,
            IMAGE_DLLCHARACTERISTICS_NO_BIND = 0x0800,
            RES_4 = 0x1000,
            IMAGE_DLLCHARACTERISTICS_WDM_DRIVER = 0x2000,
            IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE = 0x8000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DATA_DIRECTORY
        {
            public uint VirtualAddress;
            public uint Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class IMAGE_EXPORT_DIRECTORY
        {
            public uint Characteristics;
            public uint TimeDateStamp;
            public ushort MajorVersion;
            public ushort MinorVersion;
            public uint Name;
            public uint Base;
            public uint NumberOfFunctions;
            public uint NumberOfNames;
            public uint AddressOfFunctions;     // RVA from base of image
            public uint AddressOfNames;     // RVA from base of image
            public uint AddressOfNameOrdinals;  // RVA from base of image
        }

        [StructLayout(LayoutKind.Explicit)]
        public class IMAGE_NT_HEADERS
        {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public char[] Signature;

            [FieldOffset(4)]
            public IMAGE_FILE_HEADER FileHeader;

            [FieldOffset(24)]
            public IMAGE_OPTIONAL_HEADERS OptionalHeader;

            private string _Signature
            {
                get { return new string(Signature); }
            }

            public bool isValid
            {
                get { return _Signature == "PE\0\0" && (OptionalHeader.Magic == MagicType.IMAGE_NT_OPTIONAL_HDR32_MAGIC || OptionalHeader.Magic == MagicType.IMAGE_NT_OPTIONAL_HDR64_MAGIC); }
            }
        }


    }
}
