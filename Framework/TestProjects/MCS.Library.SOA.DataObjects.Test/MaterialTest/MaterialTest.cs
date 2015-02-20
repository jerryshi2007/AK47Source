using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.OGUPermission;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects.Test.MaterialTest
{
	/// <summary>
	/// 附件的相关测试，包括VersionStartTime测试
	/// </summary>
	[TestClass]
	public class MaterialTest
	{
		[TestMethod]
		[TestCategory("Material")]
		[Description("附件的复制测试（纯内存）")]
		public void MaterialCloneTest()
		{
			Material source = PrepareSourceMaterial();
			Material dest = source.Clone();

			ValidateMaterial(source, dest);
		}

		[TestMethod]
		[TestCategory("Material")]
		[Description("附件的复制测试（保存到主题数据库中）")]
		public void MaterialCloneAndPersistTest()
		{
			Material source = PrepareSourceMaterial();
			Material dest = source.Clone();

			MaterialList list = new MaterialList();

			list.Add(dest);

			DbConnectionMappingContext.DoMappingAction(
				MaterialAdapter.Instance.GetConnectionName(), "SubjectDB",
				() =>
				{
					TransactionScopeFactory.DoAction(() =>
					{
						MaterialAdapter.Instance.InsertWithContent(list);

						MaterialList loadedList = MaterialAdapter.Instance.LoadMaterialByMaterialID(dest.ID);

						Assert.IsTrue(loadedList.Count > 0);

						loadedList[0].EnsureMaterialContent();
						ValidateMaterial(source, loadedList[0]);
					});
				});
		}

		private static void ValidateMaterial(Material source, Material dest)
		{
			Assert.AreEqual(source.ID, dest.ID);
			Assert.AreEqual(source.ResourceID, dest.ResourceID);
			Assert.AreEqual(source.Content.ContentData.Length, dest.Content.ContentData.Length);
		}

		private static Material PrepareSourceMaterial()
		{
			Material m = new Material();

			m.ID = UuidHelper.NewUuidString();
			m.MaterialClass = "MaterialTest";
			m.OriginalName = "MaterialTest.txt";
			m.RelativeFilePath = "MaterialTest.txt";
			m.ResourceID = UuidHelper.NewUuidString();
			m.Title = "Material Test";
			m.CreateDateTime = DateTime.Now;
			m.Creator = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;
			m.LastUploadTag = UuidHelper.NewUuidString();

			m.Content = PrepareMaterialContent(m.ID, m.ResourceID);

			return m;
		}

		private static MaterialContent PrepareMaterialContent(string contentID, string relativeID)
		{
			MaterialContent content = new MaterialContent();

			content.ContentID = contentID;
			content.RelativeID = relativeID;
			content.Class = "MaterialTest";

			string data = string.Format("Have a good day '{0}'!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
			content.ContentData = Encoding.UTF8.GetBytes(data);

			return content;
		}
	}
}
