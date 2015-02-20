using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Builder
{
	internal class StringDescriptionGenerator : DataDescriptionGeneratorBase
	{
		public static readonly StringDescriptionGenerator Instance = new StringDescriptionGenerator();

		protected override bool DecideIsMatched(SqlCaluseBuilderItemWithData builderItem)
		{
			return builderItem.Data is string;
		}

		protected override string GetDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder)
		{
			return builder.CheckUnicodeQuotationMark(builderItem.Data.ToString());
		}
	}
}
