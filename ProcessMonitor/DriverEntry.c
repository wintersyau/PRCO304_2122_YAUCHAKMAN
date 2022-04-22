#include <ntifs.h>
#include "ProcessMgr.h"
#include "IODeviceControl.h"

NTKERNELAPI
BOOLEAN
PsIsSystemProcess(
	_In_ PEPROCESS Process
);

typedef struct _PS_PROTECTION {
	UCHAR Type : 3;
	UCHAR Audit : 1;
	UCHAR Signer : 4;
} PS_PROTECTION, * PPS_PROTECTION;

typedef struct _PROCESS_SIGNATURE_PROTECTION {
	UCHAR SignatureLevel;
	UCHAR SectionSignatureLevel;
	PS_PROTECTION Protection;
} PROCESS_SIGNATURE_PROTECTION, * PPROCESS_SIGNATURE_PROTECTION;

PEPROCESS PidToEprocess(ULONG Pid)
{
	PEPROCESS pEProc;
	PsLookupProcessByProcessId((HANDLE)Pid, &pEProc);
	ObDereferenceObject(pEProc);
	return pEProc;
}

VOID CreateProcessNotify(
	IN HANDLE   ParentId,
	IN HANDLE   ProcessId,
	IN BOOLEAN  Create
)
{
	UNICODE_STRING	usProcessParameters;

	if (NULL != Create)
	{
		LARGE_INTEGER		unCurrentSystemTime;
		LARGE_INTEGER		unCurrentLocalTime;

		PEPROCESS ParentPE = PidToEprocess(ParentId);
		PEPROCESS ThiePE = PidToEprocess(ProcessId);
		//Get DateTime
		KeQuerySystemTime(&unCurrentSystemTime);
		ExSystemTimeToLocalTime(&unCurrentSystemTime, &unCurrentLocalTime);

		PPROCESSNODE pNode = InitListNode();//init node

		if (pNode != NULL)
		{
			SIZE_T	ulNumberOfBytes = sizeof(PROCESSINFO) + 255;//PROCESSINFO.Length + 255 Extend Size  (Data Must <255)

			//SetProcessInFo
			pNode->pProcessInfo = ExAllocatePoolWithTag(NonPagedPool, ulNumberOfBytes, MEM_TAG);

			pNode->pProcessInfo->bIsCreate = TRUE;
			pNode->pProcessInfo->hParentProcessId =ParentId;
			pNode->pProcessInfo->ulParentProcessLength = 0;
			pNode->pProcessInfo->hProcessId = ProcessId;
			pNode->pProcessInfo->ulProcessLength =0;
			pNode->pProcessInfo->ulCommandLineLength = 0;
			
			BOOLEAN ParentSystem = FALSE;
			BOOLEAN ThisSystem = FALSE;

			ParentSystem = PsIsSystemProcess(ParentPE);//Check Parent Process is System Process
			ThisSystem = PsIsSystemProcess(ThiePE);//Check Process is System Process

			pNode->pProcessInfo->ParentSystem = ParentSystem;
			pNode->pProcessInfo->ThisSystem = ThisSystem;

			RtlTimeToTimeFields(&unCurrentLocalTime, &pNode->pProcessInfo->time);

			ExInterlockedInsertTailList(&g_ListHead, (PLIST_ENTRY)pNode, &g_Lock);//Insert LinkedList
			KeSetEvent(&g_Event, 0, FALSE);//Set One Sign
		}
	}
	else
	{

	}

	if (Create)
	{	

	}
	else
	{

	}
}

VOID DriverUnload(_In_ PDRIVER_OBJECT pDriverObject)
{
	PAGED_CODE();
	UNREFERENCED_PARAMETER(pDriverObject);

	DbgPrint(("UnloadC..."));//WinDBG Test

	PsSetCreateProcessNotifyRoutine(CreateProcessNotify, TRUE);

	DbgPrint(("UnloadA..."));

	RemoveDevice(pDriverObject);
	DestroyList();

    DbgPrint(("UnloadB..."));
}

NTSTATUS DriverEntry(
	_In_ PDRIVER_OBJECT  pDriverObject,
	_In_ PUNICODE_STRING RegistryPath
)
{
	NTSTATUS status;

	// Install PsSetCreateProcessNotify Hook
	
	status = PsSetCreateProcessNotifyRoutine(CreateProcessNotify, FALSE);

	if (!NT_SUCCESS(status))//Error
	{
		KdPrint(("Failed to call PsSetCreateProcessNotifyRoutine, error code = 0x%08X", status));
	}

	//Init LinkedList
	KeInitializeEvent(&g_Event, SynchronizationEvent, TRUE);
	//Init Linked Lock 
	KeInitializeSpinLock(&g_Lock);
	//Init LinkedList Head
	InitializeListHead(&g_ListHead);

	//Device Creat
	CreateDevice(pDriverObject);
	//Binding Unload
	pDriverObject->DriverUnload = DriverUnload;
	DbgPrint("InstallSucess!");

	//Config IRP
	pDriverObject->MajorFunction[IRP_MJ_CREATE] = CreateCompleteRoutine;
	pDriverObject->MajorFunction[IRP_MJ_CLOSE] = CloseCompleteRoutine;
	pDriverObject->MajorFunction[IRP_MJ_READ] = ReadCompleteRoutine;
	pDriverObject->MajorFunction[IRP_MJ_WRITE] = WriteCompleteRoutine;
	pDriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = DeviceControlCompleteRoutine;

	

	return status;
}

