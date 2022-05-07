#include "ListEntry.h"

PPROCESSNODE InitListNode()
{
	PPROCESSNODE pNode = NULL;

	pNode = (PPROCESSNODE)ExAllocatePoolWithTag(NonPagedPool, sizeof(PROCESSNODE), MEM_TAG);
	if (pNode == NULL)
	{
		return NULL;
	}

	return pNode;
}

VOID DestroyList()
{
	// Keep Process
	while (TRUE)
	{
		// Get Frist Linked Item
		PPROCESSNODE pNode = (PPROCESSNODE)ExInterlockedRemoveHeadList(&g_ListHead, &g_Lock);
		if (NULL != pNode)
		{
			if (NULL != pNode->pProcessInfo)
			{
				ExFreePoolWithTag(pNode->pProcessInfo, MEM_TAG);//Delete Item
			}
			ExFreePoolWithTag(pNode, MEM_TAG);
		}
		else
		{
			break;
		}
	};
}
