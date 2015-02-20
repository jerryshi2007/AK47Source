#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	GenericZip.cs
// Remark	��	�ṩ�Զ����������ַ����������ļ���ѹ�����ܡ�
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

namespace MCS.Library.Compression
{
	/// <summary>
	/// ѹ����
	/// </summary>
	/// <remarks>
	/// �ṩ�Զ����������ַ����������ļ���ѹ�����ܡ�
	/// </remarks>
	public static class GenericZip
	{
		#region Member Variables
		private const int BufferSize = 8192;
		#endregion

		#region Private Method
		/// <summary>
		/// Ԥ��
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
		/// ѹ����������
		/// </summary>
		/// <param name="stream">��ѹ���Ķ�������</param>
		/// <returns>����ѹ����Ķ�������</returns>
		/// <remarks>
		/// ѹ������������ͨ��CreateTempFile()������ʱ�ļ������ɴ�ѹ���Ķ�������inStream������ѹ����Ķ�������outStream��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StreamZipTest" lang="cs" title="ѹ����������" />
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
		/// ��ѹ����������
		/// </summary>
		/// <param name="stream">����ѹ���Ķ�������</param>
		/// <returns>���ؽ�ѹ����Ķ�������</returns>
		/// <remarks>
		/// ��ѹ����������������ѹ���Ķ�������outStream�����ɽ�ѹ��Ķ�������unzipStream��
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StreamUnzipTest" lang="cs" title="��ѹ����������" />
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
		/// ѹ���ַ���
		/// </summary>
		/// <param name="uncompressedString">��ѹ�����ַ���</param>
		/// <returns>����ѹ������ַ���</returns>
		/// <remarks>
		/// ѹ���ַ�����ѹ������ַ��� = GenericZip.ZipStream(��ѹ�����ַ���)
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StringZipTest" lang="cs" title="ѹ���ַ���" />
		/// </remarks>
		public static string ZipString(string uncompressedString)
		{
			return ZipString(uncompressedString, Encoding.Default);
		}

		/// <summary>
		/// ѹ���ַ���
		/// </summary>
		/// <param name="unCompressedString">��ѹ�����ַ���</param>
		/// <param name="defultEncode">�ַ�������</param>
		/// <returns>����ѹ������ַ���</returns>
		/// <remarks>
		/// ѹ���ַ�����ѹ������ַ��� = GenericZip.ZipStream(��ѹ�����ַ���)
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StringZipTest1" lang="cs" title="ѹ���ַ���" />
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
		/// ��ѹ���ַ���
		/// </summary>
		/// <param name="compressedString">����ѹ�����ַ���</param>
		/// <returns>���ؽ�ѹ������ַ���</returns>
		/// <remarks>
		/// ��ѹ���ַ�������ѹ������ַ��� = GenericZip.UnzipStream(����ѹ�����ַ���);
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StringUnzipTest" lang="cs" title="��ѹ���ַ���" />
		/// </remarks> 
		public static string UnzipString(string compressedString)
		{
			return UnzipString(compressedString, Encoding.Default);
		}

		/// <summary>
		/// ��ѹ���ַ���
		/// </summary>
		/// <param name="compressedString">����ѹ�����ַ���</param>
		/// <param name="defultEncode">�ַ�������</param>
		/// <returns>���ؽ�ѹ������ַ���</returns>
		/// <remarks>
		/// ��ѹ���ַ�������ѹ������ַ��� = GenericZip.UnzipStream(����ѹ�����ַ���);
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="StringUnzipTest1" lang="cs" title="��ѹ���ַ���" />
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
		/// ѹ���ļ�
		/// </summary>
		/// <param name="sourceFile">��ѹ�����ļ��ľ���·���������ļ���</param>
		/// <param name="destinationFileName">���ɵ�ѹ���ļ��ľ���·���������ļ���</param>
		/// <remarks>
		/// ѹ���ļ���GenericZip.ZipFiles(��ѹ�����ļ�, ���ɵ�ѹ���ļ�);
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="FileZipTest" lang="cs" title="ѹ���ļ�" />
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
		/// ��ѹ���ļ�
		/// </summary>
		/// <param name="sourceFile">����ѹ�����ļ��ľ���·���������ļ���</param>
		/// <param name="destPath">ѹ���ļ��б��ͷ��ļ��Ĵ洢Ŀ¼(����·��)</param>
		/// <remarks>
		/// ��ѹ���ļ���GenericZip.UnzipFiles(����ѹ�����ļ�, ���ͷ��ļ��Ĵ洢Ŀ¼);
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Compression\GenericZipTest.cs" region="FileUnzipTest" lang="cs" title="��ѹ���ļ�" />
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