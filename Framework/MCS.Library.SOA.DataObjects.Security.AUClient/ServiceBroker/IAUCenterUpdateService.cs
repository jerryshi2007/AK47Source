using System;
namespace MCS.Library.SOA.DataObjects.Security.AUClient
{
	/// <summary>
	/// 定义更新接口
	/// </summary>
	public interface IAUCenterUpdateService
	{
		void AddAdminSchema(ClientAUSchema schema);
		void AddAdminSchemaRole(ClientAUSchemaRole role, ClientAUSchema schema);
		void AddAdminUnit(ClientAdminUnit unit, ClientAdminUnit parent);
		void AddAdminUnitWithMembers(ClientAdminUnit unit, ClientAdminUnit parent, ClientAURole[] roles, ClientAUAdminScope[] scopes);
		void AddObjectToScope(ClientAUAdminScopeItem item, ClientAUAdminScope scope);
		void AddUserToRole(ClientGenericObject user, ClientAdminUnit unit, ClientAUSchemaRole role);
		void DeleteAdminSchemaRole(ClientAUSchemaRole role);
		void DeleteAdminUnit(ClientAdminUnit unit);
		string GetFacadeType();
		void MoveAdminUnit(ClientAdminUnit unit, ClientAdminUnit newParent);
		string NewID();
		void RemoveObjectFromScope(ClientAUAdminScopeItem item, ClientAUAdminScope scope);
		void RemoveUserFromRole(ClientGenericObject user, ClientAdminUnit unit, ClientAUSchemaRole role);
		void ReplaceAclRecursively(string ownerID, bool force);
		void ReplaceUsersInRole(ClientGenericObject[] users, ClientAdminUnit unit, ClientAUSchemaRole role);
		void UpdateAdminSchemaRole(ClientAUSchemaRole role);
		void UpdateAdminUnit(ClientAdminUnit unit);
		void UpdateObjectAcl(string ownerID, MCS.Library.SOA.DataObjects.Schemas.Client.ClientAclItem[] clientAcls);
		void UpdateScopeCondition(ClientAUAdminScope scope, MCS.Library.SOA.DataObjects.Schemas.Client.ClientConditionItem condition);
	}
}
