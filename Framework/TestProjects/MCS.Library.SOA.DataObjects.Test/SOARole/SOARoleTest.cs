using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Passport;
using MCS.Library.Principal;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;
using System.Data;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test
{
	[TestClass]
	public class SOARoleTest
	{
		[TestMethod]
		[TestCategory("SOARole")]
		public void UpdateRolePropertyDefinitionTest()
		{
			IRole role = GetTestRole();

			SOARolePropertyDefinitionCollection propertiesDefinition = UpdateRolePropertiesDefinition(role);

			SOARolePropertyDefinitionCollection loadedPropertiesDefinition = SOARolePropertyDefinitionAdapter.Instance.LoadByRole(role);

			Assert.AreEqual(propertiesDefinition.Count, loadedPropertiesDefinition.Count);
		}

		[TestMethod]
		[TestCategory("SOARole")]
		public void UpdateRolePropertyValuesTest()
		{
			SOARole role = PrepareSOARole();

			SOARolePropertiesAdapter.Instance.Update(role);

			SOARolePropertyRowCollection loadedRows = SOARolePropertiesAdapter.Instance.LoadByRole(role);

			Assert.AreEqual(role.Rows.Count, loadedRows.Count);

			int i = 0;

			foreach (SOARolePropertyRow row in role.Rows)
			{
				Assert.AreEqual(row.Values.Count, loadedRows[i].Values.Count);
				i++;
			}

			DataView view = loadedRows.ToDataView(role.PropertyDefinitions);

			foreach (DataRowView drv in view)
			{
				foreach (DataColumn column in view.Table.Columns)
				{
					Console.Write("{0}={1} ", column.ColumnName, drv[column.ColumnName]);
				}

				Console.WriteLine();
			}
		}

		[TestMethod]
		[TestCategory("SOARole")]
		public void ReplaceRoleOperatorTest()
		{
			SOARole role = PrepareSOARole();

			SOARolePropertyRowCollection rows = SOARolePropertiesAdapter.Instance.LoadByRoleID(role.ID, null);
			
			rows.ReplaceOperators(SOARoleOperatorType.User, "wanhw", "fanhy", "wanhw", "yangrui1");

			SOARolePropertiesAdapter.Instance.Update(role.ID, rows);

			SOARolePropertyRowCollection loadedRows = SOARolePropertiesAdapter.Instance.LoadByRole(role);

			Console.WriteLine(loadedRows[0].Operator);

			Assert.AreEqual("fanhy,yangrui1", loadedRows[0].Operator);
		}

		[TestMethod]
		[TestCategory("SOARole")]
		public void ReplaceRoleOperatorToNullTest()
		{
			SOARole role = PrepareSOARole();

			SOARolePropertiesAdapter.Instance.Update(role);

			role.Rows.ReplaceOperators(SOARoleOperatorType.User, "wanhw");

			SOARolePropertiesAdapter.Instance.Update(role);

			SOARolePropertyRowCollection loadedRows = SOARolePropertiesAdapter.Instance.LoadByRole(role);

			Console.WriteLine(loadedRows[0].Operator);

			Assert.AreEqual("fanhy", loadedRows[0].Operator);
		}

		[TestMethod]
		[TestCategory("SOARole")]
		[Description("测试单一属性查询")]
		public void QueryRoleSingleProperty()
		{
			SOARole role = PrepareSOARole();

			SOARolePropertiesQueryParam queryParam = new SOARolePropertiesQueryParam();

			queryParam.QueryName = "CostCenter";
			queryParam.QueryValue = "1001";

			SOARolePropertyRowCollection rows = role.Rows.Query(queryParam);

			Assert.AreEqual(1, rows.Count);
		}

		[TestMethod]
		[TestCategory("SOARole")]
		[Description("根据上下文中查询人员")]
		public void QueryRoleUserByContext()
		{
			SOARole role = PrepareSOARole();

			WfRoleResourceDescriptor roleDescriptor = new WfRoleResourceDescriptor(role);
			WfResourceDescriptorCollection roles = new WfResourceDescriptorCollection();

			roles.Add(roleDescriptor);

			Dictionary<string, object> parameters = new Dictionary<string, object>();

			parameters["CostCenter"] = "1002";
			parameters["FormType"] = "Form1";

			WfApplicationParametersContext.CreateContext(parameters);

			OguDataCollection<IUser> users = roles.ToUsers();

			Assert.AreEqual(4, users.Count);
		}

		[TestMethod]
		[TestCategory("SOARole")]
		[Description("测试已经删除的列的数据获取")]
		public void QueryRoleSingleRemovedProperty()
		{
			SOARole role = PrepareSOARole();

			role.PropertyDefinitions.Remove(spd => spd.Name == "CostCenter");

			SOARolePropertyDefinitionAdapter.Instance.Update(role, role.PropertyDefinitions);

			SOARole roleLoaded = new SOARole(role.FullCodeName);

			SOARolePropertiesQueryParam queryParam = new SOARolePropertiesQueryParam();

			queryParam.QueryName = "CostCenter";
			queryParam.QueryValue = "1001";

			SOARolePropertyRowCollection rows = roleLoaded.Rows.Query(queryParam);

			Assert.AreEqual(0, rows.Count);
		}

		[TestMethod]
		[TestCategory("SOARole")]
		public void QueryRoleSinglePropertyUsers()
		{
			SOARole role = PrepareSOARole();

			SOARolePropertiesQueryParam queryParam = new SOARolePropertiesQueryParam();

			queryParam.QueryName = "CostCenter";
			queryParam.QueryValue = "1001";

			SOARolePropertyRowCollection rows = role.Rows.Query(queryParam);

			Assert.AreEqual(1, rows.Count);

			foreach (var rowUsers in rows.GenerateRowsUsers())
			{
				rowUsers.Users.ForEach(u => Console.WriteLine(u.LogOnName));
			}
		}

		[TestMethod]
		[TestCategory("SOARole")]
		public void ReplaceUserInSOARoleTest()
		{
			SOARole role = PrepareSOARole();

			SOARolePropertiesQueryParam queryParam = new SOARolePropertiesQueryParam();

			queryParam.QueryName = "CostCenter";
			queryParam.QueryValue = "1001";

			SOARolePropertyRowCollection rows = role.Rows.Query(queryParam);

			Assert.AreEqual(1, rows.Count);

			foreach (var rowUsers in rows.GenerateRowsUsers())
			{
				rowUsers.Users.ForEach(u => Console.WriteLine(u.LogOnName));
			}
		}

		private static SOARole PrepareSOARole()
		{
			IRole originalRole = GetTestRole();

			SOARolePropertyDefinitionCollection pds = UpdateRolePropertiesDefinition(originalRole);

			SOARole role = new SOARole(originalRole.FullCodeName);

			role.Rows.Clear();

			SOARolePropertyRow row1 = new SOARolePropertyRow(role) { RowNumber = 1, OperatorType = SOARoleOperatorType.User, Operator = "fanhy,wanhw" };

			row1.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1001" });
			row1.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "1" });
			row1.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "30" });

			SOARolePropertyRow row2 = new SOARolePropertyRow(role) { RowNumber = 2, OperatorType = SOARoleOperatorType.User, Operator = "wangli5" };

			row2.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
			row2.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "2" });
			row2.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "40" });

			SOARolePropertyRow row3 = new SOARolePropertyRow(role) { RowNumber = 3, OperatorType = SOARoleOperatorType.Role, Operator = RolesDefineConfig.GetConfig().RolesDefineCollection["nestedRole"].Roles };

			row3.Values.Add(new SOARolePropertyValue(pds["CostCenter"]) { Value = "1002" });
			row3.Values.Add(new SOARolePropertyValue(pds["PayMethod"]) { Value = "2" });
			row3.Values.Add(new SOARolePropertyValue(pds["Age"]) { Value = "60" });

			role.Rows.Add(row1);
			role.Rows.Add(row2);
			role.Rows.Add(row3);

			SOARolePropertiesAdapter.Instance.Update(role);

			return role;
		}

		private static SOARolePropertyDefinitionCollection UpdateRolePropertiesDefinition(IRole role)
		{
			SOARolePropertyDefinitionCollection propertiesDefinition = PreparePropertiesDefinition(role);

			SOARolePropertyDefinitionAdapter.Instance.Update(role, propertiesDefinition);

			return propertiesDefinition;
		}

		private static SOARolePropertyDefinitionCollection PreparePropertiesDefinition(IRole role)
		{
			SOARolePropertyDefinitionCollection propertiesDefinition = new SOARolePropertyDefinitionCollection();

			propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "CostCenter", SortOrder = 0 });
			propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "PayMethod", SortOrder = 1 });
			propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "Age", SortOrder = 2, DataType = ColumnDataType.Integer });
			propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "OperatorType", SortOrder = 3, DataType = ColumnDataType.String });
			propertiesDefinition.Add(new SOARolePropertyDefinition(role) { Name = "Operator", SortOrder = 4, DataType = ColumnDataType.String });

			return propertiesDefinition;
		}

		private static IRole GetTestRole()
		{
			IRole[] roles = DeluxePrincipal.GetRoles(RolesDefineConfig.GetConfig().RolesDefineCollection["testRole"].Roles);

			return roles[0];
		}
	}
}
