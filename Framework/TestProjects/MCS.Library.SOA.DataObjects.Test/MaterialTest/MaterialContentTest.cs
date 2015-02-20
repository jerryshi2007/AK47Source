using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Test.MaterialTest
{
	[TestClass]
	public class MaterialContentTest
	{
		[TestMethod]
		[TestCategory("Material")]
		public void UpdateMaterialContent()
		{
			const string template = "Hello world !";

			MaterialContent content = new MaterialContent();

			content.ContentID = UuidHelper.NewUuidString();
			content.RelativeID = UuidHelper.NewUuidString();
			content.ContentData = PrepareSimpleData(template);

			MaterialContentAdapter.Instance.Update(content);

			MaterialContentCollection contentLoaded = MaterialContentAdapter.Instance.Load(builder => builder.AppendItem("CONTENT_ID", content.ContentID));

			Assert.IsTrue(contentLoaded.Count > 0);

			string dataRead = Encoding.UTF8.GetString(contentLoaded[0].ContentData);

			Assert.AreEqual(template, dataRead);
		}

		private static byte[] PrepareSimpleData(string data)
		{
			return Encoding.UTF8.GetBytes(data);
		}
	}
}
