using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Workflow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Passport;
using MCS.Library.OGUPermission;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects.Test.WfApplicationTest
{
	[TestClass]
	public class WfApplicationAuthTest
	{
		[Description("保存应用授权信息")]
		[TestMethod]
		[TestCategory(ProcessTestHelper.WfApplication)]
		public void SaveWfApplicationAuth()
		{
			WfApplicationAuth auth = PrepareData("秘书服务", "部门通知");

			WfApplicationAuthAdapter.Instance.Update(auth);

			WfApplicationAuth dataRead = WfApplicationAuthAdapter.Instance.Load(
				auth.ApplicationName,
				auth.ProgramName,
				auth.AuthType);

			Assert.IsNotNull(dataRead);
			Assert.AreEqual(auth.RoleID, dataRead.RoleID);
			Assert.AreEqual(auth.RoleDescription, dataRead.RoleDescription);
		}

		[Description("加载用户能够使用的应用授权信息")]
		[TestMethod]
		[TestCategory(ProcessTestHelper.WfApplication)]
		public void LoadUserApplicationAuthInfo()
		{
			IUser testUser = (IUser)OguObjectSettings.GetConfig().Objects["admin"].Object;

			WfApplicationAuthCollection authInfo = WfApplicationAuthAdapter.Instance.LoadUserApplicationAuthInfo(testUser);

			Console.WriteLine(authInfo.Count);
		}

		[Description("测试WfApplicationAuthCollection转换为WhereSqlClauseBuilder")]
		[TestMethod]
		[TestCategory(ProcessTestHelper.WfApplication)]
		public void AppProgramWhereBuilderTest()
		{
			WfApplicationAuthCollection auth = new WfApplicationAuthCollection();

			auth.Add(PrepareData("秘书服务", "部门通知"));
			auth.Add(PrepareData("秘书服务", "集团通知"));

			string sql = auth.GetApplicationAndProgramBuilder("APPLICATION_NAME", "PROGRAM_NAME").ToSqlString(TSqlBuilder.Instance);
			Console.WriteLine(sql);

			Assert.AreEqual("(APPLICATION_NAME = N'秘书服务' AND PROGRAM_NAME = N'部门通知') OR (APPLICATION_NAME = N'秘书服务' AND PROGRAM_NAME = N'集团通知')", sql);
		}

		private static WfApplicationAuth PrepareData(string appName, string progName)
		{
			WfApplicationAuth auth = new WfApplicationAuth();

			auth.ApplicationName = appName;
			auth.ProgramName = progName;
			auth.AuthType = WfApplicationAuthType.FormAdmin;

			OguRole role = new OguRole(RolesDefineConfig.GetConfig().RolesDefineCollection["testRole"].Roles);

			auth.RoleID = role.ID;
			auth.RoleDescription = role.FullCodeName;

			return auth;
		}
	}
}
