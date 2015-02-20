using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Builder
{
	internal class BytesDescriptionGenerator : DataDescriptionGeneratorBase
	{
		public static readonly BytesDescriptionGenerator Instance = new BytesDescriptionGenerator();

		protected override bool DecideIsMatched(SqlCaluseBuilderItemWithData builderItem)
		{
			return builderItem.Data is byte[];
		}

		protected override string GetDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder)
		{
			return BytesToHexString((byte[])builderItem.Data);
		}

		private static string BytesToHexString(byte[] data)
		{
			StringBuilder strB = new StringBuilder(4096);

			if (data.Length > 0)
			{
				for (int i = 0; i < data.Length; i++)
				{
					if (strB.Length == 0)
						strB.Append("0X");

					strB.AppendFormat("{0:X2}", data[i]);
				}
			}
			else
				strB.Append("NULL");

			return strB.ToString();
		}
	}
}
