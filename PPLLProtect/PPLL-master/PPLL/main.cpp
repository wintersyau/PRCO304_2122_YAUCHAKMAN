#include "utils.hpp"

PVOID ProcessListenThread;


#define PROCESS_TERMINATE         0x0001  
#define PROCESS_VM_OPERATION      0x0008  
#define PROCESS_VM_READ           0x0010  
#define PROCESS_VM_WRITE          0x0020  


NTKERNELAPI
BOOLEAN
PsIsSystemProcess(
	_In_ PEPROCESS Process
);

BOOLEAN IsBlue = false;
ULONG SignatureLevelOffset{}, SectionSignatureLevelOffset{};

PDEVICE_OBJECT CurrentDrive = NULL;

// The Symbolic Link name is correspond to the device Name.  It was used to expose the Applicaiton Layer.
#define MY_DEVOBJ_SYB_NAME (L"\\??\\Winters1002")
#define MY_DEVOBJ_NAME (L"\\Device\\Winters1002")


//CTL_CODE Control Code DeFine
#define IOCTL_SEND_AND_REC_STR\
	CTL_CODE(FILE_DEVICE_UNKNOWN\
	, 0x801, METHOD_BUFFERED,\
	FILE_READ_DATA | FILE_WRITE_DATA)


typedef struct _DEVICE_EXTENSION {
	PDEVICE_OBJECT pDevice;
	UNICODE_STRING ustrDeviceName;	//Driver Name
	UNICODE_STRING ustrSymLinkName;	///Driver Link Name
} DEVICE_EXTENSION, * PDEVICE_EXTENSION;  //Extend Module


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


PHANDLE PidToHandle(ULONG Pid)
{
	OBJECT_ATTRIBUTES ObjectAttributes;
	CLIENT_ID Clientid;
	InitializeObjectAttributes(&ObjectAttributes, 0, OBJ_CASE_INSENSITIVE | OBJ_KERNEL_HANDLE, 0, 0);
	Clientid.UniqueProcess = (HANDLE)Pid;
	Clientid.UniqueThread = 0;

	PHANDLE ThisHandle = (PHANDLE)100;

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

extern "C"
HANDLE PidToHandleA(ULONG Pid)
{
	return (HANDLE)Pid;
}

int Getlen(const char* str)
{
	int len = 0;

	while (*str++ != '\0')
		len++;
	return len;
}

//String convert to Integer
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

//Set PPLL Protect To Handle
VOID SetPPLProtect(HANDLE OneHandle)
{
	PEPROCESS Process;
	PsLookupProcessByProcessId(OneHandle, &Process);

	HANDLE hThread{};
	PsCreateSystemThread(&hThread, THREAD_ALL_ACCESS, NULL, NULL, NULL, [](PVOID StartContext)
		{
			//Set Process Access
			Sleep(1000);
			if (!SectionSignatureLevelOffset)
			{
				PULONG pFlags2 = (PULONG)(((ULONG_PTR)StartContext) + SignatureLevelOffset);
				*pFlags2 |= PROTECTED_PROCESS_MASK;
			}
			else
			{
				PPROCESS_SIGNATURE_PROTECTION pSignatureProtect = (PPROCESS_SIGNATURE_PROTECTION)(((ULONG_PTR)StartContext) + SignatureLevelOffset);
				//Full Protect
				pSignatureProtect->SignatureLevel = IsBlue ? 0x0F : 0x3F;
				pSignatureProtect->SectionSignatureLevel = IsBlue ? 0x0F : 0x3F;
				if (!IsBlue)
				{
					pSignatureProtect->Protection.Type = 2;
					pSignatureProtect->Protection.Audit = 0;
					pSignatureProtect->Protection.Signer = 6;
				}
			}
		}
	, Process);
}

/// <summary>
/// Cancel the target handle PPLLProtect
/// </summary>
/// <param name="OneHandle"></param>
VOID UNPPLProtect(HANDLE OneHandle)
{
	PEPROCESS Process;
	PsLookupProcessByProcessId(OneHandle, &Process);

	HANDLE hThread{};
	PsCreateSystemThread(&hThread, THREAD_ALL_ACCESS, NULL, NULL, NULL, [](PVOID StartContext)
		{
			Sleep(1000);
			if (!SectionSignatureLevelOffset)
			{
				PULONG pFlags2 = (PULONG)(((ULONG_PTR)StartContext) + SignatureLevelOffset);
				*pFlags2 |= PROTECTED_PROCESS_MASK;
			}
			else
			{
				PPROCESS_SIGNATURE_PROTECTION pSignatureProtect = (PPROCESS_SIGNATURE_PROTECTION)(((ULONG_PTR)StartContext) + SignatureLevelOffset);
				pSignatureProtect->SignatureLevel = true ? 0x0F : 0x3F;
				pSignatureProtect->SectionSignatureLevel = true ? 0x0F : 0x3F;
			}
		}
	, Process);
}

/// <summary>
/// PID To ProcessName
/// </summary>
/// <param name="ulProcessID"></param>
/// <returns></returns>
char* GetProcessImageNameByProcessID(ULONG ulProcessID)
{
	NTSTATUS  Status;
	PEPROCESS  EProcess = NULL;


	Status = PsLookupProcessByProcessId((HANDLE)ulProcessID, &EProcess);    //EPROCESS

	//Testing the effectiveness of PsLookupProcess by ProcessId Call State
	if (!NT_SUCCESS(Status))
	{
		return FALSE;
	}
	ObDereferenceObject(EProcess);
	//Return ProcessName
	return (char*)PsGetProcessImageFileName(EProcess);
}

/// <summary>
///  Listen NTOpenProcess Message
/// </summary>
/// <param name="RegistrationContext"></param>
/// <param name="pOperationInformation"></param>
/// <returns></returns>

OB_PREOP_CALLBACK_STATUS OneProcessAction(PVOID RegistrationContext, POB_PRE_OPERATION_INFORMATION pOperationInformation)
{
	HANDLE CurrentPID = PsGetProcessId((PEPROCESS)pOperationInformation->Object);

	char szProcName[25] = { 0 };

	UNREFERENCED_PARAMETER(RegistrationContext);

	strcpy(szProcName, GetProcessImageNameByProcessID((ULONG)CurrentPID));

	if (!_stricmp(szProcName, "WinDefense.exe"))//Protect WinDefense.exe 
	{
		if (pOperationInformation->Operation == OB_OPERATION_HANDLE_CREATE)
		{
			BOOLEAN State = FALSE;

			if (pOperationInformation->Operation == OB_OPERATION_HANDLE_CREATE)
			{
				if ((pOperationInformation->Parameters->CreateHandleInformation.OriginalDesiredAccess & PROCESS_TERMINATE) == PROCESS_TERMINATE)
				{
					State = TRUE;
					//Terminate the process by calling the user-mode TerminateProcess routine.
					pOperationInformation->Parameters->CreateHandleInformation.DesiredAccess &= ~PROCESS_TERMINATE;
				}
				if ((pOperationInformation->Parameters->CreateHandleInformation.OriginalDesiredAccess & PROCESS_VM_OPERATION) == PROCESS_VM_OPERATION)
				{
					State = TRUE;
					//Modify the address space of the process by calling the user-mode WriteProcessMemory and VirtualProtectEx routines.
					pOperationInformation->Parameters->CreateHandleInformation.DesiredAccess &= ~PROCESS_VM_OPERATION;
				}
				if ((pOperationInformation->Parameters->CreateHandleInformation.OriginalDesiredAccess & PROCESS_VM_READ) == PROCESS_VM_READ)
				{
					State = TRUE;
					//Read the address space of the Processs by calling the user-mode ReadProcessMemory routine.
					pOperationInformation->Parameters->CreateHandleInformation.DesiredAccess &= ~PROCESS_VM_READ;
				}
				if ((pOperationInformation->Parameters->CreateHandleInformation.OriginalDesiredAccess & PROCESS_VM_WRITE) == PROCESS_VM_WRITE)
				{
					State = TRUE;
					//Write to the address space of the Process by calling the user-mode WriteProcessMemory routine.
					pOperationInformation->Parameters->CreateHandleInformation.DesiredAccess &= ~PROCESS_VM_WRITE;
				}
			}

			if (State)
			{
				DbgPrint("ProcessProtect> PID:%ld \r\n", (ULONG64)CurrentPID);
			}
		}
	}
	else
	{

	}

	return OB_PREOP_SUCCESS;
}

/// <summary>
/// Start NT protect process
/// </summary>
/// <param name="Enable"></param>
/// <returns></returns>

NTSTATUS ProtectProcess(BOOLEAN Enable)
{
	NTSTATUS  Status;

	if (Enable)
	{
		DbgPrint("ProtectServiceInit");

		OB_CALLBACK_REGISTRATION CallBackReg;
		OB_OPERATION_REGISTRATION OpOperAtion;

		memset(&CallBackReg, 0, sizeof(CallBackReg));
		CallBackReg.Version = ObGetFilterVersion();
		CallBackReg.OperationRegistrationCount = 1;
		CallBackReg.RegistrationContext = NULL;
		RtlInitUnicodeString(&CallBackReg.Altitude, L"321107");
		memset(&OpOperAtion, 0, sizeof(OpOperAtion));

		OpOperAtion.ObjectType = PsProcessType;
		OpOperAtion.Operations = OB_OPERATION_HANDLE_CREATE | OB_OPERATION_HANDLE_DUPLICATE;

		OpOperAtion.PreOperation = (POB_PRE_OPERATION_CALLBACK)&OneProcessAction;

		CallBackReg.OperationRegistration = &OpOperAtion;

		Status = ObRegisterCallbacks(&CallBackReg, &ProcessListenThread);

		if (!NT_SUCCESS(Status))
		{
			Status = STATUS_UNSUCCESSFUL;
		}
		else
		{
			Status = STATUS_SUCCESS;
		}

		return Status;
	}
	else
	{
		if (NULL != ProcessListenThread)
		{
			DbgPrint("CloseProcessListenThread\n");
			ObUnRegisterCallbacks(ProcessListenThread);
		}

		Status = STATUS_SUCCESS;
		return Status;
	}

	Status = STATUS_UNSUCCESSFUL;
	return Status;
}

/// <summary>
/// Check Ring3 Program Order
/// </summary>
/// <param name="Buffer"></param>

void ProcessCommand(PVOID Buffer)
{
	int i = 0;

	char* GetAdd = (char*)Buffer;
	char GetFristChar = GetAdd[0];
	char Cache[25];

	while (*GetAdd)
	{
		GetAdd++;
		Cache[i] = *GetAdd;
		i++;
	}

	ULONG Target = Kernel_Atoi(Cache);

	if (GetFristChar == 'P')
	{
		DbgPrint("PPL ProtectProcess:%i", Target);//Set PPLLProtect
		SetPPLProtect(PidToHandleA(Target));
	}
	else
		if (GetFristChar == 'U')
		{
			DbgPrint("PPL UNProtectProcess:%i", Target);
			UNPPLProtect(PidToHandleA(Target));//Cancel PPLLProtect
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
			//KdBreakPoint(); //Set breakPoint to WinDbg

	        //Check control code equal to IOCTL_SEND_AND_REC_STR
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

	//KdBreakPoint(); //Set breakPoint to WinDbg

	// This can be return the cache sizes
	Irp->IoStatus.Information = ret_len;
	// Recv Io complete state 
	Irp->IoStatus.Status = Status;
	// End the request
	IoCompleteRequest(Irp, IO_NO_INCREMENT);
	return Status;
}





VOID DriverUnload(
	_In_ PDRIVER_OBJECT DriverObject
)
{
	PAGED_CODE();

	UNREFERENCED_PARAMETER(DriverObject);

	UNICODE_STRING DeviceLinkName = RTL_CONSTANT_STRING(MY_DEVOBJ_SYB_NAME);
	IoDeleteSymbolicLink(&DeviceLinkName);
	IoDeleteDevice(CurrentDrive);


	if (NULL != ProcessListenThread)
	{
		DbgPrint("CloseProcessListenThread\n");
		ObUnRegisterCallbacks(ProcessListenThread);
	}

	Log("Driver unloaded.\n");
}

extern "C"
NTSTATUS DriverEntry(
	_In_ PDRIVER_OBJECT DriverObject,
	_In_ PUNICODE_STRING RegistryPath
)
{
	PAGED_CODE();

	UNREFERENCED_PARAMETER(RegistryPath);



	int i;
	OSVERSIONINFOEXW VersionInfo = { sizeof(OSVERSIONINFOEXW) };
	NTSTATUS Status = RtlGetVersion(reinterpret_cast<PRTL_OSVERSIONINFOW>(&VersionInfo));

	UNICODE_STRING DeviceName = RTL_CONSTANT_STRING(MY_DEVOBJ_NAME);
	UNICODE_STRING SDDLString = RTL_CONSTANT_STRING(L"D:P(A;;GA;;;WD)");
	UNICODE_STRING DeviceLinkName = RTL_CONSTANT_STRING(MY_DEVOBJ_SYB_NAME);


	Status = IoCreateDevice(DriverObject, 0, &DeviceName, FILE_DEVICE_UNKNOWN, FILE_DEVICE_SECURE_OPEN, FALSE, &CurrentDrive);

	if (!NT_SUCCESS(Status))
		return Status;


	Status = IoCreateSymbolicLink(&DeviceLinkName, &DeviceName);
	if (!NT_SUCCESS(Status))
	{
		IoDeleteDevice(CurrentDrive);
		return Status;
	}
	for (i = 0; i < IRP_MJ_MAXIMUM_FUNCTION; i++)
	{
		//driver->MajorFunction[i] = NULL;
		DriverObject->MajorFunction[i] = MyDispatch;
	}

	DbgPrint(("InitDrive\n"));


	PLDR_DATA_TABLE_ENTRY64 ldrDataTable;
	ldrDataTable = (PLDR_DATA_TABLE_ENTRY64)DriverObject->DriverSection;
	ldrDataTable->Flags |= 0x20;//Fuck MmVerifyCallbackFunction

	ProtectProcess(TRUE);

	// Only Windows 8.1 and later are afflicted with PPL.
	if (VersionInfo.dwBuildNumber < 6002)
	{
		Log("Unsupported OS version.\n");
		return STATUS_NOT_SUPPORTED;
	}

	if (VersionInfo.dwBuildNumber == 6002)
		SignatureLevelOffset = 0x036c;
	else if (VersionInfo.dwBuildNumber == 7601)
		SignatureLevelOffset = 0x043c;
	else
	{
		if (VersionInfo.dwBuildNumber == 9200)
			IsBlue = true;
		// Find the offsets of the [Section]SignatureLevel fields
		Status = FindSignatureLevelOffsets(&SignatureLevelOffset, &SectionSignatureLevelOffset);
		if (!NT_SUCCESS(Status) && Status != STATUS_NO_MORE_ENTRIES)
		{
			Log("Failed to find the SignatureLevel and SectionSignatureLevel offsets for Windows %u.%u.%u.\n",
				VersionInfo.dwMajorVersion, VersionInfo.dwMinorVersion, VersionInfo.dwBuildNumber);
			return Status;
		}
	}


	DriverObject->DriverUnload = DriverUnload;

	Log("Driver loaded successfully. You can unload it again now since it doesn't do anything.\n");

	return STATUS_SUCCESS;
}