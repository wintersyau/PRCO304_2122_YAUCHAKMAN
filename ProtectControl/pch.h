// pch.h: 这是预编译标头文件。
// 下方列出的文件仅编译一次，提高了将来生成的生成性能。
// 这还将影响 IntelliSense 性能，包括代码完成和许多代码浏览功能。
// 但是，如果此处列出的文件中的任何一个在生成之间有更新，它们全部都将被重新编译。
// 请勿在此处添加要频繁更新的文件，这将使得性能优势无效。

#define PCH_H

// 添加要在此处预编译的标头
#include "framework.h"
#include<windows.h>
#include<string>


// 设备名对应的符号链接名，用于暴露给应用层。符号链接在应用看来是在\\.\ 的



using namespace std;

#ifdef _UNICODE
#define tcout wcout
#define tcerr wcerr
#else
#define tcout cout
#define tcerr cerr
#endif


#define FILE_READ_DATA            ( 0x0001 )    // file & pipe
#define FILE_LIST_DIRECTORY       ( 0x0001 )    // directory
#define FILE_WRITE_DATA           ( 0x0002 )    // file & pipe
#define FILE_ADD_FILE             ( 0x0002 )    // directory
#define FILE_APPEND_DATA          ( 0x0004 )    // file
#define FILE_ADD_SUBDIRECTORY     ( 0x0004 )    // directory
#define FILE_CREATE_PIPE_INSTANCE ( 0x0004 )    // named pipe
#define FILE_READ_EA              ( 0x0008 )    // file & directory
#define FILE_WRITE_EA             ( 0x0010 )    // file & directory
#define FILE_EXECUTE              ( 0x0020 )    // file
#define FILE_TRAVERSE             ( 0x0020 )    // directory
#define FILE_DELETE_CHILD         ( 0x0040 )    // directory
#define FILE_READ_ATTRIBUTES      ( 0x0080 )    // all
#define FILE_WRITE_ATTRIBUTES     ( 0x0100 )    // all
#define FILE_ALL_ACCESS (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x1FF)
#define FILE_DEVICE_SCANNER             0x00000019
#define FILE_DEVICE_SERIAL_MOUSE_PORT   0x0000001a
#define FILE_DEVICE_SERIAL_PORT         0x0000001b
#define FILE_DEVICE_SCREEN              0x0000001c
#define FILE_DEVICE_SOUND               0x0000001d
#define FILE_DEVICE_STREAMS             0x0000001e
#define FILE_DEVICE_TAPE                0x0000001f
#define FILE_DEVICE_TAPE_FILE_SYSTEM    0x00000020
#define FILE_DEVICE_TRANSPORT           0x00000021
#define FILE_DEVICE_UNKNOWN             0x00000022
#define FILE_DEVICE_VIDEO               0x00000023
#define FILE_DEVICE_VIRTUAL_DISK        0x00000024
#define FILE_DEVICE_WAVE_IN             0x00000025
#define FILE_DEVICE_WAVE_OUT            0x00000026
#define FILE_DEVICE_8042_PORT           0x00000027
#define FILE_DEVICE_NETWORK_REDIRECTOR  0x00000028
#define FILE_DEVICE_BATTERY             0x00000029
#define FILE_DEVICE_BUS_EXTENDER        0x0000002a
#define FILE_DEVICE_MODEM               0x0000002b
#define FILE_DEVICE_VDM                 0x0000002c
#define FILE_DEVICE_MASS_STORAGE        0x0000002d
#define FILE_DEVICE_SMB                 0x0000002e
#define FILE_DEVICE_KS                  0x0000002f
#define METHOD_BUFFERED                 0
#define METHOD_IN_DIRECT                1
#define METHOD_OUT_DIRECT               2
#define METHOD_NEITHER                  3


#define CTL_CODE( DeviceType, Function, Method, Access ) (                 \
    ((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method) \
)

typedef void (*ProcessCallbackFunction)(const ULONG A, ULONG B, unsigned short C, unsigned short D, unsigned short E, unsigned short F,BOOLEAN G, BOOLEAN H);


//CTL_CODE创建控制码
#define IOCTL_SEND_AND_REC_STR\
	CTL_CODE(FILE_DEVICE_UNKNOWN\
	, 0x801, METHOD_BUFFERED,\
	FILE_READ_DATA | FILE_WRITE_DATA)


#pragma region ProcessListenHead

#define OPEN_EXISTING       3


#define  CWK_DVC_RECV_STR \
	(ULONG)CTL_CODE( \
	FILE_DEVICE_UNKNOWN, \
	0x912,METHOD_BUFFERED, \
	FILE_READ_DATA)


typedef struct _TIME_FIELDS {
    WORD wYear;
    WORD wMonth;
    WORD wDay;
    WORD wHour;
    WORD wMinute;
    WORD wSecond;
    WORD wMilliseconds;
    WORD wWeekday;
} TIME_FIELDS;

typedef TIME_FIELDS* PTIME_FIELDS;

typedef struct _PROCESSINFO
{
	TIME_FIELDS			time;						// 时间
	BOOLEAN				bIsCreate;					// 是否是创建进程
	HANDLE				hParentProcessId;			// 父进程 ID
	ULONG				ulParentProcessLength;		// 父进程长度
	HANDLE				hProcessId;					// 子进程 ID
	ULONG				ulProcessLength;			// 子进程长度
	ULONG				ulCommandLineLength;		// 进程命令行参数长度
	BOOLEAN             ParentSystem;
	BOOLEAN             ThisSystem;
	UCHAR				uData[1];					// 数据域
} PROCESSINFO, * PPROCESSINFO;
#pragma endregion




extern "C"
{
	_declspec(dllexport) int SendMsgToKernel(LPWSTR pInStr, int len);

	_declspec(dllexport) int SendMsgToKernelByPPL(LPWSTR pInStr, int len);

	_declspec(dllexport) void SetProcessCallback(ProcessCallbackFunction OneCall);

    _declspec(dllexport) BOOL StartProcessListenService(int Check);

    _declspec(dllexport) void ProcessListenServiceLoop();

	_declspec(dllexport) BOOL installDvr(LPWSTR drvPath, LPWSTR serviceName);

	_declspec(dllexport) BOOL startDvr(LPWSTR serviceName);

	_declspec(dllexport) BOOL stopDvr(LPWSTR serviceName);

	_declspec(dllexport) BOOL unloadDvr(LPWSTR serviceName);
};


