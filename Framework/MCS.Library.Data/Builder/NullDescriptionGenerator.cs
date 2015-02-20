using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Builder
{
	internal class NullDescriptionGenerator : DataDescriptionGeneratorBase
	{
		public static readonly NullDescriptionGenerator Instance = new NullDescriptionGenerator();

		protected override bool DecideIsMatched(SqlCaluseBuilderItemWithData builderItem)
		{
			return builderItem.Data == null;
		}

		protected override string GetDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder)
		{
			return "NULL";
		}
	}
}
