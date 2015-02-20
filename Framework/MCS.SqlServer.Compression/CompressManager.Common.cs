using System;
using System.IO;
using System.Text;

namespace MCS.SqlServer.Compression
{
	public partial class CompressManager
	{
		#region Common

		/// <summary>
		/// Converts a string to a MemoryStream.
		/// </summary>
		public static MemoryStream StringToMemoryStream(string s, Encoding encoding)
		{
			var bytes = encoding.GetBytes(s);

			return new MemoryStream(bytes);
		}

		/// <summary>
		/// converts a MemorySteam to a string.
		/// </summary>
		/// <param name="ms"></param>
		/// <returns></returns>
		public static String MemoryStreamToString(MemoryStream ms, Encoding encoding)
		{
			var byteArray = ms.ToArray();

			return encoding.GetString(byteArray);
		}

		/// <summary>
		/// copy stream to dest stream
		/// </summary>
		/// <param name="src"></param>
		/// <param name="dest"></param>
		public static void CopyStream(Stream src, Stream dest)
		{
			var buffer = new byte[1024];
			var len = src.Read(buffer, 0, buffer.Length);

			while (len > 0)
			{
				dest.Write(buffer, 0, len);
				len = src.Read(buffer, 0, buffer.Length);
			}

			dest.Flush();
		}
		#endregion
	}
}
