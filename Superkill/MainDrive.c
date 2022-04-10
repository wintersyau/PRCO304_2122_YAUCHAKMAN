#include "HeadFunction.h"




VOID DriverUnload(__in struct _DRIVER_OBJECT* DriverObject)
{
    DbgPrint(("DriverUnload\n"));

    UNICODE_STRING DeviceLinkName = RTL_CONSTANT_STRING(MY_DEVOBJ_SYB_NAME);

    IoDeleteSymbolicLink(&DeviceLinkName);
    IoDeleteDevice(CurrentDrive);
}


NTSTATUS DriverEntry(PDRIVER_OBJECT Driver, PUNICODE_STRING RegPath)
{
    int i;
    NTSTATUS Status;

    UNICODE_STRING DeviceName = RTL_CONSTANT_STRING(MY_DEVOBJ_NAME);
    UNICODE_STRING SDDLString = RTL_CONSTANT_STRING(L"D:P(A;;GA;;;WD)");
    UNICODE_STRING DeviceLinkName = RTL_CONSTANT_STRING(MY_DEVOBJ_SYB_NAME);

    Status = IoCreateDevice(Driver, 0, &DeviceName, FILE_DEVICE_UNKNOWN, FILE_DEVICE_SECURE_OPEN, FALSE, &CurrentDrive);

    if (!NT_SUCCESS(Status))
    {
        return Status;
    }

    Status = IoCreateSymbolicLink(&DeviceLinkName, &DeviceName);
    if (!NT_SUCCESS(Status))
    {
        IoDeleteDevice(CurrentDrive);
        return Status;
    }

    for (i = 0; i < IRP_MJ_MAXIMUM_FUNCTION; i++)
    {
        //driver->MajorFunction[i] = NULL;
        Driver->MajorFunction[i] = MyDispatch;
    }

    PLDR_DATA_TABLE_ENTRY64 ldrDataTable;
    ldrDataTable = (PLDR_DATA_TABLE_ENTRY64)Driver->DriverSection;
    ldrDataTable->Flags |= 0x20;//Fuck MmVerifyCallbackFunction

    // Ö§³Ö¶¯Ì¬Ð¶ÔØ¡£
    Driver->DriverUnload = DriverUnload;

    DbgPrint((Sys_Version));

    DbgPrint(("InitDrive\n"));

    return STATUS_SUCCESS;
}





