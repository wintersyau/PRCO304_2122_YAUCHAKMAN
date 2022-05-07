#pragma once
#include <ntifs.h>
#ifndef _NTSTRUCTDEF_H_
#define _NTSTRUCTDEF_H_


#define MEM_TAG 'MEM'

// Def String Length
#define MAX_STRING_LENGTH			512
#define MAX_PID_LENGTH				32
#define MAX_TIME_LENGTH				20

// Def ConnectName
#define REGISTRY_MONITOR_DEVICE		L"\\Device\\ProcessListen"
#define REGISTRY_MONITOR_SYMBOLIC	L"\\??\\ProcessListen"
#define PROCESS_MONITOR_DEVICE		L"\\Device\\ProcessListen"
#define PROCESS_MONITOR_SYMBOLIC	L"\\??\\ProcessListen"


// Sign Definition
#define  CWK_DVC_SEND_STR \
	(ULONG)CTL_CODE( \
	FILE_DEVICE_UNKNOWN, \
	0x911,METHOD_BUFFERED, \
	FILE_WRITE_DATA)


#define  CWK_DVC_RECV_STR \
	(ULONG)CTL_CODE( \
	FILE_DEVICE_UNKNOWN, \
	0x912,METHOD_BUFFERED, \
	FILE_READ_DATA)

#ifndef _NTIFS_

typedef unsigned short WORD;

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

typedef enum _REG_NOTIFY_CLASS {
	RegNtDeleteKey,
	RegNtPreDeleteKey = RegNtDeleteKey,
	RegNtSetValueKey,
	RegNtPreSetValueKey = RegNtSetValueKey,
	RegNtDeleteValueKey,
	RegNtPreDeleteValueKey = RegNtDeleteValueKey,
	RegNtSetInformationKey,
	RegNtPreSetInformationKey = RegNtSetInformationKey,
	RegNtRenameKey,
	RegNtPreRenameKey = RegNtRenameKey,
	RegNtEnumerateKey,
	RegNtPreEnumerateKey = RegNtEnumerateKey,
	RegNtEnumerateValueKey,
	RegNtPreEnumerateValueKey = RegNtEnumerateValueKey,
	RegNtQueryKey,
	RegNtPreQueryKey = RegNtQueryKey,
	RegNtQueryValueKey,
	RegNtPreQueryValueKey = RegNtQueryValueKey,
	RegNtQueryMultipleValueKey,
	RegNtPreQueryMultipleValueKey = RegNtQueryMultipleValueKey,
	RegNtPreCreateKey,
	RegNtPostCreateKey,
	RegNtPreOpenKey,
	RegNtPostOpenKey,
	RegNtKeyHandleClose,
	RegNtPreKeyHandleClose = RegNtKeyHandleClose,
	//
	// .Net only
	//    
	RegNtPostDeleteKey,
	RegNtPostSetValueKey,
	RegNtPostDeleteValueKey,
	RegNtPostSetInformationKey,
	RegNtPostRenameKey,
	RegNtPostEnumerateKey,
	RegNtPostEnumerateValueKey,
	RegNtPostQueryKey,
	RegNtPostQueryValueKey,
	RegNtPostQueryMultipleValueKey,
	RegNtPostKeyHandleClose,
	RegNtPreCreateKeyEx,
	RegNtPostCreateKeyEx,
	RegNtPreOpenKeyEx,
	RegNtPostOpenKeyEx,
	//
	// new to Windows Vista
	//
	RegNtPreFlushKey,
	RegNtPostFlushKey,
	RegNtPreLoadKey,
	RegNtPostLoadKey,
	RegNtPreUnLoadKey,
	RegNtPostUnLoadKey,
	RegNtPreQueryKeySecurity,
	RegNtPostQueryKeySecurity,
	RegNtPreSetKeySecurity,
	RegNtPostSetKeySecurity,
	//
	// per-object context cleanup
	//
	RegNtCallbackObjectContextCleanup,
	//
	// new in Vista SP2 
	//
	RegNtPreRestoreKey,
	RegNtPostRestoreKey,
	RegNtPreSaveKey,
	RegNtPostSaveKey,
	RegNtPreReplaceKey,
	RegNtPostReplaceKey,
	//
	// new to Windows 10
	//
	RegNtPreQueryKeyName,
	RegNtPostQueryKeyName,

	MaxRegNtNotifyClass //should always be the last enum
} REG_NOTIFY_CLASS;
#endif

typedef struct _PROCESSINFO
{
	TIME_FIELDS			time;						// Message Time
	BOOLEAN				bIsCreate;					// Is Created
	HANDLE				hParentProcessId;			// Parent Process ID
	ULONG				ulParentProcessLength;		// Parent Process Length (Name)	
	HANDLE				hProcessId;					// Process Pid
	ULONG				ulProcessLength;			
	ULONG				ulCommandLineLength;		//Start Args Length
	BOOLEAN             ParentSystem;
	BOOLEAN             ThisSystem;                 //Is System Thread
	UCHAR				uData[1];					//Start InFo
} PROCESSINFO, * PPROCESSINFO;

typedef struct _REGISTRY_EVENT
{
	HANDLE				hProcessId;					// ProcessID
	TIME_FIELDS			time;						// Message Time
	REG_NOTIFY_CLASS	enRegistryNotifyClass;		// Process Module
	ULONG				ulProcessPathLength;		// Process Path Length(Path)
	ULONG				ulRegistryPathLength;		// Process StartPath(Path)
	ULONG				ulKeyValueType;				// Start Args Type
	ULONG				ulDataLength;				//Start Args Length
	UCHAR				uData[1];					//Start InFo
} REGISTRY_EVENT, * PREGISTRY_EVENT;

#endif
