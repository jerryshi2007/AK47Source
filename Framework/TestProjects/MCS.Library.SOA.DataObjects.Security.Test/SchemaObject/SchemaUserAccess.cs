using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.Actions;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Web.Library.Script;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Security.Test.SchemaObject
{
	[TestClass]
	public class SchemaUserAccess
	{
		[TestMethod]
		[Description("读写User对象的属性，用于测试Schema属性加载为PropertyValue以及属性的访问")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void GetSetSchemaUserPropertiesTest()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			Assert.AreEqual("Great Shen Zheng", user.Name);
			Assert.AreEqual("峥", user.FirstName);
			Assert.AreEqual("沈", user.LastName);
		}

		[TestMethod]
		[Description("将User序列化为xml")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void SchemaUserToXml()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			XElement userElem = new XElement("Root").AddChildElement("Object");

			user.ToXElement(userElem);

			Console.WriteLine(userElem.Parent.ToString());

			SCUser userDeserialized = new SCUser();

			userDeserialized.FromXElement(userElem);

			Assert.AreEqual(user.ID, userDeserialized.ID);
			Assert.AreEqual(user.Name, userDeserialized.Name);
			Assert.AreEqual(user.FirstName, userDeserialized.FirstName);
			Assert.AreEqual(user.LastName, userDeserialized.LastName);
		}

		[TestMethod]
		[Description("保存新User到数据库，然后读出来校验是否正确还原")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void PersistSchemaUser()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			user.Properties.TrySetValue("CadreType", MCS.Library.OGUPermission.UserAttributesType.JiaoLiuGanBu);

			SCActionContext.Current.DoActions(() => SchemaObjectAdapter.Instance.Update(user));

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			Console.WriteLine(user.Properties.GetValue("CadreType", MCS.Library.OGUPermission.UserAttributesType.Unspecified));

			Assert.IsNotNull(userLoaded);
			Assert.AreEqual(user.ID, userLoaded.ID);
			Assert.AreEqual(user.Name, userLoaded.Name);
			Assert.AreEqual(user.FirstName, userLoaded.FirstName);
			Assert.AreEqual(user.LastName, userLoaded.LastName);
		}

		[TestMethod]
		[Description("保存新User到数据库，然后调用设置状态的方法来标记为删除")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void SetUserDeletedStatus()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCActionContext.Current.DoActions(() => SchemaObjectAdapter.Instance.Update(user));

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			SCActionContext.Current.DoActions(() => SchemaObjectAdapter.Instance.UpdateStatus(userLoaded, SchemaObjectStatus.Deleted));

			Console.WriteLine(userLoaded.ID);
			SCUser userLoaded2 = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			Assert.AreEqual(SchemaObjectStatus.Deleted, userLoaded2.Status);
		}

		[TestMethod]
		[Description("保存保存两次同一个ID的User到数据库，然后读出来校验是否正确还原")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void DoubleInsertSameSchemaUser()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCActionContext.Current.DoActions(() => SchemaObjectAdapter.Instance.Update(user));

			user.Name = "一般的沈峥";

			SCActionContext.Current.DoActions(() => SchemaObjectAdapter.Instance.Update(user));

			SCUser userLoaded = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			Assert.IsNotNull(userLoaded);
			Assert.AreEqual(user.ID, userLoaded.ID);
			Assert.AreEqual(user.Name, userLoaded.Name);
			Assert.AreEqual(user.FirstName, userLoaded.FirstName);
			Assert.AreEqual(user.LastName, userLoaded.LastName);
		}

		[TestMethod]
		[Description("保存两次User对象到数据库，会形成两个版本，校验每个版本的结果是否正确")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void DuplicatePersistSchemaUser()
		{
			SCUser user = SCObjectGenerator.PrepareUserObject();

			string firstUserName = user.Name;

			//更新一次，插入最原始的版本
			SCActionContext.Current.DoActions(() => SchemaObjectAdapter.Instance.Update(user));

			//加载最原始的版本
			SCUser originalLoadedUser = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);
			originalLoadedUser.Name = "沈峥";

			//再更新一次，获取新版本
			SCActionContext.Current.DoActions(() => SchemaObjectAdapter.Instance.Update(originalLoadedUser));

			SCUser firstLoadedUser = (SCUser)SchemaObjectAdapter.Instance.Load(originalLoadedUser.ID, originalLoadedUser.VersionStartTime);
			SCUser currentLoadedUser = (SCUser)SchemaObjectAdapter.Instance.Load(originalLoadedUser.ID);

			Assert.AreEqual(firstUserName, firstLoadedUser.Name);
			Assert.AreEqual(originalLoadedUser.Name, currentLoadedUser.Name);
		}

		[TestMethod]
		[Description("更新User图片，然后读取，比较图片内容是否一致")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void UpdateUserImage()
		{
			var userImagePropName = "PhotoKey";

			SCUser user = SCObjectGenerator.PrepareUserObject();

			SCObjectOperations.Instance.AddUser(user, SCOrganization.GetRoot());

			ImageProperty image = ImageGenerator.PrepareImage();
			image.ResourceID = user.ID;

			Assert.IsTrue(CanSerialize(image));

			SCObjectOperations.Instance.UpdateObjectImageProperty(user, userImagePropName, image);

			SCUser userLoad = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);
			Assert.IsNotNull(userLoad);

			var imageLoad = JSONSerializerExecute.Deserialize<ImageProperty>(userLoad.Properties[userImagePropName].StringValue);
			MaterialContentCollection mcc = MaterialContentAdapter.Instance.Load(builder => builder.AppendItem("CONTENT_ID", imageLoad.ID));

			Assert.AreEqual(image.Name, imageLoad.Name);
			Assert.IsNotNull(mcc);
			Assert.AreEqual(1, mcc.Count);
			Assert.AreEqual(image.Content.ContentData.Length, mcc[0].ContentData.Length);

			SCObjectOperations.Instance.UpdateObjectImageProperty(user, userImagePropName, null);
			userLoad = (SCUser)SchemaObjectAdapter.Instance.Load(user.ID);

			Assert.AreEqual("", userLoad.Properties[userImagePropName].StringValue);
		}

		[TestMethod]
		[Description("根据XPath，查询出用户对象")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void LoadUserByXPathTest()
		{
			SchemaObjectAdapter.Instance.ClearAllData();

			SCUser user = SCObjectGenerator.PrepareUserObject();

			user.FirstName = "SZ's brother";
			string firstName = user.FirstName;

			//更新一次，插入最原始的版本
			SCActionContext.Current.DoActions(() => SchemaObjectAdapter.Instance.Update(user));

			string xPath = string.Format("Object[@FirstName=\"{0}\"]", firstName);

			SchemaObjectBase loadedUser = SchemaObjectAdapter.Instance.LoadByXPath(xPath, new string[] { "Users" }, DateTime.MinValue).FirstOrDefault();

			Assert.IsNotNull(loadedUser);
			Assert.AreEqual(firstName, loadedUser.Properties.GetValue("FirstName", string.Empty));
		}

		private bool CanSerialize(ImageProperty image)
		{
			try
			{
				SerializationHelper.SerializeObjectToString(image, SerializationFormatterType.Binary);
				SerializationHelper.SerializeObjectToString(image, SerializationFormatterType.Soap);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
