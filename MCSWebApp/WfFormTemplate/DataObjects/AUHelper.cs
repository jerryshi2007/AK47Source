using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects;

namespace WfFormTemplate.DataObjects
{
	/// <summary>
	/// 管理单元操作的辅助类
	/// </summary>
	public static class AUHelper
	{
		public const string AUSchemaID = "2f3e1526-275d-9aec-4734-c91696c70f1d";
		public const string AUSchemaRoleID = "1bfaaf2a-b807-90ff-4e34-f6d3343a4327";				//合同业务审批人
		public const string AUSchemaMatrixRoleID = "4cc357e9-e4a9-93ea-4809-9386ac2e266d";			//工程管理员
		public const string AUSchemaActivityMatrixRoleID = "5b3ef6f7-1a10-b32d-4c83-c7b8e960558f";	//实物管理员

		//北京管理单元
		public const string AUContainsUsersUnitID = "358a59de-0bb0-bc09-4da4-5b68f7d683ce";
		public const string AUContainsUsersUnitCodeName = "BJ_Unit";

		public const string AUContainsUsersRoleID = "a164e737-544f-abe1-467e-c3ff64827d96";		//合同业务审批人

		//集团总部管理单元
		public const string AUContainsMatrixUnitID = "0a9367c0-fd9c-9a1b-4954-40480a9a5a92";
		public const string AUContainsMatrixUnitCodeName = "Group";

		public const string AUContainsMatrixRoleID = "062fb381-0bc7-b3c1-4f6b-f12ba8470b28";	//工程管理员

		//北京管理单元
		public const string AUContainsMatrixUnitID2 = "358a59de-0bb0-bc09-4da4-5b68f7d683ce";
		public const string AUContainsMatrixUnitCodeName2 = "BJ_Unit";

		public const string ConnectionName = "AdminUnit";

		public static void InitializeAdministrativeUnitData()
		{
			//清空数据
			DbHelper.RunSql("EXECUTE SC.ClearAllData", ConnectionName);

			//准备管理单元的数据
			DbHelper.RunSql("EXECUTE SC.InitializeSampleData", ConnectionName);

			//准备活动矩阵
			PrepareActivityMatrixData(AUSchemaActivityMatrixRoleID, AUSchemaActivityMatrixRoleID);
		}

		/// <summary>
		/// 清除已经存在的数据
		/// </summary>
		/// <param name="definitionID"></param>
		/// <param name="roleID"></param>
		private static void ClearMatrixData(string definitionID, string roleID)
		{
			SOARole role = new SOARole();
			role.ID = definitionID;

			SOARolePropertyDefinitionAdapter.Instance.Delete(role);

			role.ID = roleID;
			SOARolePropertiesAdapter.Instance.Delete(role);
		}

		private static void ClearAllMatrixData()
		{
			ClearMatrixData(AUSchemaMatrixRoleID, AUSchemaMatrixRoleID);
			ClearMatrixData(AUSchemaActivityMatrixRoleID, AUSchemaActivityMatrixRoleID);
			ClearMatrixData(AUContainsMatrixRoleID, AUContainsMatrixRoleID);
		}

		private static void PrepareActivityMatrixData(string definitionID, string roleID)
		{
			ClearAllMatrixData();

			SOARole role = new SOARole();
			role.ID = definitionID;

			SOARolePropertyDefinitionCollection definition = PrepareActivityMatrixPropertiesDefinition(definitionID);

			SOARolePropertyDefinitionAdapter.Instance.Update(role, definition);

			role = new SOARole(definition);
			role.ID = roleID;

			PrepareActivityMatrixRows(role);

			SOARolePropertiesAdapter.Instance.Update(role);
		}

		/// <summary>
		/// 准备完维度定义数据后，准备行数据
		/// </summary>
		/// <param name="role"></param>
		private static void PrepareActivityMatrixRows(SOARole role)
		{
			AddOneActivityMatrixRow(role, "Group", 1, 10, SOARoleOperatorType.User, "liumh");
			AddOneActivityMatrixRow(role, "Group", 2, 20, SOARoleOperatorType.User, "fanhy");
			AddOneActivityMatrixRow(role, "BJ_Unit", 3, 10, SOARoleOperatorType.User, "yangrui1");
		}

		private static void AddOneActivityMatrixRow(SOARole role, string auCodeName, int rowNumber, int activitySN, SOARoleOperatorType operatorType, string op)
		{
			SOARolePropertyDefinitionCollection definition = role.PropertyDefinitions;

			SOARolePropertyRow row = new SOARolePropertyRow(role) { RowNumber = rowNumber, OperatorType = operatorType, Operator = op };

			row.Values.Add(new SOARolePropertyValue(definition["AdministrativeUnit"]) { Value = auCodeName });
			row.Values.Add(new SOARolePropertyValue(definition["ActivitySN"]) { Value = activitySN.ToString() });

			role.Rows.Add(row);
		}

		private static SOARolePropertyDefinitionCollection PrepareActivityMatrixPropertiesDefinition(string definitionID)
		{
			SOARole role = new SOARole();
			role.ID = definitionID;

			SOARolePropertyDefinitionCollection propertiesDefinition = new SOARolePropertyDefinitionCollection();

			propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "AdministrativeUnit", SortOrder = 0 });
			propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "ActivitySN", SortOrder = 0, DataType = ColumnDataType.Integer });
			propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "OperatorType", SortOrder = 1, DataType = ColumnDataType.String });
			propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "Operator", SortOrder = 2, DataType = ColumnDataType.String });

			return propertiesDefinition;
		}
	}
}