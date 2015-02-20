using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.OGUPermission;
using MCS.Web.Library.Script;
using MCS.Library.Passport;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects.Test.JSONTest
{
	[TestClass]
	public class OguObjectJSONTest
	{
		[TestMethod]
		[Description("OguApplication的序列化测试")]
		public void OguApplicationSerializationTest()
		{
			OguApplication app = (OguApplication)OguApplication.CreateWrapperObject(PermissionMechanismFactory.GetMechanism().GetAllApplications().First());

			JSONSerializerExecute.RegisterConverter(typeof(OguApplicationConverter));

			string serializedData = JSONSerializerExecute.Serialize(app);

			Console.WriteLine(serializedData);

			OguApplication deserializedData = JSONSerializerExecute.Deserialize<OguApplication>(serializedData);

			ValidatePermissionObject(app, deserializedData);
		}

		[TestMethod]
		[Description("OguApplicationCollection的序列化测试")]
		public void OguApplicationCollectionSerializationTest()
		{
			JSONSerializerExecute.RegisterConverter(typeof(OguApplicationConverter));

			ApplicationCollection originalApps = PermissionMechanismFactory.GetMechanism().GetAllApplications();
			OguApplicationCollection apps = new OguApplicationCollection(originalApps);

			CheckOguApplicationCollectionItems(apps);

			//测试Set操作
			for (int i = 0; i < apps.Count; i++)
				apps[i] = originalApps[i];

			CheckOguApplicationCollectionItems(apps);

			string serializedData = JSONSerializerExecute.Serialize(apps);

			Console.WriteLine(serializedData);

			OguApplicationCollection deserializedData = JSONSerializerExecute.Deserialize<OguApplicationCollection>(serializedData);

			for (int i = 0; i < apps.Count; i++)
				ValidatePermissionObject(apps[i], deserializedData[i]);
		}

		[TestMethod]
		[Description("OguRoleCollection的序列化测试")]
		public void OguRoleCollectionSerializationTest()
		{
			JSONSerializerExecute.RegisterConverter(typeof(OguApplicationConverter));
			JSONSerializerExecute.RegisterConverter(typeof(OguRoleConverter));

			IRole[] testRoles = DeluxePrincipal.GetRoles(RolesDefineConfig.GetConfig().RolesDefineCollection["testRole"].Roles);

			OguRoleCollection roles = new OguRoleCollection(testRoles);

			string serializedData = JSONSerializerExecute.Serialize(roles);

			Console.WriteLine(serializedData);

			OguRoleCollection deserializedData = JSONSerializerExecute.Deserialize<OguRoleCollection>(serializedData);

			for (int i = 0; i < roles.Count; i++)
				ValidatePermissionObject(roles[i], deserializedData[i]);
		}

		/// <summary>
		/// 检查OguApplicationCollection中每一项都是OguApplication这种可序列化类型的
		/// </summary>
		/// <param name="apps"></param>
		private void CheckOguApplicationCollectionItems(OguApplicationCollection apps)
		{
			for (int i = 0; i < apps.Count; i++)
			{
				Assert.IsTrue(apps[i] is OguApplication, string.Format("OguApplicationCollection的第{0}项不是OguApplication类型的", i));
			}
		}

		private void ValidatePermissionObject(IPermissionObject expected, IPermissionObject actual)
		{
			Assert.AreEqual(expected.ID, actual.ID);
			Assert.AreEqual(expected.Name, actual.Name);
			Assert.AreEqual(expected.CodeName, actual.CodeName);
			Assert.AreEqual(expected.Description, actual.Description);
		}
	}
}
