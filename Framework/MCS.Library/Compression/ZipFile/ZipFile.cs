#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	ZipFile.cs
// Remark	��	�ļ���ѹ��
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
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
    /// �ļ�ѹ���ͽ�ѹ
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
        /// ���ļ�������ѹ���ļ�
        /// </summary>
        /// <param name="mode">ϵͳ���ļ��ķ�ʽ</param>
        /// <param name="method">ѹ����ʽ</param>
        /// <param name="name">ѹ���ļ���</param>
        public ZipFile(string name, byte method, FileMode mode)
        {
            this.zipName = name;

            this.baseStream = new FileStream(this.zipName, mode);
            this.thisWriter = new ZipWriter(baseStream);
            this.thisWriter.Method = method;

            this.zipEntries = new List<ZipEntry>();            
        }

        /// <summary>
        /// ���ļ�����ѹ���ļ�.
        /// </summary>
        /// <param name="name">ѹ���ļ���</param>
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
        /// ѹ���ļ�
        /// </summary>
        /// <param name="fileName">��ѹ���ļ���</param>
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
        /// ��ѹ�ļ�
        /// </summary>
        /// <param name="path">��ѹ����ļ����·��</param>
        public void ExtractAll(string path)
        {
            this.thisReader.ExtractAll(this.zipEntries, path);
        }

        /// <summary>
        /// �ر�ѹ���ļ�
        /// </summary>
        public void CloseWriter()
        {
            if (this.thisWriter != null)
                this.thisWriter.Finish(this.zipEntries);

            this.Close();
        }

        /// <summary>
        /// �ر���
        /// </summary>
        public void Close()
        {
            if (this.baseStream != null)
            {
                this.baseStream.Close();
            }
        }        
        
        /// <summary>
        /// ʵ��IDisposable�ӿ�
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
