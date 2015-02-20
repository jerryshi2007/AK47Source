
using System;
using MCS.Library.SOA.DataObjects.Schemas.Client;
using MCS.Library.SOA.DataObjects.Schemas.Client.ServiceBroker;
namespace MCS.Library.SOA.DataObjects.Security.AUClient
{
	public interface IAUCenterQueryService : ISchemaQueryService
	{
		ClientAURole GetAURole(string unitID, string codeName, bool normalOnly);
		ClientAURole GetAURoleBySchemaRoleID(string unitID, string schemaRoleID, bool normalOnly);
		ClientAUSchemaRole[] GetAUSchemaRoles(string schemaID, string[] codeNames, bool normalOnly);
		ClientAUSchema[] GetAUSchemaByCategory(string categoryID, bool normalOnly);
		ClientAUAdminScope GetAUAdminScope(string unitID, string scopeType, bool normalOnly);
		ClientAUSchemaCategory[] GetSubCategories(string parentID, bool normalOnly);

	}
}
