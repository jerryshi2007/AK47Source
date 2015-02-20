#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	GenericZip.cs
// Remark	：	提供对二进制流、字符串、物理文件的压缩功能。
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

namespace MCS.Library.Compression
{
	/// <summary>
	/// 压缩类
	/// </summary>
	/// <remarks>
	/// 提供对二进制流、字符串、物理文件的压缩功能。
	/// </remarks>
	public static class GenericZip
	{
		#region Member Variables
		private const int BufferSize = 8192;
		#endregion

		#region Private Method
		/// <summary>
		/// 预检
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		private static bool PreCheck(Stream stream)
		{
			if (stream == null) return false;

			return true;
		}

		private static bool PreCheck(string unZipsting)
		{
			if (unZipsting == null) return false;

			return true;
		}

		#endregion

		#region Public Method
		/// <summary>
		/// 压缩二进制流
		/// </summary>
		/// <param name="stream">待压缩的二进制流</param>
		/// <returns>返回压缩后的二进制流</returns>
		/// <remarks>
		/// 压缩二进制流：通过CreateTempFile()生成临时文件；生成待压缩的二进制流inStream；返回压缩后的二进制流outStream；
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StreamZipTest" lang="cs" title="压缩二进制流" />
		/// </remarks>
		public static Stream ZipStream(Stream stream)
		{
			if (false == PreCheck(stream))
				return null;

			#region compress stream by buffers

			MemoryStream result = new MemoryStream();

			Byte[] buff = new Byte[BufferSize];

			using (DeflateStream compressStream = new DeflateStream(result, CompressionMode.Compress, true))
			{
				int cnt = stream.Read(buff, 0, BufferSize);
				while (cnt > 0)
				{
					compressStream.Write(buff, 0, cnt);
					cnt = stream.Read(buff, 0, BufferSize);
				}
			}

			#endregion

			return result;
		}

		/// <summary>
		/// 解压缩二进制流
		/// </summary>
		/// <param name="stream">待解压缩的二进制流</param>
		/// <returns>返回解压缩后的二进制流</returns>
		/// <remarks>
		/// 解压缩二进制流：待解压缩的二进制流outStream；生成解压后的二进制流unzipStream；
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StreamUnzipTest" lang="cs" title="解压缩二进制流" />
		/// </remarks>
		public static Stream UnzipStream(Stream stream)
		{
			if (false == PreCheck(stream)) return null;

			stream.Position = 0;

			#region decompress stream by buffers
			MemoryStream result = new MemoryStream();

			Byte[] buff = new Byte[BufferSize];


			using (DeflateStream decompressStream = new DeflateStream(stream, CompressionMode.Decompress, true))
			{
				int cnt = decompressStream.Read(buff, 0, BufferSize);
				while (cnt > 0)
				{
					result.Write(buff, 0, cnt);
					cnt = decompressStream.Read(buff, 0, BufferSize);
				}
			}
			#endregion

			return result;
		}

		/// <summary>
		/// 压缩字符串
		/// </summary>
		/// <param name="uncompressedString">待压缩的字符串</param>
		/// <returns>返回压缩后的字符串</returns>
		/// <remarks>
		/// 压缩字符串：压缩后的字符串 = GenericZip.ZipStream(待压缩的字符串)
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StringZipTest" lang="cs" title="压缩字符串" />
		/// </remarks>
		public static string ZipString(string uncompressedString)
		{
			return ZipString(uncompressedString, Encoding.Default);
		}

		/// <summary>
		/// 压缩字符串
		/// </summary>
		/// <param name="unCompressedString">待压缩的字符串</param>
		/// <param name="defultEncode">字符串编码</param>
		/// <returns>返回压缩后的字符串</returns>
		/// <remarks>
		/// 压缩字符串：压缩后的字符串 = GenericZip.ZipStream(待压缩的字符串)
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StringZipTest1" lang="cs" title="压缩字符串" />
		/// </remarks>
		public static string ZipString(string unCompressedString, Encoding defultEncode)
		{
			string zipStr = null;

			if (false == PreCheck(unCompressedString))
				return null;

			byte[] bytData = defultEncode.GetBytes(unCompressedString);

		    MemoryStream ms = new MemoryStream();

            using (Stream s = new GZipStream(ms, CompressionMode.Compress))
            {
                s.Write(bytData, 0, bytData.Length);
            }

            byte[] compressedData = (byte[])ms.ToArray();

            zipStr = System.Convert.ToBase64String(compressedData, 0, compressedData.Length);

			return zipStr;
		}

		/// <summary>
		/// 解压缩字符串
		/// </summary>
		/// <param name="compressedString">待解压缩的字符串</param>
		/// <returns>返回解压缩后的字符串</returns>
		/// <remarks>
		/// 解压缩字符串：解压缩后的字符串 = GenericZip.UnzipStream(待解压缩的字符串);
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StringUnzipTest" lang="cs" title="解压缩字符串" />
		/// </remarks> 
		public static string UnzipString(string compressedString)
		{
			return UnzipString(compressedString, Encoding.Default);
		}

		/// <summary>
		/// 解压缩字符串
		/// </summary>
		/// <param name="compressedString">待解压缩的字符串</param>
		/// <param name="defultEncode">字符串编码</param>
		/// <returns>返回解压缩后的字符串</returns>
		/// <remarks>
		/// 解压缩字符串：解压缩后的字符串 = GenericZip.UnzipStream(待解压缩的字符串);
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StringUnzipTest1" lang="cs" title="解压缩字符串" />
		/// </remarks> 
		public static string UnzipString(string compressedString, Encoding defultEncode)
		{
			if (false == PreCheck(compressedString)) 
				return null;

			System.Text.StringBuilder result = new System.Text.StringBuilder();

			byte[] writeData = new byte[4096];

			byte[] bytData = System.Convert.FromBase64String(compressedString);

			int totalLength = 0;

			int size = 0;

			using (Stream s = new GZipStream(new MemoryStream(bytData), CompressionMode.Decompress))
			{
				while (true)
				{
					size = s.Read(writeData, 0, writeData.Length);

					if (size > 0)
					{
						totalLength += size;

						result.Append(defultEncode.GetString(writeData, 0, size));
					}
					else
					{
						break;
					}
				}
			}

			return result.ToString();
		}

		/// <summary>
		/// 压缩文件
		/// </summary>
		/// <param name="sourceFile">待压缩的文件的绝对路径，包括文件名</param>
		/// <param name="destinationFileName">生成的压缩文件的绝对路径，包括文件名</param>
		/// <remarks>
		/// 压缩文件：GenericZip.ZipFiles(待压缩的文件, 生成的压缩文件);
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="FileZipTest" lang="cs" title="压缩文件" />
		/// </remarks> 
		public static void ZipFiles(string[] sourceFile, string destinationFileName)
		{
			byte method = 4;

			Directory.CreateDirectory(Path.GetDirectoryName(destinationFileName));

			using (ZipFile zip = new ZipFile(destinationFileName, method, FileMode.Create))
			{
				foreach (string srcFileName in sourceFile)
				{
					if (srcFileName != null)
					{
						zip.Add(srcFileName);
					}
				}
			}
		}

		/// <summary>
		/// 解压缩文件
		/// </summary>
		/// <param name="sourceFile">待解压缩的文件的绝对路径，包括文件名</param>
		/// <param name="destPath">压缩文件中被释放文件的存储目录(绝对路径)</param>
		/// <remarks>
		/// 解压缩文件：GenericZip.UnzipFiles(待解压缩的文件, 被释放文件的存储目录);
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="FileUnzipTest" lang="cs" title="解压缩文件" />
		/// </remarks>
		public static void UnzipFiles(string sourceFile, string destPath)
		{
			Directory.CreateDirectory(destPath);

			using (ZipFile zip = new ZipFile(sourceFile))
			{
				zip.ExtractAll(destPath);
			}
		}
		#endregion
	}
}