#pragma once

#include "pch.h"


#define KernelListenDrive (L"\\\\.\\ProcessListen")

#define KernelShellDrive (L"\\\\.\\Winters1007")

#define KernelProtectPPLDrive (L"\\\\.\\Winters1002")
#define Superkill_NAME "Superkill"
#define Superkill_PATH "..\\Driver\\Tools\\Superkill.sys"
#define ProcessListen_NAME "ProcessListen"
#define ProcessListen_PATH "..\\Driver\\Tools\\ProcessListen.sys"
#define ProcessListen_NAME "PPLLProtect"
#define ProcessListen_PATH "..\\Driver\\Tools\\PPLLProtect.sys"



BOOL installDvr(LPWSTR drvPath, LPWSTR serviceName) {

    SC_HANDLE schSCManager = OpenSCManager(
        NULL,                   // target,NULL：connect to local
        NULL,                   // Serivce database name，NULL：open SERVICES_ACTIVE_DATABASE 
        SC_MANAGER_ALL_ACCESS   // Access right
    );
    if (schSCManager == NULL) {
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    // create service
    SC_HANDLE schService = CreateService(
        schSCManager,               // command
        serviceName,                // Service name to install
        serviceName,                // Service name
        SERVICE_ALL_ACCESS,         // Service access right
        SERVICE_KERNEL_DRIVER,      // Service type
        SERVICE_DEMAND_START,       // StartService start
        SERVICE_ERROR_IGNORE,       // error ignor
        drvPath,                    // path
        NULL,                       // Not own by other
        NULL,                       // Not receive mark
        NULL,                       // Service sequents
        NULL,                       // Account
        NULL                        // LocalSystem password
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

BOOL startDvr(LPWSTR serviceName) {

    SC_HANDLE schSCManager = OpenSCManager(
        NULL,                   
        NULL,                   
        SC_MANAGER_ALL_ACCESS   
    );
    if (schSCManager == NULL) {
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    SC_HANDLE hs = OpenService(
        schSCManager,           
        serviceName,           
        SERVICE_ALL_ACCESS      
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

BOOL stopDvr(LPWSTR serviceName) {

    SC_HANDLE schSCManager = OpenSCManager(
        NULL,                   
        NULL,                   
        SC_MANAGER_ALL_ACCESS   
    );
    if (schSCManager == NULL) {
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    SC_HANDLE hs = OpenService(
        schSCManager,           
        serviceName,            
        SERVICE_ALL_ACCESS      
    );
    if (hs == NULL) {
        CloseServiceHandle(hs);
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    SERVICE_STATUS status;
    if (QueryServiceStatus(hs, &status) == 0) {
        CloseServiceHandle(hs);
        CloseServiceHandle(schSCManager);
        return FALSE;
    }
    if (status.dwCurrentState != SERVICE_STOPPED &&
        status.dwCurrentState != SERVICE_STOP_PENDING
        ) {
        // send closing request
        if (ControlService(
            hs,                         // service command
            SERVICE_CONTROL_STOP,       
            &status                     // status
        ) == 0) {
            CloseServiceHandle(hs);
            CloseServiceHandle(schSCManager);
            return FALSE;
        }

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

// unload driver
BOOL unloadDvr(LPWSTR serviceName) {

    SC_HANDLE schSCManager = OpenSCManager(
        NULL,                   
        NULL,                   
        SC_MANAGER_ALL_ACCESS   
    );
    if (schSCManager == NULL) {
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    SC_HANDLE hs = OpenService(
        schSCManager,           // command
        serviceName,            // service name open
        SERVICE_ALL_ACCESS      // Access right of services
    );
    if (hs == NULL) {
        CloseServiceHandle(hs);
        CloseServiceHandle(schSCManager);
        return FALSE;
    }

    // Delete Service
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

	HANDLE deviceHandle = CreateFile(KernelShellDrive, GENERIC_READ | GENERIC_WRITE,
		0, 0, OPEN_EXISTING, FILE_ATTRIBUTE_SYSTEM, 0);
	if (deviceHandle == INVALID_HANDLE_VALUE)
	{
		DWORD errCode = ::GetLastError();
	}
	else
	{

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

	HANDLE deviceHandle = CreateFile(KernelProtectPPLDrive, GENERIC_READ | GENERIC_WRITE,
		0, 0, OPEN_EXISTING, FILE_ATTRIBUTE_SYSTEM, 0);

	if (deviceHandle == INVALID_HANDLE_VALUE)
	{
		DWORD errCode = ::GetLastError();
	}
	else
	{

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


