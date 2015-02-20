using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Builder
{
	internal class EnumDescriptionGenerator : DataDescriptionGeneratorBase
	{
		public static readonly EnumDescriptionGenerator Instance = new EnumDescriptionGenerator();

		protected override bool DecideIsMatched(SqlCaluseBuilderItemWithData builderItem)
		{
			return builderItem.Data.GetType().IsEnum;
		}

		protected override string GetDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder)
		{
			return builder.CheckUnicodeQuotationMark(builderItem.Data.ToString());
		}
	}
}
