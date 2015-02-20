#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	ZipReader.cs
// Remark	：	读取文件流
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    梁东	    20070430		创建
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
    /// 通过读取文件流获取一个压缩文件实体对象，其中包含文件名，原文件大小，
    /// 压缩后大小，CRC，etc等信息。
    /// </summary>
#if DELUXEWORKSTEST
    public class ZipReader : IDisposable
#else
    internal class ZipReader : IDisposable
#endif
    {
        #region Private field
        //private Stream zipStream = null;		//The zip stream which is instantiated
        //and closed after every operation

		//private string zipName;			//Name given to the archive

        private Stream baseStream;		//The base stream from which the header 
        //and compressed data are read

        //private Int16 numberOfFiles;
        private byte method;

		//private string comment;

        private long offsetOfFirstEntry = 0;

        //private List<ZipEntry> entries;

        private Crc32 crc = new Crc32();
        #endregion

        #region Constructor
        /// <summary>
        /// 创建一个Zip的输入流，读取压缩文件。
        /// </summary>
        public ZipReader(Stream fileStream)//, string name)
        {
			//this.zipName = name;
            this.baseStream = fileStream;
            //numberOfFiles = -1;

        }
        #endregion

        #region Private method

		///// <summary>
		///// read buffer from stream
		///// </summary>
		///// <param name="outBuf"></param>
		///// <param name="length"></param>
		///// <returns></returns>
		//private int ReadBuffer(byte[] outBuf, int length)
		//{
		//    return this.baseStream.Read(outBuf, 0, length);
		//}

        /// <summary>
        /// Read a byte from baseStream.
        /// </summary>
        private byte ReadLeByte()
        {
            return (byte)this.baseStream.ReadByte();
        }

        /// <summary>
        /// Read an unsigned short baseStream little endian byte order.
        /// </summary>
        private Int16 ReadLeInt16()
        {
            return (Int16)(ReadLeByte() | (ReadLeByte() << 8));
        }

        /// <summary>
        /// Read an int baseStream little endian byte order.
        /// </summary>
        private Int32 ReadLeInt32()
        {
            return (UInt16)ReadLeInt16() | ((UInt16)ReadLeInt16() << 16);
        }

		///// <summary>
		///// convert string to byte[]
		///// </summary>
		///// <param name="data"></param>
		///// <returns></returns>
		//private string ConvertToString(byte[] data)
		//{
		//    return System.Text.Encoding.ASCII.GetString(data, 0, data.Length);
		//}

        // NOTE this returns the offset of the first byte after the signature.
        private long LocateBlockWithSignature(int signature, long endLocation, int minimumBlockSize, int maximumVariableData)
        {
            long pos = endLocation - minimumBlockSize;
            if (pos < 0)
            {
                return -1;
            }

            long giveUpMarker = Math.Max(pos - maximumVariableData, 0);

            // TODO: this loop could be optimised for speed.
            do
            {
                if (pos < giveUpMarker)
                {
                    return -1;
                }
                this.baseStream.Seek(pos--, SeekOrigin.Begin);
            } while (ReadLeInt32() != signature);

            return this.baseStream.Position;
        }

		///// <summary>
		///// Open the next entry from the zip archive, and return its 
		///// description. The method expects the pointer to be intact.
		///// </summary>
		//private ZipEntry GetNextEntry()
		//{
		//    ZipEntry currentEntry = null;

		//    Int32 size = ReadLeInt32();
		//    if (size == -1)
		//        return new ZipEntry(String.Empty);

		//    Int32 csize = ReadLeInt32();
		//    byte[] crc = new byte[16];
		//    ReadBuffer(crc, crc.Length);

		//    Int32 dostime = ReadLeInt32();
		//    Int16 nameLength = ReadLeInt16();

		//    byte[] buffer = new byte[nameLength];
		//    ReadBuffer(buffer, nameLength);
		//    string name = ConvertToString(buffer);

		//    currentEntry = new ZipEntry(name);
		//    currentEntry.Size = size;
		//    currentEntry.CompressedSize = csize;
		//    //currentEntry.Crc = crc;
		//    currentEntry.DosTime = dostime;

		//    return currentEntry;
		//}

        /// <summary>
        /// Writes the uncompressed data into the filename in the 
        /// entry. It instantiates a memory stream which will serve 
        /// as a temp store and decompresses it using Gzip Stream or
        /// Deflate stream
        /// </summary>
        private void WriteDecompressedFile(ZipEntry entry, string fullPath)
        {
            this.baseStream.Seek(this.offsetOfFirstEntry + entry.Offset, SeekOrigin.Begin);

            ReadLeInt32();               //文件头标记                  4 bytes  (0x04034b50)
            ReadLeInt16();    //解压文件所需 pkware 版本    2 bytes
            ReadLeInt16();    // 全局方式位标记              2 bytes
            ReadLeInt16();    //压缩方式                    2 bytes
            ReadLeInt32();               //最后修改文件时间             2 bytes+最后修改文件日期             2 bytes
            uint crc2 = (uint)ReadLeInt32();//intValue = ReadLeInt32();               //CRC-32校验                  4 bytes
            ReadLeInt32();               //压缩后尺寸                  4 bytes
            ReadLeInt32();               //未压缩尺寸                  4 bytes
            int storedNameLength = ReadLeInt16();   //文件名长度                  2 bytes
            int extraLen = storedNameLength + ReadLeInt16();
            int start = (int)this.offsetOfFirstEntry + entry.Offset + ZipConstants.LOCHDR + extraLen;

            this.baseStream.Position = (long)start;
            byte[] b = new byte[entry.CompressedSize];
            this.baseStream.Read(b, 0, entry.CompressedSize);

            //if (CheckCRC([byte]entry.Crc, b))
            this.crc.Update(b);

            ExceptionHelper.TrueThrow(this.crc.Value != crc2, CompressionRes.ResourceManager.GetString(CompressionError.InvalidCRC.ToString()));

            this.crc.Reset();

            MemoryStream ms = new MemoryStream();

            ms.Write(b, 0, b.Length);
            ms.Seek(0, SeekOrigin.Begin);

            ExceptionHelper.FalseThrow(this.method == ZipConstants.DEFLATE || this.method == ZipConstants.GZIP, CompressionRes.ResourceManager.GetString(CompressionError.InvalidMethod.ToString()));

            Stream zipStream = null;
            if (this.method == ZipConstants.DEFLATE)
                zipStream = new DeflateStream(ms, CompressionMode.Decompress, false);
            else// if (method == ZipConstants.GZIP)
                zipStream = new GZipStream(ms, CompressionMode.Decompress, false);

            using (zipStream as IDisposable)
            {
                //int index = entry.Name.LastIndexOf(ZipConstants.BackSlash);
                string name = fullPath + entry.Name;//.Substring(index + 1);

                using (FileStream rewrite = new FileStream(name, FileMode.Create))
                {
                    b = new byte[entry.Size];

                    zipStream.Read(b, 0, (int)entry.Size);
                    rewrite.Write(b, 0, (int)entry.Size);
                }
            }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// 解压缩zipEntries中的文件，释放文件到目录destPath
        /// </summary>
        /// <param name="zipEntries">被压缩文件的文件信息</param>
        /// <param name="destPath">文件释放的存储目录</param>
        public void ExtractAll(List<ZipEntry> zipEntries, string destPath)
        {
            foreach (ZipEntry entry in zipEntries)
            {
                WriteDecompressedFile(entry, destPath);
            }

        }

        /// <summary>
        /// 获得压缩文件中所有被压缩文件信息
        /// </summary>
        /// <returns>被压缩文件的文件信息</returns>
        /// <remarks>
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\ZipReaderTest.cs" region="GetAllEntriesTest" lang="cs" title="获得压缩文件中所有被压缩文件信息" />  
        /// </remarks>
        public List<ZipEntry> GetAllEntries()
        {
            List<ZipEntry> entries = new List<ZipEntry>();

            //if (baseStream.CanSeek == false)
            //{
            //    return entries;//throw new ZipException("ZipFile stream must be seekable");

            //}
            ExceptionHelper.FalseThrow(this.baseStream.CanSeek, CompressionRes.ResourceManager.GetString(CompressionError.InvalidStream.ToString()));

            long locatedCentralDirOffset = LocateBlockWithSignature(ZipConstants.ENDSIG, this.baseStream.Length, ZipConstants.ENDHDR, 0xffff);

            //if (locatedCentralDirOffset < 0)
            //{
            //    return entries;// throw new ZipException("Cannot find central directory");
            //}
            ExceptionHelper.TrueThrow(locatedCentralDirOffset < 0, CompressionRes.ResourceManager.GetString(CompressionError.InvalidFileEnding.ToString()));

            //int thisDiskNumber = ReadLeInt16();
			ReadLeInt16();
            //int startCentralDirDisk = ReadLeInt16();
			ReadLeInt16();
            int entriesForThisDisk = ReadLeInt16();
            //int entriesForWholeCentralDir = ReadLeInt16();
			ReadLeInt16();
            int centralDirSize = ReadLeInt32();
            int offsetOfCentralDir = ReadLeInt32();
            int commentSize = ReadLeInt16();

            byte[] zipComment = new byte[commentSize];
            this.baseStream.Read(zipComment, 0, zipComment.Length);

			//this.comment = ZipConstants.ConvertToString(zipComment);

            // SFX support, find the offset of the first entry vis the start of the stream
            // This applies to Zip files that are appended to the end of the SFX stub.
            // Zip files created by some archivers have the offsets altered to reflect the true offsets
            // and so dont require any adjustment here...
            if (offsetOfCentralDir < locatedCentralDirOffset - (4 + centralDirSize))
            {
                this.offsetOfFirstEntry = locatedCentralDirOffset - (4 + centralDirSize + offsetOfCentralDir);

                //if (offsetOfFirstEntry <= 0)
                //{
                //    //throw new ZipException("Invalid SFX file");
                //}
                ExceptionHelper.TrueThrow(this.offsetOfFirstEntry <= 0, CompressionRes.ResourceManager.GetString(CompressionError.InvalidSFXFile.ToString()));
            }

            this.baseStream.Seek(this.offsetOfFirstEntry + offsetOfCentralDir, SeekOrigin.Begin);

            for (int i = 0; i < entriesForThisDisk; i++)
            {
                //if (ReadLeInt32() != ZipConstants.CENSIG)
                //{
                //    //throw new ZipException("Wrong Central Directory signature");
                //}
                ExceptionHelper.TrueThrow(ReadLeInt32() != ZipConstants.CENSIG, CompressionRes.ResourceManager.GetString(CompressionError.InvalidFileDir.ToString()));

                //int versionMadeBy = ReadLeInt16();
				ReadLeInt16();
                //int versionToExtract = ReadLeInt16();
				ReadLeInt16();
				//int bitFlags = ReadLeInt16();
				ReadLeInt16();
                int method1 = ReadLeInt16();
                int dostime = ReadLeInt32();
                int crc32 = ReadLeInt32();
                int csize = ReadLeInt32();
                int size = ReadLeInt32();
                int nameLen = ReadLeInt16();
                int extraLen = ReadLeInt16();
                int commentLen = ReadLeInt16();
				//int diskStartNo = ReadLeInt16();  // Not currently used
				ReadLeInt16();
                //int internalAttributes = ReadLeInt16();  // Not currently used
				ReadLeInt16();
                //int externalAttributes = ReadLeInt32();
				ReadLeInt32();
                int offset = ReadLeInt32();

                byte[] buffer = new byte[Math.Max(nameLen, commentLen)];
                this.baseStream.Read(buffer, 0, nameLen);

                string name = ZipConstants.ConvertToString(buffer, nameLen);
                ZipEntry entry = new ZipEntry(name);//ZipEntry entry = new ZipEntry(name, versionToExtract, versionMadeBy);

                this.method = (byte)method1;//entry.CompressionMethod = (CompressionMethod)method;                

                entry.Crc = crc32;// &0xffffffffL;
                entry.Size = size;// &0xffffffffL;
                entry.CompressedSize = csize;// &0xffffffffL;
				//entry.Flags = bitFlags;
                entry.DosTime = dostime;//entry.DosTime = (uint)dostime;

                if (extraLen > 0)
                {
                    byte[] extra = new byte[extraLen];
                    this.baseStream.Read(extra, 0, extraLen);
                    entry.ExtraData = extra;
                }

                if (commentLen > 0)
                {
                    this.baseStream.Read(buffer, 0, commentLen);
                    //entry.Comment = ZipConstants.ConvertToString(buffer, commentLen);
                }

				//entry.ZipFileIndex = i;
                entry.Offset = offset;
				//entry.ExternalFileAttributes = externalAttributes;
                entries.Add(entry);
            }

            return entries;
        }

        /// <summary>
        /// 实现IDisposable接口
        /// </summary>
        public void Dispose()
        {
            //Modify By yuanyong 20070426
            //baseStream有外界传入，这里遵守谁创建谁关闭的原则。
            //if (baseStream != null)
            //    baseStream.Dispose();
        }
        #endregion

        #region Property
		///// <summary>
		///// Gets the method of compression of the archive.
		///// </summary>
		///// <returns>
		///// the ZipConstants.Deflate or ZipConstants.Gzip
		///// </returns>
		//public byte Method
		//{
		//    get
		//    {
		//        return this.method;
		//    }
		//}
#if DELUXEWORKSTEST
		/// <summary>
		/// 文件流
		/// </summary>
		/// <remarks>文件流(仅在测试代码中使用)</remarks>
		public Stream BaseStream
		{
			get
			{
				return this.baseStream;
			}
		}

		///// <summary>
		///// 待解压的文件名称
		///// </summary>
		//public string ZipName
		//{
		//    get
		//    {
		//        return this.zipName;
		//    }
		//}
#endif
        #endregion
    }
}
