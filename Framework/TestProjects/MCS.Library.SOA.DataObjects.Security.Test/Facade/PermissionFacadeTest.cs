using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Security.Test.SchemaObject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Executors;

namespace MCS.Library.SOA.DataObjects.Security.Test.Facade
{
	[TestClass]
	public class PermissionFacadeTest
	{
		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void AddPermissionTest()
		{
			Trace.CorrelationManager.ActivityId = UuidHelper.NewUuid();

			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCPermission permission = SCObjectGenerator.PreparePermissionObject();

			SCObjectOperations.Instance.AddPermission(permission, application);

			application.CurrentPermissions.ContainsKey(permission.ID);
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void JoinRoleAndPermissionTest()
		{
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role = SCObjectGenerator.PrepareRoleObject();

			SCObjectOperations.Instance.AddRole(role, application);

			SCPermission permission = SCObjectGenerator.PreparePermissionObject();

			SCObjectOperations.Instance.AddPermission(permission, application);

			SCRelationObject relation = SCObjectOperations.Instance.JoinRoleAndPermission(role, permission);

			role.ClearRelativeData();
			Assert.IsTrue(role.CurrentPermissions.ContainsKey(permission.ID));
			Assert.IsTrue(permission.CurrentRoles.ContainsKey(role.ID));

			SCObjectOperations.Instance.DisjoinRoleAndPermission(role, permission);

			role.ClearRelativeData();
			permission.ClearRelativeData();

			Assert.IsFalse(role.CurrentPermissions.ContainsKey(permission.ID));
			Assert.IsFalse(permission.CurrentRoles.ContainsKey(role.ID));
		}

		[TestMethod]
		[TestCategory(Constants.FacadeCategory)]
		public void DeletePermissionTest()
		{
			SCApplication application = SCObjectGenerator.PrepareApplicationObject();

			SCObjectOperations.Instance.AddApplication(application);

			SCRole role = SCObjectGenerator.PrepareRoleObject();

			SCObjectOperations.Instance.AddRole(role, application);

			SCPermission permission = SCObjectGenerator.PreparePermissionObject();

			SCObjectOperations.Instance.AddPermission(permission, application);

			SCRelationObject relation = SCObjectOperations.Instance.JoinRoleAndPermission(role, permission);

			SCObjectOperations.Instance.DeletePermission(permission);

			application.ClearRelativeData();
			role.ClearRelativeData();

			Console.WriteLine("Role permission count {0}, applicaiton permission count {1}",
				role.CurrentPermissions.Count, application.CurrentPermissions.Count);

			Assert.AreEqual(0, role.CurrentPermissions.Count);
			Assert.AreEqual(0, application.CurrentPermissions.Count);
		}
	}
}
