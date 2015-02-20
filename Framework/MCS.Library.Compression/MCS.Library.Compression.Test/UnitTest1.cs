using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.Compression.Test
{
    [TestClass]
    public class UnitTest1
    {
        public TestContext TestContext { get; set; }

        /// <summary>
        /// compress files to archive file-compressfile.zip 
        /// </summary>
        [TestMethod]
        public void TestCompressFile()
        {
            const string zipPath = @"D:\Temp\compressfile.zip";
            var fileInfos = new List<ZipFileInfo>
                                {
                                    new ZipFileInfo(){Filename = @"D:\Temp\gpl.txt",Password = "123",Comment = System.DateTime.Now.ToString("G")},
                                    new ZipFileInfo(){Filename = @"D:\Temp\ru_office.txt",Password = "123"}
                                };
            CompressManager.CompressFile(zipPath, fileInfos);

            Assert.IsTrue(File.Exists(zipPath));
        }

        [TestMethod]
        public void TextExtractFile()
        {
            const string zipPath = @"D:\Temp\compressfile.zip";
            CompressManager.Extract(zipPath, @"D:\Temp\compressfile", true,"123");
            Assert.IsTrue(Directory.Exists(@"D:\Temp\compressfile"));
        }

        [TestMethod]
        public void TestCompressDirectory()
        {
            const string zipPath = @"D:\Temp\compressdirectory.zip";
            CompressManager.CompressDirectory(zipPath, @"D:\Temp\dotnetzip-85217");
            Assert.IsTrue(File.Exists(zipPath));
        }

        [TestMethod]
        public void TestExtractDirectory()
        {
            const string zipPath = @"D:\Temp\compressdirectory.zip";
            CompressManager.Extract(zipPath, @"D:\Temp\compressdirectory",true);
            Assert.IsTrue(Directory.Exists(@"D:\Temp\compressdirectory"));
        }

        [TestMethod]
        public void TestCompressStream()
        {
            using (var stream = OrginolStream())
            {
                using (var compressStream = GetCompressStream())
                {
                    Assert.IsNotNull(compressStream.Length);
                    Assert.IsTrue(stream.Length > compressStream.Length);
                }
            }
        }

        [TestMethod]
        public void TestAddStreamtoArchive()
        {
            var zipPath = @"D:\Temp\08022009.zip";
            using (var stream = OrginolStream())
            {
                CompressManager.AddStreamToArchive(zipPath, stream, @"D:\Temp\stream.bin");
            }                        
        }

        private static Stream OrginolStream()
        {
            var stream = new FileStream(@"D:\Temp\08022009.jpg", FileMode.Open, FileAccess.Read);
            return stream;
        }

        private static Stream GetCompressStream()
        {
            var stream = OrginolStream();
            var compressStream = CompressManager.CompressStream(stream);
            return compressStream;
        }

        [TestMethod]
        public void TestExtractStream()
        {
            using (var stream = OrginolStream())
            {
                using (var compressStream = GetCompressStream())
                {
                    var extractStream = CompressManager.ExtractStream(compressStream);
                    Assert.IsNotNull(extractStream);
                    Assert.AreEqual(stream.Length,extractStream.Length);
                }
            }
        }

        [TestMethod]
        public void TestGetStreamFromArchive()
        {
            const string zipPath = @"D:\Temp\08022009.zip";
             using(var stream =CompressManager.GetStreamFromArchive(zipPath, "stream.bin"))
             {
             }
        }

        [TestMethod]
        public void TestRemoveItemFromArchive()
        {
            const string zipPath = @"D:\Temp\compressfile.zip";
            CompressManager.RemvoeEntry(zipPath, "gpl.txt");
        }

        [TestMethod]
        public void TestRenameItemFromArchive()
        {
            const string zipPath = @"D:\Temp\compressfile.zip";
            CompressManager.RenameEntry(zipPath,"luck.txt" ,"gpl.txt");
        }
    }
}
