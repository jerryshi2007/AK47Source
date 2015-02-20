using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Builder
{
	internal class GuidDescriptionGenerator : DataDescriptionGeneratorBase
	{
		public static readonly GuidDescriptionGenerator Instance = new GuidDescriptionGenerator();

		protected override bool DecideIsMatched(SqlCaluseBuilderItemWithData builderItem)
		{
			return builderItem.Data is Guid;
		}

		protected override string GetDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder)
		{
			string result = string.Empty;

			if ((Guid)builderItem.Data == Guid.Empty)
				result = "NULL";
			else
				result = builder.CheckUnicodeQuotationMark(builderItem.Data.ToString());

			return result;
		}
	}
}
