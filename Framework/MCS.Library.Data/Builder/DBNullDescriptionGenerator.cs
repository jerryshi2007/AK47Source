using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Builder
{
	internal class DBNullDescriptionGenerator : DataDescriptionGeneratorBase
	{
		public static readonly DBNullDescriptionGenerator Instance = new DBNullDescriptionGenerator();

		protected override bool DecideIsMatched(SqlCaluseBuilderItemWithData builderItem)
		{
			return builderItem.Data is DBNull;
		}

		protected override string GetDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder)
		{
			return "NULL";
		}
	}
}
