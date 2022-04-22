#ifndef __LISTENTRY_H__
#define __LISTENTRY_H__

#include "StructDef.h"
#include <ntifs.h>

KSPIN_LOCK	g_Lock;			// Linked Locker
KEVENT		g_Event;		// Linked AnySign
LIST_ENTRY	g_ListHead;		// Linked Head



//Process Linked 
typedef struct
{
	LIST_ENTRY		list_entry;
	PPROCESSINFO	pProcessInfo;
} PROCESSNODE, *PPROCESSNODE;


PPROCESSNODE	InitListNode();
VOID			DestroyList();

#endif // !__LISTENTRY_H__


