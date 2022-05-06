#pragma once

#include <ntifs.h>

PDEVICE_OBJECT CurrentDrive = NULL;


#define MY_DEVOBJ_SYB_NAME (L"\\??\\Winters1007")
#define MY_DEVOBJ_NAME (L"\\Device\\Winters1007")

NTKERNELAPI NTSTATUS PsSuspendProcess(PEPROCESS Process);
NTKERNELAPI NTSTATUS PsResumeProcess(PEPROCESS Process);
NTKERNELAPI UCHAR* PsGetProcessImageFileName(__in PEPROCESS Process);
NTKERNELAPI HANDLE PsGetProcessInheritedFromUniqueProcessId(IN PEPROCESS Process);

#define IOCTL_SEND_AND_REC_STR\
	CTL_CODE(FILE_DEVICE_UNKNOWN\
	, 0x801, METHOD_BUFFERED,\
	FILE_READ_DATA | FILE_WRITE_DATA)

#define PROCESS_TERMINATE         0x0001  
#define PROCESS_VM_OPERATION      0x0008  
#define PROCESS_VM_READ           0x0010  
#define PROCESS_VM_WRITE          0x0020  

#define Sys_Version "2.1.12"



typedef struct _LDR_DATA_TABLE_ENTRY64
{
	LIST_ENTRY64    InLoadOrderLinks;
	LIST_ENTRY64    InMemoryOrderLinks;
	LIST_ENTRY64    InInitializationOrderLinks;
	PVOID            DllBase;
	PVOID            EntryPoint;
	ULONG            SizeOfImage;
	UNICODE_STRING    FullDllName;
	UNICODE_STRING     BaseDllName;
	ULONG            Flags;
	USHORT            LoadCount;
	USHORT            TlsIndex;
	PVOID            SectionPointer;
	ULONG            CheckSum;
	PVOID            LoadedImports;
	PVOID            EntryPointActivationContext;
	PVOID            PatchInformation;
	LIST_ENTRY64    ForwarderLinks;
	LIST_ENTRY64    ServiceTagLinks;
	LIST_ENTRY64    StaticLinks;
	PVOID            ContextInformation;
	ULONG64            OriginalBase;
	LARGE_INTEGER    LoadTime;
} LDR_DATA_TABLE_ENTRY64, * PLDR_DATA_TABLE_ENTRY64;



#pragma region ProcessProtectHead

/// <summary>
/// Thread Id convert to handle
/// </summary>
/// <param name="Pid"></param>
/// <returns></returns>
PHANDLE PidToHandle(ULONG Pid)
{
	OBJECT_ATTRIBUTES ObjectAttributes;
	CLIENT_ID Clientid;
	InitializeObjectAttributes(&ObjectAttributes, 0, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, 0, 0);
	Clientid.UniqueProcess = (HANDLE)Pid;
	Clientid.UniqueThread = 0;

	PHANDLE ThisHandle = 100;

	ZwOpenProcess(ThisHandle, PROCESS_ALL_ACCESS, &ObjectAttributes, &Clientid);

	return ThisHandle;
}

/// <summary>
/// Thread ID convert to C++ Eprocess (KMDF Type)
/// </summary>
/// <param name="Pid"></param>
/// <returns></returns>
PEPROCESS PidToEprocess(ULONG Pid)
{
	PEPROCESS pEProc;
	PsLookupProcessByProcessId((HANDLE)Pid, &pEProc);
	ObDereferenceObject(pEProc);
	return pEProc;
}

/// <summary>
/// Kernel kill the process (Target Ram Delete)
/// </summary>
/// <param name="PID"></param>
/// <returns></returns>
BOOLEAN ZeroKill(ULONG PID)   //X32  X64
{
	DbgPrint("KillProcess:%i", PID);

	NTSTATUS ntStatus = STATUS_SUCCESS;
	int i = 0;
	PVOID handle;
	PEPROCESS Eprocess;
	ntStatus = PsLookupProcessByProcessId(PID, &Eprocess);
	if (NT_SUCCESS(ntStatus))
	{
		PKAPC_STATE PKAPC_STATEpKs = (PKAPC_STATE)ExAllocatePool(NonPagedPool, sizeof(PKAPC_STATE));
		KeStackAttachProcess(Eprocess, PKAPC_STATEpKs);//Attach target handle 
		for (i = 0; i <= 0x7fffffff; i += 0x1000) //Enumeration Ram Address IntPtr.Zero To 0x7fffffff 
		{
			if (MmIsAddressValid((PVOID)i))
			{
				_try
				{
				   ProbeForWrite((PVOID)i,0x1000,sizeof(ULONG));
				   memset((PVOID)i,0xcc,0x1000); //Set addresss =  0XCC
				}_except(1) { continue; }
			}
			else {
				if (i > 0x1000000)  //Write size limit
					break;
			}
		}
		KeUnstackDetachProcess(PKAPC_STATEpKs);
		if (ObOpenObjectByPointer((PVOID)Eprocess, 0, NULL, 0, NULL, KernelMode, &handle) != STATUS_SUCCESS)
			return FALSE;
		ZwTerminateProcess((HANDLE)handle, STATUS_SUCCESS);
		ZwClose((HANDLE)handle);
		return TRUE;
	}
	return FALSE;
}

/// <summary>
/// Kernel terminate process (KMDF Process Terminate)
/// </summary>
/// <param name="Pid"></param>

void KillProcessByTerminate(ULONG Pid)
{
	NTSTATUS status;
	OBJECT_ATTRIBUTES objAttr = { 0 };
	CLIENT_ID  Client = { 0 };
	PHANDLE CurrentProcess = PidToHandle(Pid);

	if (CurrentProcess != 0)
	{
		status = ZwOpenProcess(CurrentProcess, PROCESS_ALL_ACCESS, &objAttr, &Client);
		if (NT_SUCCESS(status))
		{
			DbgPrint("KillProcess:%i", Pid);

			ZwTerminateProcess(CurrentProcess, 0);
			ZwClose(CurrentProcess);
		}
	}

}


/// <summary>
/// 
/// </summary>
/// <param name="Pid"></param>
void KillProcessByZeroMemory(ULONG Pid)
{
	NTSTATUS status;
	PEPROCESS Process;
	ULONG64  Address;

	PHANDLE CurrentProcess = PidToHandle(Pid);

	if (CurrentProcess != 0)
	{
		status = PsLookupProcessByProcessId(CurrentProcess, &Process);

		if (NT_SUCCESS(status))
		{
			KeAttachProcess(Process);
			for (Address = 0; Address <= 0x80000000; Address += PAGE_SIZE)
			{
				_try
				{
					memset((void*)Address, 0, PAGE_SIZE);

				}_except(0)
				{

				}

			}

			ObDereferenceObject(Process);
		}
	}
}

void TryWriteProcessMemory(ULONG Pid, void* Address, int Value)
{
	NTSTATUS status;
	PEPROCESS Process;

	PHANDLE CurrentProcess = PidToHandle(Pid);

	if (CurrentProcess != 0)
	{
		status = PsLookupProcessByProcessId(CurrentProcess, &Process);

		if (NT_SUCCESS(status))
		{
			KeAttachProcess(Process);

			_try
			{
				memset(Address,Value,sizeof(Address));

			}_except(0)
			{

			}

			ObDereferenceObject(Process);
		}
	}
}

INT PauseAndKeepProcess(ULONG Pid, INT Check)
{
	PEPROCESS CurrentProcess = PidToEprocess(Pid);
	NTSTATUS State = 0;

	if (Check == 0)
	{
		State = PsResumeProcess(CurrentProcess);//Kernel pause process
	}
	else
	{
		State = PsSuspendProcess(CurrentProcess);//Kernel suspend process
	}
}


/// <summary>
/// Kernel Set Process Access
/// </summary>
/// <param name="Pid"></param>
/// <param name="Level"></param>

void SetProcessLevel(ULONG Pid, ULONG Level)
{
	HANDLE Token = 0;

	PHANDLE CurrentProcess = PidToHandle(Pid);

	NTSTATUS Status = ZwOpenProcessTokenEx(CurrentProcess, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, OBJ_KERNEL_HANDLE, &Token);	//Open the access token associated with a process
	//"ZwOpenProcessTokenEx Succeed!"
	TOKEN_PRIVILEGES _TP;
	_TP.PrivilegeCount = 1;
	_TP.Privileges[0].Luid.HighPart = 0;
	_TP.Privileges[0].Luid.LowPart = Level;
	_TP.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

	PULONG pLength = 0;
	Status = NtAdjustPrivilegesToken(Token, 0, &_TP, sizeof(_TP), NULL, &_TP.Privileges[0].Luid.LowPart);//One Access Has Benn Using

	return;
}

/// <summary>
/// Kernel delete file
/// </summary>
/// <param name="uFilePath"></param>
/// <returns></returns>
NTSTATUS FileDelete(UNICODE_STRING uFilePath)
{
	NTSTATUS status = STATUS_SUCCESS;

	OBJECT_ATTRIBUTES objectAttributes = { 0 };

	InitializeObjectAttributes(&objectAttributes, &uFilePath, OBJ_CASE_INSENSITIVE, NULL, NULL);

	status = ZwDeleteFile(&objectAttributes);

	if (status == STATUS_SUCCESS)

		DbgPrint("FileDelete -> DeleteFile success");

	else

		DbgPrint("FileDelete -> DeleteFile failed");

	return status;
}

#pragma endregion

/// <summary>
/// Get String Length
/// </summary>
/// <param name="str"></param>
/// <returns></returns>
int Getlen(const char* str)
{
	int len = 0;

	while (*str++ != '\0')
		len++;
	return len;
}

/// <summary>
/// String To Integer
/// </summary>
/// <param name="s"></param>
/// <returns></returns>
int Kernel_Atoi(const char* s)
{
	BOOLEAN negative = FALSE;
	int result = 0;
	int len = Getlen(s);
	int limit = -2147483647;
	int i = 0;
	if (len > 0) {
		if (s[0] < '0') {
			if ('-' == s[0]) {
				negative = TRUE;
				limit = -2147483648;
			}
			else if ('+' != s[0]) {

			}

			if (len == 1) {

			}
			i++;
		}
		while (i < len) {
			int digit = s[i++] - '0';
			if (digit < 0 || digit>9) {

			}
			else {
				result *= 10;
				if (result - digit < limit) {

				}
				result -= digit;
			}
		}
	}
	else {

	}
	return negative ? result : -result;
}

/// <summary>
/// Check Ring3 Program Order
/// </summary>
/// <param name="Buffer"></param>
void ProcessCommand(PVOID Buffer)
{
	int bi = 0;
	int i = 0;

	char* GetAdd = (char*)Buffer;
	char GetFristChar = GetAdd[0];

	if (GetFristChar != 'F')
	{
		char Cache[25];

		while (*GetAdd)
		{
			GetAdd++;
			Cache[i] = *GetAdd;
			i++;
		}

		ULONG Target = Kernel_Atoi(Cache);

		if (GetFristChar == 'U')
		{
			//Process UPAccess U{PID}
			DbgPrint("TargetProcess:%i", Target);
			SetProcessLevel(Target, SE_DEBUG_PRIVILEGE);
		}
		else
			if (GetFristChar == 'K')
			{
				//Process Kill Z{PID} (Process) Terminate Process Mode1
				DbgPrint("TargetProcess:%i", Target);
				KillProcessByTerminate(Target);
			}
		if (GetFristChar == 'Z')
		{
			//Process RamErase Z{PID} Terminate Process Mode2
			DbgPrint("TargetProcess:%i", Target);
			ZeroKill(Target);
		}
		else
			if (GetFristChar == 'S')
			{
				//Pause Process S{PID}
				DbgPrint("TargetProcess:%i", Target);
				PauseAndKeepProcess(Target, 1);
			}
			else
				if (GetFristChar == 'M')
				{
					//Keep Process M{PID}
					DbgPrint("TargetProcess:%i", Target);
					PauseAndKeepProcess(Target, 0);
				}
	}
	else
	{
		//File Delete F{Path}
		char BigCache[255];

		while (*GetAdd)
		{
			GetAdd++;
			BigCache[i] = *GetAdd;
			bi++;
		}

		ANSI_STRING ansiString;
		UNICODE_STRING uniString;

		PWCHAR buffer;

		ULONG length = strlen(BigCache) * sizeof(WCHAR) + sizeof(WCHAR);

		buffer = ExAllocatePool(PagedPool, length);

		RtlInitAnsiString(&ansiString, BigCache);

		uniString.Length = 0;
		uniString.MaximumLength = (USHORT)length;
		uniString.Buffer = buffer;

		RtlAnsiStringToUnicodeString(
			&uniString,
			&ansiString,
			FALSE);

		FileDelete(uniString);
	}


}

/// <summary>
/// Listen IOControl Message
/// </summary>
/// <param name="DeviceObject"></param>
/// <param name="Irp"></param>
/// <returns></returns>
NTSTATUS MyDispatch(IN PDEVICE_OBJECT DeviceObject, IN PIRP Irp)
{
	NTSTATUS Status = STATUS_SUCCESS;
	ULONG ret_len = 0, i = 0, temp = 0;
	if (DeviceObject == CurrentDrive)
	{
		PIO_STACK_LOCATION irpStack = IoGetCurrentIrpStackLocation(Irp);

		if (irpStack->MajorFunction == IRP_MJ_CREATE ||
			irpStack->MajorFunction == IRP_MJ_CLOSE)
		{
			Status = STATUS_SUCCESS;
		}

		else if (irpStack->MajorFunction == IRP_MJ_DEVICE_CONTROL)
		{
			PVOID buffer = Irp->AssociatedIrp.SystemBuffer;
			ULONG inLen = irpStack->Parameters.DeviceIoControl.InputBufferLength;
			ULONG outLen = irpStack->Parameters.DeviceIoControl.OutputBufferLength;
			//KdBreakPoint(); //Set One BreakPoint To WinDbg

			//Check Control Code == IOCTL_SEND_AND_REC_STR
			if (irpStack->Parameters.DeviceIoControl.IoControlCode == IOCTL_SEND_AND_REC_STR)
			{
				if (inLen > 0)
				{
					ProcessCommand(buffer);

					DbgPrint(((char*)buffer));

					if (outLen >= inLen)
					{
						ret_len = inLen;

						for (; i <= inLen; i++)
						{
							temp = (ULONG)((char*)buffer)[i];
							if (temp >= 97 && temp <= 122)
							{
								((char*)buffer)[i] -= 32;
							}
						}
					}
					else
					{
						Status = STATUS_INVALID_PARAMETER;
					}
				}
				else
				{
					Status = STATUS_INVALID_PARAMETER;
				}

			}
			else
			{
				// IS INVALID PARAMETER
				Status = STATUS_INVALID_PARAMETER;
			}
		}
	}

	KdBreakPoint(); //Set breakPoint to WinDbg

   
   // This information return the cache sizes
	Irp->IoStatus.Information = ret_len;
	// Recv Io complete state 
	Irp->IoStatus.Status = Status;
	// End request
	IoCompleteRequest(Irp, IO_NO_INCREMENT);
	return Status;
}
