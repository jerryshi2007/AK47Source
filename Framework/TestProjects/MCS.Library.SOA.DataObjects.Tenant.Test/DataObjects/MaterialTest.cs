using MCS.Library.Core;
using MCS.Library.OGUPermission;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.DataObjects
{
    [TestClass]
    public class MaterialTest
    {
        [TestMethod]
        [TestCategory("Material")]
        public void MaterialCloneTest()
        {
            Material source = PrepareSourceMaterial();
            Material dest = source.Clone();

            ValidateMaterial(source, dest);
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
