#pragma once

#include "pch.h"


#define KernelListenDrive (L"\\\\.\\ProcessListen")

#define KernelShellDrive (L"\\\\.\\Yueding1007")

#define KernelProtectPPLDrive (L"\\\\.\\Yueding1002")


//Superkill.sys 提供内核级对进程的操作 结束进程 挂起恢复进程 以及强制删除文件 和提权等

#define Superkill_NAME "Superkill"
#define Superkill_PATH "..\\Driver\\Tools\\Superkill.sys"

//ProcessListen.sys 是对SecurityCore的一次大改动他主要是监控新创建的文件
//之前的SecurityCore是用objectcallback监控进程的但是这个函数可不仅仅是监控进程(我把问题考虑的太简单了)
//他比预期拿到了更多的数据意味着 我们的软件需要对这些数据进行化验验证安全性但是这是需要时间的
//但是一个问题产生了objectcallback在没有return的时候目标进程是没法启动的
//我们在return中间发送数据给软件软件进行检测 虽然我们用了链表但是我们必须要让他
//对称否则会蓝屏 KeSetEvent 这个函数可以在读取的时候进行加锁也就是读取的时候
//我们是无法做任何多余的操作的 这会导致一个大问题就是死机 目标进程无法启动
//如果是其他软件还好如果是系统进程呢？ 很明显我们不能在这个地方检测进程
//这个sys调用了真正意义的监控进程函数 CreateProcessNotify

#define ProcessListen_NAME "ProcessListen"
#define ProcessListen_PATH "..\\Driver\\Tools\\ProcessListen.sys"


#define ProcessListen_NAME "PPLLProtect"
#define ProcessListen_PATH "..\\Driver\\Tools\\PPLLProtect.sys"



BOOL installDvr(LPWSTR drvPath, LPWSTR serviceName) {

    // 打开服务控制管理器数据库
    SC_HANDLE schSCManager = OpenSCManager(
        NULL,                   // 目标计算机的名称,NULL：连接本地计算机上的服务控制管理器
        NULL,                   // 服务控制管理器数据库的名称，NULL：打开 SERVICES_ACTIVE_DATABASE 数据库
        SC_MANAGER_ALL_ACCESS   // 所有权限
    );
    if (schSCManager == NULL) {
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    // 创建服务对象，添加至服务控制管理器数据库
    SC_HANDLE schService = CreateService(
        schSCManager,               // 服务控件管理器数据库的句柄
        serviceName,                // 要安装的服务的名称
        serviceName,                // 用户界面程序用来标识服务的显示名称
        SERVICE_ALL_ACCESS,         // 对服务的访问权限：所有全权限
        SERVICE_KERNEL_DRIVER,      // 服务类型：驱动服务
        SERVICE_DEMAND_START,       // 服务启动选项：进程调用 StartService 时启动
        SERVICE_ERROR_IGNORE,       // 如果无法启动：忽略错误继续运行
        drvPath,                    // 驱动文件绝对路径，如果包含空格需要多加双引号
        NULL,                       // 服务所属的负载订购组：服务不属于某个组
        NULL,                       // 接收订购组唯一标记值：不接收
        NULL,                       // 服务加载顺序数组：服务没有依赖项
        NULL,                       // 运行服务的账户名：使用 LocalSystem 账户
        NULL                        // LocalSystem 账户密码
    );
    if (schService == NULL) {
        CloseServiceHandle(schService);
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    CloseServiceHandle(schService);
    CloseServiceHandle(schSCManager);
    return TRUE;
}

// 启动服务
BOOL startDvr(LPWSTR serviceName) {

    // 打开服务控制管理器数据库
    SC_HANDLE schSCManager = OpenSCManager(
        NULL,                   // 目标计算机的名称,NULL：连接本地计算机上的服务控制管理器
        NULL,                   // 服务控制管理器数据库的名称，NULL：打开 SERVICES_ACTIVE_DATABASE 数据库
        SC_MANAGER_ALL_ACCESS   // 所有权限
    );
    if (schSCManager == NULL) {
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    // 打开服务
    SC_HANDLE hs = OpenService(
        schSCManager,           // 服务控件管理器数据库的句柄
        serviceName,            // 要打开的服务名
        SERVICE_ALL_ACCESS      // 服务访问权限：所有权限
    );
    if (hs == NULL) {
        CloseServiceHandle(hs);
        CloseServiceHandle(schSCManager);
        return FALSE;
    }
    if (StartService(hs, 0, 0) == 0) {
        CloseServiceHandle(hs);
        CloseServiceHandle(schSCManager);
        return FALSE;
    }


    CloseServiceHandle(hs);
    CloseServiceHandle(schSCManager);
    return TRUE;
}

// 停止服务
BOOL stopDvr(LPWSTR serviceName) {

    // 打开服务控制管理器数据库
    SC_HANDLE schSCManager = OpenSCManager(
        NULL,                   // 目标计算机的名称,NULL：连接本地计算机上的服务控制管理器
        NULL,                   // 服务控制管理器数据库的名称，NULL：打开 SERVICES_ACTIVE_DATABASE 数据库
        SC_MANAGER_ALL_ACCESS   // 所有权限
    );
    if (schSCManager == NULL) {
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    // 打开服务
    SC_HANDLE hs = OpenService(
        schSCManager,           // 服务控件管理器数据库的句柄
        serviceName,            // 要打开的服务名
        SERVICE_ALL_ACCESS      // 服务访问权限：所有权限
    );
    if (hs == NULL) {
        CloseServiceHandle(hs);
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    // 如果服务正在运行
    SERVICE_STATUS status;
    if (QueryServiceStatus(hs, &status) == 0) {
        CloseServiceHandle(hs);
        CloseServiceHandle(schSCManager);
        return FALSE;
    }
    if (status.dwCurrentState != SERVICE_STOPPED &&
        status.dwCurrentState != SERVICE_STOP_PENDING
        ) {
        // 发送关闭服务请求
        if (ControlService(
            hs,                         // 服务句柄
            SERVICE_CONTROL_STOP,       // 控制码：通知服务应该停止
            &status                     // 接收最新的服务状态信息
        ) == 0) {
            CloseServiceHandle(hs);
            CloseServiceHandle(schSCManager);
            return FALSE;
        }

        // 判断超时
        INT timeOut = 0;
        while (status.dwCurrentState != SERVICE_STOPPED) {
            timeOut++;
            QueryServiceStatus(hs, &status);
            Sleep(50);
        }
        if (timeOut > 80) {
            CloseServiceHandle(hs);
            CloseServiceHandle(schSCManager);
            return FALSE;
        }
    }

    CloseServiceHandle(hs);
    CloseServiceHandle(schSCManager);
    return TRUE;
}

// 卸载驱动
BOOL unloadDvr(LPWSTR serviceName) {

    // 打开服务控制管理器数据库
    SC_HANDLE schSCManager = OpenSCManager(
        NULL,                   // 目标计算机的名称,NULL：连接本地计算机上的服务控制管理器
        NULL,                   // 服务控制管理器数据库的名称，NULL：打开 SERVICES_ACTIVE_DATABASE 数据库
        SC_MANAGER_ALL_ACCESS   // 所有权限
    );
    if (schSCManager == NULL) {
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    // 打开服务
    SC_HANDLE hs = OpenService(
        schSCManager,           // 服务控件管理器数据库的句柄
        serviceName,            // 要打开的服务名
        SERVICE_ALL_ACCESS      // 服务访问权限：所有权限
    );
    if (hs == NULL) {
        CloseServiceHandle(hs);
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    // 删除服务
    if (DeleteService(hs) == 0) {
        CloseServiceHandle(hs);
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    CloseServiceHandle(hs);
    CloseServiceHandle(schSCManager);
    return TRUE;
}





char* ConvertLPWSTRToLPSTR(LPWSTR lpwszStrIn)
{
	LPSTR pszOut = NULL;

	if (lpwszStrIn != NULL)
	{
		int nInputStrLen = wcslen(lpwszStrIn);

		// Double NULL Termination  
		int nOutputStrLen = WideCharToMultiByte(CP_ACP, 0, lpwszStrIn, nInputStrLen, NULL, 0, 0, 0) + 2;
		pszOut = new char[nOutputStrLen];

		if (pszOut)
		{
			memset(pszOut, 0x00, nOutputStrLen);
			WideCharToMultiByte(CP_ACP, 0, lpwszStrIn, nInputStrLen, pszOut, nOutputStrLen, 0, 0);
		}
	}


	return pszOut;
}

void TC2C(const PTCHAR tc, char* c)

{

#if defined(UNICODE) 

	WideCharToMultiByte(CP_ACP, 0, tc, -1, c, wcslen(tc), 0, 0);

	c[wcslen(tc)] = 0;

#else 

	lstrcpy((PTSTR)c, (PTSTR)tc);

#endif 


}

int SendMsgToKernel(LPWSTR pInStr, int len)
{
	char* InPutStr = new char[len + 1];

	TC2C(pInStr, InPutStr);

	char* pOutStr = new char[len + 1];

	memset(pOutStr, 0, len + 1);

	int ret_len = 0;
	// 1.打开驱动设备
	HANDLE deviceHandle = CreateFile(KernelShellDrive, GENERIC_READ | GENERIC_WRITE,
		0, 0, OPEN_EXISTING, FILE_ATTRIBUTE_SYSTEM, 0);
	if (deviceHandle == INVALID_HANDLE_VALUE)
	{
		DWORD errCode = ::GetLastError();
	}
	else
	{
		// 2.向驱动设备发送设备控制请求
		if (DeviceIoControl(deviceHandle, IOCTL_SEND_AND_REC_STR,
			InPutStr, len, pOutStr, len + 1, (LPDWORD)&ret_len, NULL))
		{

			return (int)pOutStr;
		}
	}

	return 0;
}

int SendMsgToKernelByPPL(LPWSTR pInStr, int len)
{
	char* InPutStr = new char[len + 1];

	TC2C(pInStr, InPutStr);

	char* pOutStr = new char[len + 1];

	memset(pOutStr, 0, len + 1);

	int ret_len = 0;

	// 1.打开驱动设备

	HANDLE deviceHandle = CreateFile(KernelProtectPPLDrive, GENERIC_READ | GENERIC_WRITE,
		0, 0, OPEN_EXISTING, FILE_ATTRIBUTE_SYSTEM, 0);

	if (deviceHandle == INVALID_HANDLE_VALUE)
	{
		DWORD errCode = ::GetLastError();
	}
	else
	{
		// 2.向驱动设备发送设备控制请求
		if (DeviceIoControl(deviceHandle, IOCTL_SEND_AND_REC_STR,
			InPutStr, len, pOutStr, len + 1, (LPDWORD)&ret_len, NULL))
		{

			return (int)pOutStr;
		}
	}

	return 0;
}


ProcessCallbackFunction OneProcessAction = NULL;

void SetProcessCallback(ProcessCallbackFunction OneCall)
{
	OneProcessAction = OneCall;
}

HANDLE ProcessListenDrive = NULL;

BOOL StartProcessListenService(int Check)
{
	if (Check == 1)
	{
		if (ProcessListenDrive == NULL)
		{
			ULONG bytesReturned;
			/*打开设备，用我们自定的符号链接，响应驱动IRP_MJ_CREATE*/
			ProcessListenDrive = CreateFile(KernelListenDrive,
				GENERIC_READ | GENERIC_WRITE,
				0,
				0,
				OPEN_EXISTING,
				FILE_ATTRIBUTE_NORMAL,
				0);

			if (ProcessListenDrive == INVALID_HANDLE_VALUE)
			{
				return FALSE;
			}

			return TRUE;
		}
	}
	else
	{
		if (ProcessListenDrive != NULL)
		{
			CloseHandle(ProcessListenDrive);
			ProcessListenDrive = NULL;
		}
	}
	

	return FALSE;
}

void ProcessListenServiceLoop()
{
	DWORD ReturnLength = 0;
	PROCESSINFO PLItem;

	memset(&PLItem, 0, sizeof(PROCESSINFO));

	BOOL SendState = DeviceIoControl(
		ProcessListenDrive,
		CWK_DVC_RECV_STR,
		0,
		0,
		&PLItem,
		sizeof(PLItem),
		&ReturnLength,
		0
	);

	if (OneProcessAction != NULL)
	{
		OneProcessAction((ULONG)PLItem.hParentProcessId, (ULONG)PLItem.hProcessId, PLItem.time.wHour,PLItem.time.wMinute,PLItem.time.wSecond,PLItem.time.wMilliseconds,PLItem.ParentSystem,PLItem.ThisSystem);
	}

}


