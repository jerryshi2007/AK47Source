#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	ZipFile.cs
// Remark	：	文件解压缩
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    梁东	    20070430		创建
// -------------------------------------------------
#endregion
#region using
using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
#endregion
namespace MCS.Library.Compression
{
    /// <summary>
    /// 文件压缩和解压
    /// </summary>
    internal class ZipFile : IDisposable
    {
        #region private field

        List<ZipEntry> zipEntries;		// The collection of entries

        private ZipReader thisReader;

        private ZipWriter thisWriter;

        private Stream baseStream;		// Stream to which the writer writes 
                                        // both header and data, the reader
                                        // reads this
        private string zipName;

        #endregion

        #region constructor
        /// <summary>
        /// 按文件名创建压缩文件
        /// </summary>
        /// <param name="mode">系统打开文件的方式</param>
        /// <param name="method">压缩方式</param>
        /// <param name="name">压缩文件名</param>
        public ZipFile(string name, byte method, FileMode mode)
        {
            this.zipName = name;

            this.baseStream = new FileStream(this.zipName, mode);
            this.thisWriter = new ZipWriter(baseStream);
            this.thisWriter.Method = method;

            this.zipEntries = new List<ZipEntry>();            
        }

        /// <summary>
        /// 按文件名打开压缩文件.
        /// </summary>
        /// <param name="name">压缩文件名</param>
        public ZipFile(string name)
        {
            this.zipName = name;

            this.baseStream = new FileStream(this.zipName, FileMode.Open);

			this.thisReader = new ZipReader(this.baseStream);//, Path.GetFileName(this.zipName));

            //zipEntries = new List<ZipEntry>();

            this.zipEntries = this.thisReader.GetAllEntries();

        }
        #endregion       

        #region public method

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileName">待压缩文件名</param>
        public void Add(string fileName)
        {
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentUICulture;
            if (fileName.ToLower(ci).Equals(this.zipName.ToLower(ci)))
            {
                return;
            }

            ZipEntry entry = new ZipEntry(fileName);
            this.thisWriter.Add(entry);
            this.zipEntries.Add(entry);
        }       

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="path">解压后的文件存放路径</param>
        public void ExtractAll(string path)
        {
            this.thisReader.ExtractAll(this.zipEntries, path);
        }

        /// <summary>
        /// 关闭压缩文件
        /// </summary>
        public void CloseWriter()
        {
            if (this.thisWriter != null)
                this.thisWriter.Finish(this.zipEntries);

            this.Close();
        }

        /// <summary>
        /// 关闭流
        /// </summary>
        public void Close()
        {
            if (this.baseStream != null)
            {
                this.baseStream.Close();
            }
        }        
        
        /// <summary>
        /// 实现IDisposable接口
        /// </summary>
        public void Dispose()
        {
            CloseWriter();

            if (this.baseStream != null)
                this.baseStream.Dispose();

			if (this.thisReader != null)
				this.thisReader.Dispose();

			if (this.thisWriter != null)
				this.thisWriter.Dispose();
        }
        #endregion

        #region public property
		///// <summary>
		///// Gets the entries of compressed files.
		///// </summary>
		///// <returns>
		///// Collection of ZipEntries
		///// </returns>
		//public List<ZipEntry> Entries
		//{
		//    get
		//    {
		//        return this.zipEntries;
		//    }
		//}
		///// <summary>
		///// compress
		///// </summary>
		//public byte Method
		//{
		//    get
		//    {
		//        return this.thisWriter.Method;
		//    }
		//}
        #endregion
    }
}
