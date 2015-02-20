using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Builder
{
	internal class StreamDescriptionGenerator : DataDescriptionGeneratorBase
	{
		public static readonly StreamDescriptionGenerator Instance = new StreamDescriptionGenerator();

		protected override bool DecideIsMatched(SqlCaluseBuilderItemWithData builderItem)
		{
			return builderItem.Data is Stream;
		}

		protected override string GetDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder)
		{
			return StreamToHexString((Stream)builderItem.Data);
		}

		private static string StreamToHexString(Stream stream)
		{
			byte[] buffer = new byte[4096];

			StringBuilder strB = new StringBuilder(4096);

			using (BinaryReader br = new BinaryReader(stream))
			{
				int byteRead = br.Read(buffer, 0, buffer.Length);

				while (byteRead > 0)
				{
					for (int i = 0; i < byteRead; i++)
					{
						if (strB.Length == 0)
							strB.Append("0X");

						strB.AppendFormat("{0:X2}", buffer[i]);
					}

					byteRead = br.Read(buffer, 0, buffer.Length);
				}
			}

			if (strB.Length == 0)
				strB.Append("NULL");

			return strB.ToString();
		}
	}
}
