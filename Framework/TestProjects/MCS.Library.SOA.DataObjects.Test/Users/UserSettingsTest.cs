using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Library.SOA.DataObjects.Test.Users
{
	[TestClass]
	public class UserSettingsTest
	{
		[TestMethod]
		[Description("用户个人设置测试")]
		[TestCategory(UsersTestCommon.UserSettingsCategory)]
		public void UserSettingsReadTest()
		{
			UserSettings settings = UserSettings.GetSettings(OguObjectSettings.GetConfig().Objects["requestor"].Object.ID);

			settings.Categories["CommonSettings"].Properties["ToDoListPageSize"].StringValue = "15";

			Assert.AreEqual(100, settings.GetPropertyValue("CommonSettings", "NotExistsProperty", 100));
			Assert.AreEqual(100, settings.GetPropertyValue("NotExistsCategory", "NotExistsProperty", 100));

			settings.Update();

			Thread.Sleep(500);

			UserSettings settingsLoaded = UserSettings.GetSettings(settings.UserID);

			Assert.AreEqual(settings.Categories["CommonSettings"].Properties["ToDoListPageSize"].StringValue,
				settingsLoaded.Categories["CommonSettings"].Properties["ToDoListPageSize"].StringValue);
		}

		[TestMethod]
		[Description ("用户个人设置的JSON序列化")]
		[TestCategory(ProcessTestHelper.JSONConverter)]
		public void UserSettingsJSONConverter()
		{
			//No parameterless constructor defined for this object.
			WfConverterHelper.RegisterConverters();

			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

			UserSettings settings = UserSettings.GetDefaultSettings (user.ID);
			settings.Update();

			string result = JSONSerializerExecute.Serialize(settings);
			Console.WriteLine(result);

			UserSettings deserializedSettings = JSONSerializerExecute.Deserialize<UserSettings>(result);
			string reSerialized = JSONSerializerExecute.Serialize(deserializedSettings);
			Assert.AreEqual(result, reSerialized);
		}

	}
}
