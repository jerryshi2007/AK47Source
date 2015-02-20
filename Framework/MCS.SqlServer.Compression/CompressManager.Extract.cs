using System.Data.SqlTypes;
using System.IO;
using System.Text;
using Ionic.Zlib;
using Microsoft.SqlServer.Server;

namespace MCS.SqlServer.Compression
{
	public partial class CompressManager
	{
		#region Extract
		/// <summary>
		/// extract a stream
		/// </summary>
		/// <param name="decompressed"></param>
		/// <returns></returns>
		public static Stream ExtractStream(Stream decompressed)
		{
			decompressed.Seek(0, SeekOrigin.Begin);
			var result = new MemoryStream();
			using (
				var zlibStream = new ZlibStream(result, CompressionMode.Decompress, true)
				)
			{
				CopyStream(decompressed, zlibStream);
				return result;
			}
		}

		[SqlFunction]
		public static SqlString ExtractString(SqlBinary binary)
		{
			using (var decompressed = new MemoryStream(binary.Value))
			{
				using (var stream = ExtractStream(decompressed))
				{
					return MemoryStreamToString(stream as MemoryStream, Encoding.UTF8);
				}
			}
		}

		[SqlFunction]
		public static SqlString ExtractStringWithEncoding(SqlBinary binary, string encodingName)
		{
			using (var decompressed = new MemoryStream(binary.Value))
			{
				using (var stream = ExtractStream(decompressed))
				{
					if (encodingName == string.Empty || encodingName == null)
						encodingName = "utf-8";

					return MemoryStreamToString(stream as MemoryStream, Encoding.GetEncoding(encodingName));
				}
			}
		}
		#endregion
	}
}
