using System;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zip;
using Ionic.Zlib;

namespace MCS.Library.Compression
{
	public partial class CompressManager
	{
		#region Extract
		/// <summary>
		/// extract a archive file to directory
		/// </summary>
		/// <param name="archiveFile"></param>
		/// <param name="targetDirectory"></param>
		/// <param name="overwrite"></param>
		public static void Extract(string archiveFile, string targetDirectory, ExtractExistingFileAction overwrite)
		{
			using (var zip = ZipFile.Read(archiveFile))
			{
				foreach (var entry in zip)
				{
					entry.Extract(targetDirectory, (Ionic.Zip.ExtractExistingFileAction)((int)overwrite));
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="archiveFile"></param>
		/// <param name="targetDirectory"></param>
		/// <param name="overwrite"></param>
		/// <param name="password"></param>
		public static void Extract(string archiveFile, string targetDirectory, ExtractExistingFileAction overwrite, string password)
		{
			using (var zip = ZipFile.Read(archiveFile))
			{
				foreach (var entry in zip)
				{
					entry.ExtractWithPassword(targetDirectory, (Ionic.Zip.ExtractExistingFileAction)((int)overwrite), password);
				}
			}
		}

		/// <summary>
		/// extract a stream
		/// </summary>
		/// <param name="decompressed"></param>
		/// <returns></returns>
		public static Stream ExtractStream(Stream decompressed)
		{
			decompressed.Seek(0, SeekOrigin.Begin);
			var result = new MemoryStream();

			using (var zlibStream = new ZlibStream(result, CompressionMode.Decompress, true))
			{
				CopyStream(decompressed, zlibStream);
				return result;
			}
		}

		/// <summary>
		/// 将字节数组解压缩到字符数组
		/// </summary>
		/// <param name="byteArray"></param>
		/// <returns></returns>
		public static byte[] ExtractBytes(byte[] byteArray)
		{
			if (byteArray == null)
				throw new ArgumentNullException("byteArray");

			using (MemoryStream decompressed = new MemoryStream(byteArray))
			{
				using (Stream stream = ExtractStream(decompressed))
				{
					return ((MemoryStream)stream).ToArray();
				}
			}
		}

		/// <summary>
		/// 从字符数组中解压缩出字符串，默认的编码方式是Encoding.Default
		/// </summary>
		/// <param name="byteArray"></param>
		/// <returns></returns>
		public static string ExtractString(byte[] byteArray)
		{
			return ExtractString(byteArray, Encoding.Default);
		}

		/// <summary>
		/// 从字符数组中解压缩出字符串
		/// </summary>
		/// <param name="byteArray"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string ExtractString(byte[] byteArray, Encoding encoding)
		{
			using (MemoryStream decompressed = new MemoryStream(byteArray))
			{
				using (Stream stream = ExtractStream(decompressed))
				{
					return MemoryStreamToString(stream as MemoryStream, encoding);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="existingZipFile"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static Stream GetStreamFromArchive(string existingZipFile, string filename)
		{
			using (var zip = ZipFile.Read(existingZipFile))
			{
				var result = new MemoryStream();
				var entryQuery = zip.Entries.Where(o => filename.Equals(Path.GetFileName(o.FileName)));

				if (!entryQuery.Any())
					return Stream.Null;

				var zipEntry = entryQuery.FirstOrDefault();

				if (zipEntry == null)
					return Stream.Null;

				zipEntry.Extract(result);

				return result;
			}
		}

		public static Stream GetStreamFromArchive(string existingZipFile, string filename, string password)
		{
			using (var zip = ZipFile.Read(existingZipFile))
			{
				var result = new MemoryStream();
				var entryQuery = zip.Entries.Where(o => filename.Equals(Path.GetFileName(o.FileName)));

				if (!entryQuery.Any())
					return Stream.Null;

				var zipEntry = entryQuery.FirstOrDefault();

				if (zipEntry == null)
					return Stream.Null;

				zipEntry.ExtractWithPassword(result, password);

				return result;
			}
		}

		#endregion
	}
}
