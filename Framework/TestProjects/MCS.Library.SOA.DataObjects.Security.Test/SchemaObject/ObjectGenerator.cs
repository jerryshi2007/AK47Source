using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Conditions;

namespace MCS.Library.SOA.DataObjects.Security.Test.SchemaObject
{
	public static class SCObjectGenerator
	{
		public static SCPermission PreparePermissionObject()
		{
			SCPermission permission = new SCPermission();

			permission.ID = UuidHelper.NewUuidString();
			permission.Name = "Great Permission";
			permission.CodeName = permission.ID;

			return permission;
		}

		public static SCRole PrepareRoleObject()
		{
			SCRole role = new SCRole();

			role.ID = UuidHelper.NewUuidString();
			role.Name = "Great Role";
			role.CodeName = role.ID;

			return role;
		}

		public static SCApplication PrepareApplicationObject()
		{
			SCApplication app = new SCApplication();

			app.ID = UuidHelper.NewUuidString();
			app.Name = "Great Applicaiton";
			app.CodeName = app.ID;

			return app;
		}

		public static SCGroup PrepareGroupObject()
		{
			SCGroup group = new SCGroup();

			group.ID = UuidHelper.NewUuidString();
			group.Name = "Great Group";
			group.CodeName = group.ID;

			return group;
		}

		public static SCUser PrepareUserObject()
		{
			SCUser user = new SCUser();

			user.ID = UuidHelper.NewUuidString();
			user.Name = "Great Shen Zheng";
			user.FirstName = "峥";
			user.LastName = "沈";
			user.CodeName = user.ID;

			return user;
		}

		public static SCUser PrepareUserObject(string lastName, string firstName, string codeName)
		{
			SCUser user = new SCUser();

			user.ID = UuidHelper.NewUuidString();
			user.FirstName = firstName;
			user.LastName = lastName;
			user.Name = lastName + firstName;
			user.DisplayName = user.Name;
			user.CodeName = codeName;

			return user;
		}

		public static SCOrganization PrepareOrganizationObject()
		{
			SCOrganization org = new SCOrganization();

			org.ID = UuidHelper.NewUuidString();
			org.Name = "Root Organization";
			org.CodeName = org.ID;

			return org;
		}

		public static T PrepareSCObject<T>(string name, string codeName) where T : SCBase, new()
		{
			T org = new T();

			org.ID = UuidHelper.NewUuidString();
			org.Name = name;
			org.DisplayName = name;
			org.CodeName = org.ID;

			return org;
		}

		public static SCOrganization PrepareOrganizationObject(string name, string codeName)
		{
			SCOrganization org = new SCOrganization();

			org.ID = UuidHelper.NewUuidString();
			org.Name = name;
			org.DisplayName = name;
			org.CodeName = org.ID;

			return org;
		}

		public static TestRoleData PrepareTestRoleWithOrgAndGroup()
		{
			return PrepareTestRoleWithOrgAndGroup(null);
		}

		/// <summary>
		/// 准备一个包含一个人员、一个组织、一个群组的角色，群组和组织中各包含一个人
		/// </summary>
		/// <param name="userInOrg"></param>
		/// <param name="userInGroup"></param>
		/// <param name="userInRole"></param>
		/// <param name="testOrg"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		public static TestRoleData PrepareTestRoleWithOrgAndGroup(Func<TestRoleData, string> builtInCondition)
		{
			TestRoleData result = new TestRoleData();

			string orgCodeName = UuidHelper.NewUuidString();
			result.Organization = PrepareOrganizationObject("RoleOrg-" + orgCodeName, orgCodeName);
			SCObjectOperations.Instance.AddOrganization(result.Organization, SCOrganization.GetRoot());

			result.UserInOrg = PrepareUserObject();
			SCObjectOperations.Instance.AddUser(result.UserInOrg, result.Organization);

			string sidelineOrgCodeName = UuidHelper.NewUuidString();
			result.SidelineOrganization = PrepareOrganizationObject("RoleOrg-" + sidelineOrgCodeName, sidelineOrgCodeName);
			SCObjectOperations.Instance.AddOrganization(result.SidelineOrganization, SCOrganization.GetRoot());

			result.SidelineUserInOrg = PrepareUserObject();
			SCObjectOperations.Instance.AddUser(result.SidelineUserInOrg, result.Organization);
			SCObjectOperations.Instance.AddUser(result.SidelineUserInOrg, result.SidelineOrganization);

			result.Group = PrepareGroupObject();
			SCObjectOperations.Instance.AddGroup(result.Group, SCOrganization.GetRoot());

			result.UserInGroup = PrepareUserObject();
			SCObjectOperations.Instance.AddUser(result.UserInGroup, SCOrganization.GetRoot());
			SCObjectOperations.Instance.AddUserToGroup(result.UserInGroup, result.Group);

			result.UserInConditionGroup = PrepareUserObject();
			SCObjectOperations.Instance.AddUser(result.UserInConditionGroup, SCOrganization.GetRoot());

			SCConditionOwner groupOwner = new SCConditionOwner();

			groupOwner.OwnerID = result.Group.ID;
			groupOwner.Conditions.Add(new SCCondition(string.Format("Users.CodeName == \"{0}\"", result.UserInConditionGroup.CodeName)));
			SCConditionAdapter.Instance.UpdateConditions(groupOwner);

			result.UserInRole = PrepareUserObject();
			SCObjectOperations.Instance.AddUser(result.UserInRole, SCOrganization.GetRoot());

			result.UserInConditionRole = PrepareUserObject();
			SCObjectOperations.Instance.AddUser(result.UserInConditionRole, SCOrganization.GetRoot());

			SCApplication app = PrepareSCObject<SCApplication>("应用Role", "应用" + UuidHelper.NewUuidString());
			SCObjectOperations.Instance.AddApplication(app);

			result.Role = PrepareRoleObject();

			SCObjectOperations.Instance.AddRole(result.Role, app);
			SCObjectOperations.Instance.AddMemberToRole(result.UserInRole, result.Role);
			SCObjectOperations.Instance.AddMemberToRole(result.Organization, result.Role);
			SCObjectOperations.Instance.AddMemberToRole(result.Group, result.Role);

			SCConditionOwner roleOwner = new SCConditionOwner();

			roleOwner.OwnerID = result.Role.ID;

			//增加一条错误的条件，!=是非法运算符
			roleOwner.Conditions.Add(new SCCondition(string.Format("Users.CodeName != \"{0}\"", result.UserInConditionRole.CodeName)));
			roleOwner.Conditions.Add(new SCCondition(string.Format("Users.CodeName == \"{0}\"", result.UserInConditionRole.CodeName)));

			SCConditionAdapter.Instance.UpdateConditions(roleOwner);

			result.BuiltInFunctionRole = PrepareRoleObject();

			SCObjectOperations.Instance.AddRole(result.BuiltInFunctionRole, app);
			
			string builtConditionExp = string.Empty;

			if (builtInCondition != null)
			{
				builtConditionExp = builtInCondition(result);
			}

			if (builtConditionExp.IsNotEmpty())
			{
				SCConditionOwner builtInRoleOwner = new SCConditionOwner();

				builtInRoleOwner.OwnerID = result.BuiltInFunctionRole.ID;

				builtInRoleOwner.Conditions.Add(new SCCondition(builtConditionExp));

				SCConditionAdapter.Instance.UpdateConditions(builtInRoleOwner);
			}

			return result;
		}

		public static SCGroup PrepareGroupWithConditions(out SCUser userInGroup, out SCUser userNotInGroup)
		{
			userInGroup = PrepareUserObject();
			SCObjectOperations.Instance.AddUser(userInGroup, SCOrganization.GetRoot());

			userNotInGroup = PrepareUserObject();
			SCObjectOperations.Instance.AddUser(userNotInGroup, SCOrganization.GetRoot());

			SCGroup group = PrepareGroupObject();
			SCObjectOperations.Instance.AddGroup(group, SCOrganization.GetRoot());

			SCConditionOwner owner = new SCConditionOwner();

			owner.OwnerID = group.ID;
			owner.Conditions.Add(new SCCondition(string.Format("Users.CodeName == \"{0}\"", userInGroup.CodeName)));

			SCConditionAdapter.Instance.UpdateConditions(owner);

			return group;
		}

		/// <summary>
		/// 构造一棵用于测试的多级组织机构树(每级10个对象，总共5级，11110个组织，10万用户)
		/// </summary>
		public static void PrepareHugeTestOrgTree()
		{
			SchemaObjectAdapter.Instance.ClearAllData();

			PrepareOneLevelTestData(SCOrganization.GetRoot(), 0, "0");
		}

		private static void PrepareOneLevelTestData(SCOrganization parent, int level, string levelPrefix)
		{
			if (level < 4)
			{
				for (int i = 0; i < 10; i++)
				{
					string newLevelPrefix = string.Format("{0}.{1}", levelPrefix, i);
					string name = string.Format("第{0}组织", newLevelPrefix);

					SCOrganization org = PrepareOrganizationObject(name, name);

					SCObjectOperations.Instance.AddOrganization(org, parent);

					PrepareOneLevelTestData(org, level + 1, newLevelPrefix);
				}
			}
			else
			{
				for (int i = 0; i < 10; i++)
				{
					string newLevelPrefix = string.Format("{0}.{1}", levelPrefix, i);
					string lastName = string.Format("第{0}", newLevelPrefix);

					SCUser user = PrepareUserObject(lastName, "用户", lastName + "用户");

					SCObjectOperations.Instance.AddUser(user, parent);
				}
			}
		}

		/// <summary>
		/// 构造一棵用于测试的简单组织机构树
		/// </summary>
		public static void PreareTestOrgTree()
		{
			SchemaObjectAdapter.Instance.ClearAllData();

			SCOrganization parent1 = PrepareOrganizationObject("第一组织", "第一组织");
			SCObjectOperations.Instance.AddOrganization(parent1, SCOrganization.GetRoot());

			SCObjectOperations.Instance.AddUser(PrepareUserObject("沈", "峥", "shenzheng"), parent1);
			SCObjectOperations.Instance.AddUser(PrepareUserObject("危", "仁飞", "weirf"), parent1);

			SCOrganization parent2 = PrepareOrganizationObject("第二组织", "第二组织");
			parent2.DisplayName = "第二组织(很多人)";
			SCObjectOperations.Instance.AddOrganization(parent2, SCOrganization.GetRoot());
			AddMultiUsers(parent2);

			SCOrganization parent3 = PrepareOrganizationObject("第三组织", "第三组织");
			SCObjectOperations.Instance.AddOrganization(parent3, SCOrganization.GetRoot());

			SCOrganization parent31 = PrepareOrganizationObject("第三.一组织", "第三.一组织");
			SCObjectOperations.Instance.AddOrganization(parent31, parent3);

			SCUser user1 = PrepareUserObject("金", "涛", "jintao");
			SCObjectOperations.Instance.AddUser(user1, parent31);

			SCUser user2 = PrepareUserObject("郑", "桂龙", "zhenggl");
			SCObjectOperations.Instance.AddUser(user2, parent31);

			SCOrganization parent32 = PrepareOrganizationObject("第三.二组织", "第三.二组织");
			SCObjectOperations.Instance.AddOrganization(parent32, parent3);

			SCUser user3 = PrepareUserObject("李", "琪", "liqi");
			SCObjectOperations.Instance.AddUser(user3, parent32);

			SCUser user4 = PrepareUserObject("徐", "磊", "xulei");
			SCObjectOperations.Instance.AddUser(user4, parent32);

			SCOrganization parent4 = PrepareOrganizationObject("第四组织", "第四组织");
			parent4.DisplayName = "第四组织(包含组)";
			SCObjectOperations.Instance.AddOrganization(parent4, SCOrganization.GetRoot());

			SCGroup group = PrepareSCObject<SCGroup>("包含领导的组", "包含领导的组");
			SCObjectOperations.Instance.AddGroup(group, parent4);

			SCObjectOperations.Instance.AddUserToGroup(user1, group);
			SCObjectOperations.Instance.AddUserToGroup(user2, group);

			SCGroup group2 = PrepareSCObject<SCGroup>("包含孩子们的组", "包含孩子们的组");
			SCObjectOperations.Instance.AddGroup(group2, parent4);

			//重复插入
			SCObjectOperations.Instance.AddUserToGroup(user3, group2);
			SCObjectOperations.Instance.AddUserToGroup(user3, group2);

			SCObjectOperations.Instance.AddUserToGroup(user4, group2);
			SCObjectOperations.Instance.AddUserToGroup(user4, group2);

			PrepareTestApplicationData();
		}

		private static void PrepareTestApplicationData()
		{
			SCApplication app1 = PrepareSCObject<SCApplication>("应用1", "应用1");
			SCObjectOperations.Instance.AddApplication(app1);

			SCRole role11 = PrepareSCObject<SCRole>("应用1角色1", "应用1角色1");
			SCObjectOperations.Instance.AddRole(role11, app1);

			SCRole role12 = PrepareSCObject<SCRole>("应用1角色2", "应用1角色2");
			SCObjectOperations.Instance.AddRole(role12, app1);

			SCPermission permission11 = PrepareSCObject<SCPermission>("应用1权限1", "应用1权限1");
			SCObjectOperations.Instance.AddPermission(permission11, app1);

			SCPermission permission12 = PrepareSCObject<SCPermission>("应用1权限2", "应用1权限2");
			SCObjectOperations.Instance.AddPermission(permission12, app1);

			SCApplication app2 = PrepareSCObject<SCApplication>("应用2", "应用2");
			SCObjectOperations.Instance.AddApplication(app2);

			SCRole role21 = PrepareSCObject<SCRole>("应用2角色1", "应用2角色1");
			SCObjectOperations.Instance.AddRole(role21, app2);

			SCRole role22 = PrepareSCObject<SCRole>("应用2角色2", "应用2角色2");
			SCObjectOperations.Instance.AddRole(role22, app2);

			SCPermission permission21 = PrepareSCObject<SCPermission>("应用2权限1", "应用2权限1");
			SCObjectOperations.Instance.AddPermission(permission21, app2);

			SCPermission permission22 = PrepareSCObject<SCPermission>("应用2权限2", "应用2权限2");
			SCObjectOperations.Instance.AddPermission(permission22, app2);
		}

		private static void AddMultiUsers(SCOrganization parent)
		{
			for (int i = 0; i < 200; i++)
			{
				string firstName = string.Format("仁{0}", i);

				SCObjectOperations.Instance.AddUser(PrepareUserObject("危", firstName, "危仁" + i), parent);
			}
		}

		/// <summary>
		/// 准备用于删除的数据
		/// </summary>
		internal static void PreareTestOguObjectForDelete()
		{
			//SchemaObjectAdapter.Instance.ReGenOUData();
			OguObjectGenerator1.Generate();
		}
	}

	public static class ImageGenerator
	{
		public static ImageProperty PrepareImage()
		{
			FileInfo file = new FileInfo("baijuan.jpg");
			var name = file.Name;
			name = name.Substring(name.LastIndexOf("\\") + 1);
			ImageProperty image = new ImageProperty();
			image.ID = UuidHelper.NewUuid().ToString();
			image.Name = name;
			image.NewName = name;
			image.Content = new MaterialContent();
			image.Content.FileName = name;

			using (Stream s = file.OpenRead())
			{
				image.Content.ContentData = new byte[s.Length];
				s.Read(image.Content.ContentData, 0, (int)s.Length);
			}

			return image;
		}
	}
}
