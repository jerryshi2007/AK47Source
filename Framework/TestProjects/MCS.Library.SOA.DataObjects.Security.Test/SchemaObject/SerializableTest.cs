using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.SOA.DataObjects.Security.Test.SchemaObject
{
	[TestClass]
	public class SerializableTest
	{
		/// <summary>
		/// 测试集合的序列化
		/// </summary>
		[TestMethod]
		[TestCategory(Constants.SerializationCategory)]
		public void TestSerializeSchemaPropertyValueCollection()
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			SchemaPropertyValueCollection obj1 = new SchemaPropertyValueCollection();

			SCGroup group = SCObjectGenerator.PrepareGroupObject();

			foreach (string key in group.Properties.GetAllKeys())
			{
				obj1.Add(group.Properties[key]);
			}

			bf.Serialize(ms, obj1);
			ms.Seek(0, System.IO.SeekOrigin.Begin);

			var obj2 = (SchemaPropertyValueCollection)bf.Deserialize(ms);

			Assert.AreEqual(obj1.Count, obj2.Count);

			var keys1 = obj1.GetAllKeys();

			foreach (var key in keys1)
			{
				Assert.IsTrue(obj2.ContainsKey(key));
				Assert.AreEqual(obj1[key].StringValue, obj2[key].StringValue);
			}
		}

		[TestMethod]
		[TestCategory(Constants.SerializationCategory)]
		public void TestSerializeGroup()
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			SCGroup obj1 = SCObjectGenerator.PrepareGroupObject();
			obj1.CreateDate = DateTime.Now;
			obj1.VersionEndTime = new DateTime(567890);
			obj1.VersionEndTime = DateTime.MaxValue;
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bf.Serialize(ms, obj1);
			ms.Seek(0, System.IO.SeekOrigin.Begin);
			SCGroup obj2 = (SCGroup)bf.Deserialize(ms);

			CommonAssert(obj1, obj2);
		}

		[TestMethod]
		[TestCategory(Constants.SerializationCategory)]
		public void TestSerializeOrg()
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			var obj1 = SCObjectGenerator.PrepareOrganizationObject();

			obj1.CreateDate = DateTime.Now;
			obj1.VersionEndTime = new DateTime(567890);
			obj1.VersionEndTime = DateTime.MaxValue;

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bf.Serialize(ms, obj1);
			ms.Seek(0, System.IO.SeekOrigin.Begin);

			var obj2 = (SCOrganization)bf.Deserialize(ms);

			CommonAssert(obj1, obj2);
		}

		[TestMethod]
		[TestCategory(Constants.SerializationCategory)]
		public void TestSerializeUser()
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

			SCUser obj1 = SCObjectGenerator.PrepareUserObject();

			obj1.CreateDate = DateTime.Now;
			obj1.VersionEndTime = new DateTime(567890);
			obj1.VersionEndTime = DateTime.MaxValue;
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bf.Serialize(ms, obj1);

			ms.Seek(0, System.IO.SeekOrigin.Begin);

			SCUser obj2 = (SCUser)bf.Deserialize(ms);
			CommonAssert(obj1, obj2);
		}

		[TestMethod]
		[TestCategory(Constants.SerializationCategory)]
		public void TestSerializeRole()
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			SCRole obj1 = SCObjectGenerator.PrepareRoleObject();
			obj1.CreateDate = DateTime.Now;
			obj1.VersionEndTime = new DateTime(567890);
			obj1.VersionEndTime = DateTime.MaxValue;
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bf.Serialize(ms, obj1);
			ms.Seek(0, System.IO.SeekOrigin.Begin);
			SCRole obj2 = (SCRole)bf.Deserialize(ms);
			CommonAssert(obj1, obj2);
		}

		[TestMethod]
		[TestCategory(Constants.SerializationCategory)]
		public void TestSerializeApplication()
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			var obj1 = SCObjectGenerator.PrepareApplicationObject();
			obj1.CreateDate = DateTime.Now;
			obj1.VersionEndTime = new DateTime(567890);
			obj1.VersionEndTime = DateTime.MaxValue;
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bf.Serialize(ms, obj1);
			ms.Seek(0, System.IO.SeekOrigin.Begin);
			SCApplication obj2 = (SCApplication)bf.Deserialize(ms);
			CommonAssert(obj1, obj2);
		}

		[TestMethod]
		[TestCategory(Constants.SerializationCategory)]
		public void TestSerializePermission()
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			var obj1 = SCObjectGenerator.PreparePermissionObject();
			obj1.CreateDate = DateTime.Now;
			obj1.VersionEndTime = new DateTime(567890);
			obj1.VersionEndTime = DateTime.MaxValue;
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bf.Serialize(ms, obj1);
			ms.Seek(0, System.IO.SeekOrigin.Begin);
			var obj2 = (SCPermission)bf.Deserialize(ms);
			CommonAssert(obj1, obj2);
		}

		[TestMethod]
		[TestCategory(Constants.SerializationCategory)]
		public void TestSerializeRelation()
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			var org = SCObjectGenerator.PrepareOrganizationObject();
			var user = SCObjectGenerator.PrepareUserObject();
			SCRelationObject obj1 = new SCRelationObject(org, user);
			obj1.CreateDate = DateTime.Now;
			obj1.VersionEndTime = new DateTime(567890);
			obj1.VersionEndTime = DateTime.MaxValue;
			obj1.ID = MCS.Library.Core.UuidHelper.NewUuidString();
			obj1.InnerSort = 23;
			obj1.Status = SchemaObjectStatus.Deleted;
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bf.Serialize(ms, obj1);
			ms.Seek(0, System.IO.SeekOrigin.Begin);
			var obj2 = (SCRelationObject)bf.Deserialize(ms);
			RelationAssert(obj1, obj2);
		}

		[TestMethod]
		[TestCategory(Constants.SerializationCategory)]
		public void TestSerializeMemberRelation()
		{
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			var role = SCObjectGenerator.PrepareRoleObject();
			var user = SCObjectGenerator.PrepareUserObject();
			var obj1 = new SCMemberRelation(role, user);
			obj1.CreateDate = DateTime.Now;
			obj1.VersionEndTime = new DateTime(567890);
			obj1.VersionEndTime = DateTime.MaxValue;
			obj1.ID = MCS.Library.Core.UuidHelper.NewUuidString();
			obj1.InnerSort = 23;
			obj1.Status = SchemaObjectStatus.Deleted;
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			bf.Serialize(ms, obj1);
			ms.Seek(0, System.IO.SeekOrigin.Begin);
			var obj2 = (SCMemberRelation)bf.Deserialize(ms);
			RelationAssert(obj1, obj2);
		}

		private void RelationAssert(SCMemberRelation obj1, SCMemberRelation obj2)
		{
			Assert.IsNotNull(obj1);
			Assert.IsNotNull(obj2);
			Assert.IsTrue(obj1.GetType() == obj2.GetType());
			Assert.IsTrue(obj2.VersionEndTime == obj1.VersionEndTime);
			Assert.IsTrue(obj2.VersionStartTime == obj1.VersionStartTime);
			Assert.IsTrue(obj2.SchemaType == obj1.SchemaType);
			Assert.IsTrue(obj2.CreateDate == obj1.CreateDate);
			if (obj2.Creator != null && obj1.Creator != null)
			{
				Assert.AreEqual(obj2.Creator.ID, obj1.Creator.ID);
				Assert.AreEqual(obj2.Creator.DisplayName, obj2.Creator.DisplayName);
			}
			else
			{
				Assert.AreEqual(obj2.Creator, obj1.Creator); //两者或者都为空
			}

			Assert.IsTrue(obj1.ID == obj1.ID);
			Assert.IsTrue(obj2.Status == obj1.Status);
			foreach (string key in obj1.Properties.GetAllKeys())
			{
				Assert.AreEqual(obj2.Properties[key].StringValue, obj1.Properties[key].StringValue);
			}
		}

		private void RelationAssert(SCMemberRelation obj1, SCRelationObject obj2)
		{
			Assert.IsNotNull(obj1);
			Assert.IsNotNull(obj2);
			Assert.IsTrue(obj1.GetType() == obj2.GetType());
			Assert.IsTrue(obj2.VersionEndTime == obj1.VersionEndTime);
			Assert.IsTrue(obj2.VersionStartTime == obj1.VersionStartTime);
			Assert.IsTrue(obj2.SchemaType == obj1.SchemaType);
			Assert.IsTrue(obj2.CreateDate == obj1.CreateDate);
			if (obj2.Creator != null && obj1.Creator != null)
			{
				Assert.AreEqual(obj2.Creator.ID, obj1.Creator.ID);
				Assert.AreEqual(obj2.Creator.DisplayName, obj2.Creator.DisplayName);
			}
			else
			{
				Assert.AreEqual(obj2.Creator, obj1.Creator); //两者或者都为空
			}
			Assert.IsTrue(obj1.ID == obj1.ID);
			Assert.IsTrue(obj2.Status == obj1.Status);
			foreach (string key in obj1.Properties.GetAllKeys())
			{
				Assert.AreEqual(obj2.Properties[key].StringValue, obj1.Properties[key].StringValue);
			}
		}

		private void RelationAssert(SCRelationObject obj1, SCRelationObject obj2)
		{
			Assert.IsNotNull(obj1);
			Assert.IsNotNull(obj2);
			Assert.IsTrue(obj1.GetType() == obj2.GetType());
			Assert.IsTrue(obj2.VersionEndTime == obj1.VersionEndTime);
			Assert.IsTrue(obj2.VersionStartTime == obj1.VersionStartTime);
			Assert.IsTrue(obj2.SchemaType == obj1.SchemaType);
			Assert.IsTrue(obj2.CreateDate == obj1.CreateDate);
			if (obj2.Creator != null && obj1.Creator != null)
			{
				Assert.AreEqual(obj2.Creator.ID, obj1.Creator.ID);
				Assert.AreEqual(obj2.Creator.DisplayName, obj2.Creator.DisplayName);
			}
			else
			{
				Assert.AreEqual(obj2.Creator, obj1.Creator); //两者或者都为空
			}
			Assert.IsTrue(obj1.ID == obj1.ID);
			Assert.IsTrue(obj2.Status == obj1.Status);
			foreach (string key in obj1.Properties.GetAllKeys())
			{
				Assert.AreEqual(obj2.Properties[key].StringValue, obj1.Properties[key].StringValue);
			}
		}

		private static void CommonAssert(SCBase obj1, SCBase obj2)
		{
			Assert.IsNotNull(obj1);
			Assert.IsNotNull(obj2);
			Assert.IsTrue(obj1.GetType() == obj2.GetType());
			Assert.IsTrue(obj2.VersionEndTime == obj1.VersionEndTime);
			Assert.IsTrue(obj2.VersionStartTime == obj1.VersionStartTime);
			Assert.IsTrue(obj2.SchemaType == obj1.SchemaType);
			Assert.IsTrue(obj2.CodeName == obj1.CodeName);
			Assert.IsTrue(obj2.CreateDate == obj1.CreateDate);
			if (obj2.Creator != null && obj1.Creator != null)
			{
				Assert.AreEqual(obj2.Creator.ID, obj1.Creator.ID);
				Assert.AreEqual(obj2.Creator.DisplayName, obj2.Creator.DisplayName);
			}
			else
			{
				Assert.AreEqual(obj2.Creator, obj1.Creator); //两者或者都为空
			}
			Assert.IsTrue(obj1.ID == obj1.ID);
			Assert.IsTrue(obj2.Status == obj1.Status);
			Assert.IsTrue(obj2.Name == obj1.Name);
			foreach (string key in obj1.Properties.GetAllKeys())
			{
				Assert.AreEqual(obj2.Properties[key].StringValue, obj1.Properties[key].StringValue);
			}
		}
	}
}
