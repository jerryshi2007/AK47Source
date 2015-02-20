#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	ZipWriter.cs
// Remark	��	ѹ���ļ�
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

using MCS.Library.Core;
using MCS.Library.Properties;

namespace MCS.Library.Compression
{
    /// <summary>
    /// ѹ���ļ�
    /// </summary>
#if DELUXEWORKSTEST
    public class ZipWriter : IDisposable
#else
    internal class ZipWriter : IDisposable
#endif
    {
        #region private field
        private Crc32 crc = new Crc32();
        
        //private Stream zipStream;	    	//The zip stream which is instantiated
                                            //and closed after every operation
        private Stream baseStream;		    //The base stream from which the header 
                                            //and compressed data are read

        private byte[] zipComment = new byte[0];

        private long offset;
        private byte method;        

        #endregion

        #region constructor
        /// <summary>
        /// ��������ѹ���ļ�
        /// </summary>
        /// <param name="fileStream">
        /// ����д�ļ�����
        /// </param>
        public ZipWriter(Stream fileStream)
        {
            this.baseStream = fileStream;
        }
        #endregion

        #region private method
        
		///// <summary>
		///// Writes a stream of bytes into the stream.
		///// </summary>
		//private void WriteBytes(byte[] value)
		//{
		//    foreach (byte b in value)
		//        this.baseStream.WriteByte(b);
		//}

        /// <summary>
        /// Write an int16 in little endian byte order.
        /// </summary>
        private void WriteLeInt16(Int16 value)
        {
            this.baseStream.WriteByte((byte)value);
            this.baseStream.WriteByte((byte)(value >> 8));
        }

        /// <summary>
        /// Write an int32 in little endian byte order.
        /// </summary>
        private void WriteLeInt32(Int32 value)
        {
            WriteLeInt16((Int16)value);
            WriteLeInt16((Int16)(value >> 16));
        }

        /// <summary>
        /// Puts the next header in a predefined order
        /// </summary>
        /// <param name="entry">
        /// the ZipEntry which contains all the information
        /// </param>
        private void PutNextHeader(ZipEntry entry)
        {
            entry.Offset = (int)this.offset;

            // Write the local file header
            WriteLeInt32(ZipConstants.LOCSIG);                        //�ļ�ͷ���                  4 bytes  (0x04034b50)
            WriteLeInt16(20);                                         //��ѹ�ļ����� pkware �汾    2 bytes
            WriteLeInt16(0);                                          // ȫ�ַ�ʽλ���              2 bytes
            WriteLeInt16((byte)this.method);                               //ѹ����ʽ                    2 bytes
            WriteLeInt32(entry.DosTime);                              //����޸��ļ�ʱ��             2 bytes+����޸��ļ�����             2 bytes
            this.offset = this.baseStream.Position;
            WriteLeInt32(0);                                          //CRC-32У��                  4 bytes
            WriteLeInt32(0);                                          //ѹ����ߴ�                  4 bytes
            WriteLeInt32(0);                                          //δѹ���ߴ�                  4 bytes

            byte[] names = ZipConstants.ConvertToArray(Path.GetFileName(entry.Name));
            byte[] extra = new byte[0];                               //byte[] extra = entry.ExtraData;

            WriteLeInt16((short)names.Length);                         //�ļ�������                  2 bytes
            WriteLeInt16((short)extra.Length);                         //��չ��¼����                2 bytes            
            this.baseStream.Write(names, 0, names.Length);                  //�ļ���                     ���������ȣ�
            this.baseStream.Write(extra, 0, extra.Length);                  //��չ�ֶ�                   ���������ȣ�
            
            this.crc.Reset();
        }

        /// <summary>
        /// Writes the compressed data into the basestream 
        /// It instantiates a memory stream which will serve 
        /// as a temp store and then compresses it using Gzip Stream or
        /// Deflate stream and writes it to the base stream
        /// </summary>
        private void WriteCompressedFile(FileStream fStream, ZipEntry entry)
        {
            MemoryStream ms = new MemoryStream();

            ExceptionHelper.FalseThrow(this.method == ZipConstants.DEFLATE || this.method == ZipConstants.GZIP, CompressionRes.ResourceManager.GetString(CompressionError.InvalidMethod.ToString()));

            byte[] buffer = new byte[fStream.Length];
            fStream.Read(buffer, 0, buffer.Length);

            Stream zipStream = null;	    	//The zip stream which is instantiated
            if (this.method == ZipConstants.DEFLATE)
                zipStream = new DeflateStream(ms, CompressionMode.Compress, true);
            else// if (method == ZipConstants.GZIP)
                zipStream = new GZipStream(ms, CompressionMode.Compress, true);

            using (zipStream as IDisposable)
            {
                zipStream.Write(buffer, 0, buffer.Length);
            }

            byte[] b = new byte[ms.Length];

            ms.Seek(0, SeekOrigin.Begin);

            ms.Read(b, 0, b.Length);

            this.baseStream.Write(b, 0, b.Length);

            WriteCompressedSizeCRC((int)ms.Length, b, entry); //Go back and write the length and the CRC
        }

        private void WriteCompressedSizeCRC(Int32 value, byte[] buff, ZipEntry entry)
        {
            entry.CompressedSize = value;
            this.crc.Update(buff);
            entry.Crc = this.crc.Value;
            this.baseStream.Seek(this.offset, SeekOrigin.Begin);
            WriteLeInt32((int)entry.Crc);
            WriteLeInt32(entry.CompressedSize);
            WriteLeInt32(entry.Size);            
            this.baseStream.Seek(0, SeekOrigin.End);
            this.offset = this.baseStream.Position;
        }
       
        #endregion

        #region public method
        
        /// <summary>
        /// �����ļ�ZipEntry������ѹ���ļ�
        /// </summary>
        /// <param name="entry">��ѹ���ļ����ļ���Ϣ</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\ZipWriterTest.cs" region="AddTest" lang="cs" title="�����ļ�ZipEntry������ѹ���ļ�" /> 
        /// </remarks>
        public void Add(ZipEntry entry)
        {
            if (entry != null)
                using (FileStream fs = File.OpenRead(entry.Name))
                {
                    entry.Size = (Int32)fs.Length;
                    entry.DateTime = File.GetLastWriteTime(entry.Name);
                    PutNextHeader(entry);
                    WriteCompressedFile(fs, entry);
                }
        }
        
        /// <summary>
        /// ����ѹ���ļ�������־
        /// </summary>
        /// <param name="entries">��ѹ���ļ����ļ���Ϣ</param>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\ZipWriterTest.cs" region="FinishTest" lang="cs" title="����ѹ���ļ�������־" /> 
        /// </remarks>
        public void Finish(List<ZipEntry> entries)
        {
            if (entries == null) return;

            int numEntries = 0;
            int sizeEntries = 0;
            // ѹ��Դ�ļ�Ŀ¼��
            foreach (ZipEntry entry in entries)
            {
                if (entry == null) return;

                WriteLeInt32(ZipConstants.CENSIG);                      //Ŀ¼���ļ��ļ�ͷ���             4 bytes  (0x02014b50)
                WriteLeInt16(20);                                       //ѹ��ʹ�õġ�pkware �汾          2 bytes
                WriteLeInt16(20);                                       //��ѹ�ļ����� pkware �汾         2 bytes
                WriteLeInt16(0);                                        //ȫ�ַ�ʽλ���                   2 bytes
                WriteLeInt16((byte)this.method);                             //ѹ����ʽ                        2 bytes
                WriteLeInt32((int)entry.DosTime);                       //����޸��ļ�ʱ��                 2 bytes+����޸��ļ�����                 2 bytes
                WriteLeInt32((int)entry.Crc);                           //�ãңã�����У��                 4 bytes
                WriteLeInt32((int)entry.CompressedSize);                //ѹ����ߴ�                      4 bytes
                WriteLeInt32((int)entry.Size);                          //δѹ���ߴ�                      4 bytes


                byte[] name = ZipConstants.ConvertToArray(Path.GetFileName(entry.Name));
                WriteLeInt16((short)name.Length);                       //�ļ�������                      2 bytes
                byte[] extra = new byte[0];//byte[] extra = entry.ExtraData;

                byte[] entryComment = new byte[0];//byte[] entryComment = entry.Comment != null ? ZipConstants.ConvertToArray(entry.Comment) : new byte[0];

                WriteLeInt16((short)extra.Length);                                        //��չ�ֶγ���                    2 bytes
                WriteLeInt16((short)entryComment.Length);                                        //�ļ�ע�ͳ���                    2 bytes
                WriteLeInt16(0);	// disk number  �ļ�ע�ͳ���                    2 bytes
                WriteLeInt16(0);	// internal file attr       �ڲ��ļ�����                    2 bytes

                WriteLeInt32(0);     //�ⲿ�ļ�����                    4 bytes   

                WriteLeInt32(entry.Offset); //�ֲ�ͷ��ƫ����                  4 bytes

                this.baseStream.Write(name, 0, name.Length);                   //�ļ���                       ���������ȣ�
                this.baseStream.Write(extra, 0, extra.Length);                 //��չ�ֶ�                     ���������ȣ�
                this.baseStream.Write(entryComment, 0, entryComment.Length);   //�ļ�ע��                     ���������ȣ�
                ++numEntries;
                sizeEntries += ZipConstants.CENHDR + name.Length + extra.Length + entryComment.Length;
            }
            // ѹ��Դ�ļ�Ŀ¼������־
            WriteLeInt32(ZipConstants.ENDSIG);
            WriteLeInt16(0);                    // number of this disk
            WriteLeInt16(0);                    // no of disk with start of central dir
            WriteLeInt16((short)numEntries);           // entries in central dir for this disk
            WriteLeInt16((short)numEntries);           // total entries in central directory
            WriteLeInt32(sizeEntries);            // size of the central directory
            WriteLeInt32((int)this.offset);            // offset of start of central dir
            WriteLeInt16((short)this.zipComment.Length);
            this.baseStream.Write(this.zipComment, 0, this.zipComment.Length);
            this.baseStream.Flush();
        }
        #endregion

        #region public property
        /// <summary>
        /// ����Ĭ��ѹ����ʽ
        /// </summary>
        public byte Method
        {
			//get
			//{
			//    return this.method;
			//}
            set
            {
                this.method = value;
            }
        }

#if DELUXEWORKSTEST
		/// <summary>
		/// ������������������
		/// </summary>
		/// <remarks>
		/// ������������������(���ڲ��Դ�����ʹ��)
		/// </remarks>
		public Stream BaseStream
		{
			get
			{
				return this.baseStream;
			}
		}
#endif
        /// <summary>
        /// ʵ��IDisposable�ӿ�
        /// </summary>
        public void Dispose()
        {
            //Modify By yuanyong 20070426
            //baseStream����紫�룬��������˭����˭�رյ�ԭ��
            //if (baseStream != null)
            //    baseStream.Dispose();
        }
        #endregion
    }
}
