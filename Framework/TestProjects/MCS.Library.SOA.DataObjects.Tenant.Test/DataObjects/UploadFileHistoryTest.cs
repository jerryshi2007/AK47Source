using MCS.Library.Core;
using MCS.Library.OGUPermission;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MCS.Library.SOA.DataObjects.Tenant.Test.DataObjects
{
    [TestClass]
    public class UploadFileHistoryTest
    {
        /// <summary>
        /// 已经迁移到MCS.Library.SOA.DataObjects.Tenant.Test
        /// </summary>
        [TestMethod]
        [TestCategory("UploadFileHistory")]
        public void Insert()
        {
            UploadFileHistory history = new UploadFileHistory();

            history.Operator = null;
            history.OriginalFileName = UuidHelper.NewUuidString() + ".txt";

            history.ApplicationName = "App";
            history.ProgramName = "Prog";
            history.StatusText = "一切正常";
            history.Status = UploadFileHistoryStatus.Success;
            history.Operator = (IUser)OguObjectSettings.GetConfig().Objects["requestor"].Object;

            using (Stream stream = PrepareFileStream(history.OriginalFileName, history.OriginalFileName))
            {
                UploadFileHistoryAdapter.Instance.Insert(history, stream);
            }

            using (Stream stream = history.GetMaterialContentStream())
            {
                using (StreamReader sr = new StreamReader(history.GetMaterialContentStream()))
                {
                    string content = sr.ReadToEnd();

                    Assert.AreEqual(history.OriginalFileName, content);
                }
            }
        }

        private static Stream PrepareFileStream(string fileName, string content)
        {
            using (Stream fileStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    sw.Write(content);
                }
            }

            return new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }
    }
}
