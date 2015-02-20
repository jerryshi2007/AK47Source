using System.IO;
using System.Text;
using Ionic.Zlib;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace MCS.SqlServer.Compression
{
	public partial class CompressManager
	{
		#region Compress

		[SqlFunction]
		public static SqlBinary CompressString(SqlString content)
		{
			using (var ms = StringToMemoryStream((string)content, Encoding.UTF8))
			{
				using (var compressed = CompressStream(ms))
				{
					return new SqlBinary((compressed as MemoryStream).ToArray());
				}
			}
		}

		[SqlFunction]
		public static SqlBinary CompressStringWithEncoding(SqlString content, string encodingName)
		{
			if (encodingName == string.Empty || encodingName == null)
				encodingName = "utf-8";

			using (var ms = StringToMemoryStream((string)content, Encoding.GetEncoding(encodingName)))
			{
				using (var compressed = CompressStream(ms))
				{
					return new SqlBinary((compressed as MemoryStream).ToArray());
				}
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
		#endregion
	}
}
