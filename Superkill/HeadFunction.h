#pragma once

#include <ntifs.h>

PDEVICE_OBJECT CurrentDrive = NULL;

// 设备名对应的符号链接名，用于暴露给应用层。符号链接一般都是在\??\路径下
#define MY_DEVOBJ_SYB_NAME (L"\\??\\Yueding1007")
// 设备一般都是位于 \Device\这个路径下的
#define MY_DEVOBJ_NAME (L"\\Device\\Yueding1007")

NTKERNELAPI NTSTATUS PsSuspendProcess(PEPROCESS Process);
NTKERNELAPI NTSTATUS PsResumeProcess(PEPROCESS Process);
NTKERNELAPI UCHAR* PsGetProcessImageFileName(__in PEPROCESS Process);

NTKERNELAPI HANDLE PsGetProcessInheritedFromUniqueProcessId(IN PEPROCESS Process);//未公开

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

PEPROCESS PidToEprocess(ULONG Pid)
{
	PEPROCESS pEProc;
	PsLookupProcessByProcessId((HANDLE)Pid, &pEProc);
	ObDereferenceObject(pEProc);
	return pEProc;
}

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
		KeStackAttachProcess(Eprocess, PKAPC_STATEpKs);//Attach进程虚拟空间
		for (i = 0; i <= 0x7fffffff; i += 0x1000)
		{
			if (MmIsAddressValid((PVOID)i))
			{
				_try
				{
				   ProbeForWrite((PVOID)i,0x1000,sizeof(ULONG));
				   memset((PVOID)i,0xcc,0x1000);
				}_except(1) { continue; }
			}
			else {
				if (i > 0x1000000)  //填这么多足够破坏进程数据了  
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

//64位别用这个直接蓝屏

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
		State = PsResumeProcess(CurrentProcess);//回复
	}
	else
	{
		State = PsSuspendProcess(CurrentProcess);//挂起
	}
}

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
	Status = NtAdjustPrivilegesToken(Token, 0, &_TP, sizeof(_TP), NULL, &_TP.Privileges[0].Luid.LowPart);

	return;
}

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

int Getlen(const char* str)
{
	int len = 0;

	while (*str++ != '\0')
		len++;
	return len;
}

int Kernel_Atoi(const char* s)
{
	BOOLEAN negative = FALSE;
	int result = 0;//存放中间变量
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
			DbgPrint("TargetProcess:%i", Target);
			SetProcessLevel(Target, SE_DEBUG_PRIVILEGE);
		}
		else
			if (GetFristChar == 'K')
			{
				DbgPrint("TargetProcess:%i", Target);
				KillProcessByTerminate(Target);
			}
		if (GetFristChar == 'Z')
		{
			DbgPrint("TargetProcess:%i", Target);
			ZeroKill(Target);
		}
		else
			if (GetFristChar == 'S')
			{
				DbgPrint("TargetProcess:%i", Target);
				PauseAndKeepProcess(Target, 1);
			}
			else
				if (GetFristChar == 'M')
				{
					DbgPrint("TargetProcess:%i", Target);
					PauseAndKeepProcess(Target, 0);
				}
	}
	else
	{
		
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
			//KdBreakPoint();// 断点设置，使用windbg调试时可以打开

			//控制码由这个宏函数CTL_CODE创建
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
				// 其他控制码请求，一律返回非法参数错误。
				Status = STATUS_INVALID_PARAMETER;
			}
		}
	}

	// KdBreakPoint(); // 断点设置，使用windbg调试时可以打开

	// 第4步，结束请求
	// 这个Informatica用来记录这次返回到底使用了多少输出空间
	Irp->IoStatus.Information = ret_len;
	// 用于记录这个请求的完成状态
	Irp->IoStatus.Status = Status;
	// 用于结束这个请求
	IoCompleteRequest(Irp, IO_NO_INCREMENT);
	return Status;
}
