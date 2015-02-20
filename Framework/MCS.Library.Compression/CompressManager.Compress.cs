﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zip;
using Ionic.Zlib;

namespace MCS.Library.Compression
{
	public partial class CompressManager
	{
		#region Compress
		/// <summary>
		/// compress any files
		/// </summary>
		/// <param name="archive"></param>
		/// <param name="fileInfos"></param>
		public static void CompressFile(ZipFileInfo archive, List<ZipFileInfo> fileInfos, string directoryPathInArchive)
		{
			using (var zip = new ZipFile(Encoding.UTF8))
			{
				SetZipFile(zip, archive);
				foreach (var file in fileInfos)
				{
					SetZipFile(zip, file);
					zip.AddFile(file.Filename, directoryPathInArchive);
				}
				zip.Save(archive.Filename);
			}
		}

		private static void SetZipFile(ZipFile zip, ZipFileInfo file)
		{
			if (!string.IsNullOrEmpty(file.Password))
				zip.Password = file.Password;

			zip.Comment = file.Comment;
			zip.CompressionMethod = (Ionic.Zip.CompressionMethod)((int)file.CompressionMethod);
			zip.CompressionLevel = (Ionic.Zlib.CompressionLevel)((int)file.CompressionLevel);
		}

		private static void SetZipFile(ZipEntry zipEntry, ZipFileInfo file)
		{
			if (!string.IsNullOrEmpty(file.Password))
				zipEntry.Password = file.Password;

			zipEntry.Comment = file.Comment;
			zipEntry.CompressionMethod = (Ionic.Zip.CompressionMethod)((int)file.CompressionMethod);
			zipEntry.CompressionLevel = (Ionic.Zlib.CompressionLevel)((int)file.CompressionLevel);
		}

		/// <summary>
		/// compress a directory
		/// </summary>
		/// <param name="archive"></param>
		/// <param name="directoryName"></param>
		public static void CompressDirectory(ZipFileInfo archive, string directoryName)
		{
			using (var zip = new ZipFile(Encoding.UTF8))
			{
				SetZipFile(zip, archive);
				zip.AddDirectory(directoryName);
				zip.Save(archive.Filename);
			}
		}

		/// <summary>
		/// compress a stream<see cref="Stream"/>
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static Stream CompressStream(Stream stream)
		{
			var result = new MemoryStream();
			using (var zlibStream = new ZlibStream(result, CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestSpeed, true))
			{
				CopyStream(stream, zlibStream);
			}

			return result;
		}

		/// <summary>
		/// 压缩字符串到字节数组，默认的编码方式是Encoding.Default
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public static byte[] CompressString(string content)
		{
			return CompressString(content, Encoding.Default);
		}

		/// <summary>
		/// 压缩字符串到字节数组
		/// </summary>
		/// <param name="content"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static byte[] CompressString(string content, Encoding encoding)
		{
			using (var ms = StringToMemoryStream(content, encoding))
			{
				using (var compressed = CompressStream(ms))
				{
					return (compressed as MemoryStream).ToArray();
				}
			}
		}

		/// <summary>
		/// 压缩字符串到字节流，默认的编码方式是Encoding.Default
		/// </summary>
		/// <param name="originalText"></param>
		/// <returns></returns>
		public static Stream CompressStream(string originalText)
		{
			return CompressStream(originalText, Encoding.Default);
		}

		/// <summary>
		/// 压缩字符串到字节流
		/// </summary>
		/// <param name="originalText"></param>
		/// <param name="encoding">编码方式</param>
		/// <returns></returns>
		public static Stream CompressStream(string originalText, Encoding encoding)
		{
			var result = new MemoryStream();

			using (var zlibStream = new ZlibStream(result, CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestSpeed, true))
			{
				CopyStream(StringToMemoryStream(originalText, encoding), zlibStream);
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="archive"></param>
		/// <param name="stream"></param>
		/// <param name="streamFile"></param>
		public static void AddStreamToArchive(ZipFileInfo archive, ZipFileInfo streamFile, Stream stream)
		{
			using (var zip = new ZipFile(Encoding.UTF8))
			{
				SetZipFile(zip, archive);
				ZipEntry entry = zip.AddEntry(streamFile.Filename, stream);

				SetZipFile(entry, streamFile);
				zip.Save(archive.Filename);
			}
		}

		/// <summary>
		/// compress a file to archive file
		/// </summary>
		/// <param name="archive"></param>
		/// <param name="directoryName"></param>
		/// <param name="directoryPathInArchive"></param>
		public static void CompressAndSpecifyPathArchive(ZipFileInfo archive, string directoryName, string directoryPathInArchive)
		{
			using (var zip = new ZipFile(Encoding.UTF8))
			{
				SetZipFile(zip, archive);
				ZipEntry entry = zip.AddDirectory(directoryName, directoryPathInArchive);

				SetZipFile(entry, archive);
				zip.Save(archive.Filename);
			}
		}
		#endregion
	}
}
