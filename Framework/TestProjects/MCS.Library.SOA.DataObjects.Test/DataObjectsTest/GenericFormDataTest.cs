using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Test.DataObjectsTest
{
	[TestClass]
	public class GenericFormDataTest
	{
		[TestMethod]
		[TestCategory("GenericFormData")]
		public void UpdateFormData()
		{
			//准备并更新数据
			SampleFormData data = PrepareSampleData();
			SampleFormDataAdapter.Instance.Update(data);

			SampleFormData dataLoaded = SampleFormDataAdapter.Instance.Load(data.ID);

			Assert.AreEqual(data.ID, dataLoaded.ID);
			Assert.AreEqual(data.Subject, dataLoaded.Subject);
			Assert.AreEqual(data.Creator.ID, dataLoaded.Creator.ID);
			Assert.AreEqual(data.Creator.Name, dataLoaded.Creator.Name);
			Assert.AreEqual(data.StringProperty, dataLoaded.StringProperty);
			Assert.AreEqual(data.Creator.ID, dataLoaded.Creator.ID);
			Assert.AreEqual(data.Creator.Name, dataLoaded.Creator.Name);
			Assert.AreEqual(data.Creator.FullPath, dataLoaded.Creator.FullPath);
			Assert.AreEqual(data.SearchContent, dataLoaded.SearchContent);

			Assert.AreEqual(data.SubData.Count, dataLoaded.SubData.Count);

			for (int i = 0; i < data.SubData.Count; i++)
			{
				Assert.AreEqual(data.SubData[i].SubItemID, dataLoaded.SubData[i].SubItemID);
				Assert.AreEqual(data.SubData[i].Name, dataLoaded.SubData[i].Name);
			}
		}

		[TestMethod]
		[TestCategory("GenericFormData")]
		public void UpdateComplexFormData()
		{
			ComplexFormData data = PrepareComplexFormData();

			ComplexFormDataAdapter.Instance.Update(data);

			ComplexFormData dataLoaded = ComplexFormDataAdapter.Instance.Load(data.ID);

			Assert.AreEqual(data.ID, dataLoaded.ID);
			Assert.AreEqual(data.Subject, dataLoaded.Subject);
			Assert.AreEqual(data.Creator.ID, dataLoaded.Creator.ID);
			Assert.AreEqual(data.Creator.Name, dataLoaded.Creator.Name);
			Assert.AreEqual(data.StringProperty, dataLoaded.StringProperty);

			Assert.AreEqual(data.SubDataA.Count, dataLoaded.SubDataA.Count);
			Assert.AreEqual(data.SubDataB.Count, dataLoaded.SubDataB.Count);

			for (int i = 0; i < data.SubDataA.Count; i++)
				Assert.AreEqual(data.SubDataA[i].SubStringPropertyA, dataLoaded.SubDataA[i].SubStringPropertyA);

			for (int i = 0; i < data.SubDataB.Count; i++)
				Assert.AreEqual(data.SubDataB[i].SubStringPropertyB, dataLoaded.SubDataB[i].SubStringPropertyB);
		}

		[TestMethod]
		[TestCategory("GenericFormData")]
		public void ReplaceComplexFormData()
		{
			ComplexFormData data = PrepareComplexFormData();

			Console.WriteLine("Data ID={0}", data.ID);

			ComplexFormDataAdapter.Instance.Update(data);

			ComplexFormData dataLoaded = ComplexFormDataAdapter.Instance.Load(data.ID);

			for (int i = 0; i < dataLoaded.SubDataA.Count; i++)
				dataLoaded.SubDataA[i].SubStringPropertyA = "NewA";

			for (int i = 0; i < dataLoaded.SubDataA.Count; i++)
				dataLoaded.SubDataB[i].SubStringPropertyB = "NewB";

			ComplexFormDataAdapter.Instance.ReplaceRelativeData(dataLoaded.SubDataA);
			ComplexFormDataAdapter.Instance.ReplaceRelativeData(dataLoaded.SubDataB);

			dataLoaded = ComplexFormDataAdapter.Instance.Load(data.ID);

			for (int i = 0; i < dataLoaded.SubDataA.Count; i++)
				Assert.AreEqual("NewA", dataLoaded.SubDataA[i].SubStringPropertyA);

			for (int i = 0; i < dataLoaded.SubDataB.Count; i++)
				Assert.AreEqual("NewB", dataLoaded.SubDataB[i].SubStringPropertyB);
		}

		private static ComplexFormData PrepareComplexFormData()
		{
			ComplexFormData result = new ComplexFormData();

			result.ID = UuidHelper.NewUuidString();
			result.Subject = string.Format("测试ComplexFormData-{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;
			result.Creator = user;

			result.StringProperty = "Windows A,B,C";

			result.SubDataA.Add(new SubClassDataTypeA() { ResourceID = result.ID, Class = "SubDataA", SubStringPropertyA = "Data A1", SearchContent = "贾彦军" });
			result.SubDataA.Add(new SubClassDataTypeA() { ResourceID = result.ID, Class = "SubDataA", SubStringPropertyA = "Data A2" });

			result.SubDataB.Add(new SubClassDataTypeB() { ResourceID = result.ID, Class = "SubDataB", SubStringPropertyB = "Data B1", SearchContent = "徐磊" });
			result.SubDataB.Add(new SubClassDataTypeB() { ResourceID = result.ID, Class = "SubDataB", SubStringPropertyB = "Data B2" });

			return result;
		}

		private static SampleFormData PrepareSampleData()
		{
			SampleFormData result = new SampleFormData();

			result.ID = UuidHelper.NewUuidString();
			result.Subject = string.Format("测试FormData-{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
			result.SearchContent = "晏德智";

			IUser user = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;
			result.Creator = user;

			result.StringProperty = "Windows 7,8,9";

			SampleSubFormData sub1 = new SampleSubFormData();

			sub1.SubItemID = 1;
			sub1.Name = "Sub Data 1";

			result.SubData.Add(sub1);

			SampleSubFormData sub2 = new SampleSubFormData();

			sub2.SubItemID = 2;
			sub2.Name = "Sub Data 2";

			result.SubData.Add(sub2);

			return result;
		}
	}
}
