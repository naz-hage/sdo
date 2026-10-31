---
title: Azure DevOps work item payloads
---

# Azure DevOps work item payloads

SDO uses Azure DevOps JSON Patch requests when it creates or updates work
items. This is an Azure DevOps API requirement and is unrelated to tool
manifest formats; SDO supports YAML tool manifests only.

The implementation is in
[`AzureDevOpsClient.cs`](../Sdo/Services/AzureDevOpsClient.cs).

## Create a work item

Azure DevOps expects a `POST` request with a JSON Patch array:

```http
POST https://dev.azure.com/{organization}/{project}/_apis/wit/workitems/${type}?api-version=7.1
Content-Type: application/json-patch+json
```

```json
[
  { "op": "add", "path": "/fields/System.Title", "value": "Example issue title" },
  { "op": "add", "path": "/fields/System.Description", "value": "Description" },
  { "op": "add", "path": "/fields/Microsoft.VSTS.Common.AcceptanceCriteria", "value": "- AC1\n- AC2" }
]
```

The acceptance-criteria operation is included only when the project supports
the `Microsoft.VSTS.Common.AcceptanceCriteria` field. SDO discovers the
available work-item fields from the Azure DevOps work-item type schema.

## Authentication

Set `AZURE_DEVOPS_PAT` or configure the Azure DevOps credential in Windows
Credential Manager. The PAT needs permission to read and write work items.

## Common failures

- **401 Unauthorized**: Check `AZURE_DEVOPS_PAT`, Credential Manager, and PAT
  permissions.
- **400 Bad Request**: Check the work-item type, field reference names, and
  required fields.
- **404 Not Found**: Check the organization, project, and work-item endpoint.
