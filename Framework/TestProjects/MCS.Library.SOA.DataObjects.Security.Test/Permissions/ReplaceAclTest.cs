using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace MCS.Library.SOA.DataObjects.Security.Test.Permissions
{
	[TestClass]
	public class ReplaceAclTest
	{
		[TestMethod]
		[TestCategory(Constants.PermissionSetCategory)]
		public void ReplaceAcl()
		{
			SCObjectGenerator.PreareTestOguObjectForDelete();
            var parent1 = (SCOrganization)SchemaObjectAdapter.Instance.LoadByCodeName("Organizations", "groupHQ", SchemaObjectStatus.Normal, DateTime.MinValue);

            var parent2 = (SCOrganization)SchemaObjectAdapter.Instance.LoadByCodeName("Organizations", "流程管理部", SchemaObjectStatus.Normal, DateTime.MinValue);

            var role1 = (SCRole)SchemaObjectAdapter.Instance.LoadByCodeName("Roles", "系统管理员", SchemaObjectStatus.Normal, DateTime.MinValue);

            var role2 = (SCRole)SchemaObjectAdapter.Instance.LoadByCodeName("Roles", "系统维护员", SchemaObjectStatus.Normal, DateTime.MinValue);

			var container = new PC.Permissions.SCAclContainer(parent1);

			container.Members.Add("AddChildren", role1);
			container.Members.Add("DeleteChildren", role1);

			container.Members.Add("UpdateChildren", role2);
			container.Members.Add("EditPermissionsOfChildren", role2);
			container.Members.Add("AddChildren", role2);

			PC.Executors.SCObjectOperations.Instance.UpdateObjectAcl(container);

			var childAcls = PC.Adapters.SCAclAdapter.Instance.LoadByContainerID(parent2.ID, DateTime.MinValue);
			Assert.IsTrue(childAcls.Count == 0);

			SCReplaceAclRecursivelyExecutor executor = new SCReplaceAclRecursivelyExecutor(SOA.DataObjects.Security.Actions.SCOperationType.ReplaceAclRecursively, parent1) { };
			executor.Execute();

			childAcls = PC.Adapters.SCAclAdapter.Instance.LoadByContainerID(parent2.ID, DateTime.MinValue);
			Assert.IsTrue(childAcls.Count == 5);

			Assert.IsTrue((from p in childAcls where p.ContainerID == parent2.ID && p.ContainerPermission == "AddChildren" && p.MemberID == role1.ID select p).Any());
			Assert.IsTrue((from p in childAcls where p.ContainerID == parent2.ID && p.ContainerPermission == "DeleteChildren" && p.MemberID == role1.ID select p).Any());
			Assert.IsTrue((from p in childAcls where p.ContainerID == parent2.ID && p.ContainerPermission == "UpdateChildren" && p.MemberID == role2.ID select p).Any());
			Assert.IsTrue((from p in childAcls where p.ContainerID == parent2.ID && p.ContainerPermission == "EditPermissionsOfChildren" && p.MemberID == role2.ID select p).Any());
			Assert.IsTrue((from p in childAcls where p.ContainerID == parent2.ID && p.ContainerPermission == "AddChildren" && p.MemberID == role2.ID select p).Any());
		}
	}
}
