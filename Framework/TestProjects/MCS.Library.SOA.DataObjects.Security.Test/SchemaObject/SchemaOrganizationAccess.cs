using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MCS.Library.SOA.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using System.Xml.Linq;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Converters;
using MCS.Library.SOA.DataObjects.Security.Executors;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Schemas.Actions;

namespace MCS.Library.SOA.DataObjects.Security.Test.SchemaObject
{
	[TestClass]
	public class SchemaOrganizationAccess
	{
		[TestMethod]
		[Description("读写Organization对象的属性，用于测试Schema属性加载为PropertyValue以及属性的访问")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void GetSetSchemaOrganizationPropertiesTest()
		{
			SCOrganization org = SCObjectGenerator.PrepareOrganizationObject();

			Assert.AreEqual("Root Organization", org.Name);
		}

		[TestMethod]
		[Description("将Organization序列化为xml")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void SchemaOrganizationToXml()
		{
			SCOrganization org = SCObjectGenerator.PrepareOrganizationObject();

			XElement orgElem = new XElement("Root").AddChildElement("Object");

			org.ToXElement(orgElem);

			Console.WriteLine(orgElem.Parent.ToString());

			SCOrganization orgDeserialized = new SCOrganization();

			orgDeserialized.FromXElement(orgElem);

			Assert.AreEqual(org.ID, orgDeserialized.ID);
			Assert.AreEqual(org.Name, orgDeserialized.Name);
		}

		[TestMethod]
		[Description("将Organization序列化为简单Json")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void SchemaOrganizationToSimpleJson()
		{
			SCOrganization org = SCObjectGenerator.PrepareOrganizationObject();

			JSONSerializerExecute.RegisterConverter(typeof(SchemaObjectSimpleConverter));

			string serializedData = JSONSerializerExecute.Serialize(org);

			Console.WriteLine(serializedData);

			SCOrganization deserializedOrg = JSONSerializerExecute.Deserialize<SCOrganization>(serializedData);

			Assert.AreEqual(org.ID, deserializedOrg.ID);
			Assert.AreEqual(org.Name, deserializedOrg.Name);
			Assert.AreEqual(org.Properties.GetValue("CodeName", string.Empty), deserializedOrg.Properties.GetValue("CodeName", string.Empty));
			Assert.AreEqual(org.Properties.GetValue("DisplayName", string.Empty), deserializedOrg.Properties.GetValue("DisplayName", string.Empty));
			Assert.AreEqual(org.Properties.GetValue("Description", string.Empty), deserializedOrg.Properties.GetValue("Description", string.Empty));
		}

		[TestMethod]
		[Description("保存两次Organization对象到数据库，会形成两个版本，校验每个版本的结果是否正确")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void DuplicatePersistSchemaUser()
		{
			SCOrganization org = SCObjectGenerator.PrepareOrganizationObject();

			string firstOrgName = org.Name;

			//更新一次，插入最原始的版本
			SCActionContext.Current.DoActions(() => SchemaObjectAdapter.Instance.Update(org));

			//加载最原始的版本
			SCOrganization originalLoadedOrg = (SCOrganization)SchemaObjectAdapter.Instance.Load(org.ID);
			originalLoadedOrg.Name = "根组织";

			//再更新一次，获取新版本
			SCActionContext.Current.DoActions(() => SchemaObjectAdapter.Instance.Update(originalLoadedOrg));

			SCOrganization firstLoadedOrg = (SCOrganization)SchemaObjectAdapter.Instance.Load(originalLoadedOrg.ID, originalLoadedOrg.VersionStartTime);
			SCOrganization currentLoadedOrg = (SCOrganization)SchemaObjectAdapter.Instance.Load(originalLoadedOrg.ID);

			Assert.AreEqual(firstOrgName, firstLoadedOrg.Name);
			Assert.AreEqual(originalLoadedOrg.Name, currentLoadedOrg.Name);
		}

		[TestMethod]
		[Description("加载组织对象，然后转换成简单对象，并且初始化Name和DisplayName；如果DisplayName属性为空，则DisplayName为Name")]
		[TestCategory(Constants.SchemaObjectCategory)]
		public void LoadSimpleObjectTest()
		{
			SCOrganization root = SCOrganization.GetRoot();

			SCOrganization newOrg = SCObjectGenerator.PrepareOrganizationObject();

			SCObjectOperations.Instance.AddOrganization(newOrg, root);

			root.ClearRelativeData();
			SCSimpleObjectCollection simpleChildren = root.CurrentChildren.ToSimpleObjects();

			SCSimpleObject simpleObj = simpleChildren.Find(obj => obj.ID == newOrg.ID);

			Console.WriteLine(simpleObj.DisplayName);

			Assert.IsNotNull(simpleObj);
			Assert.AreEqual(simpleObj.Name, simpleObj.DisplayName);
		}
	}
}
