using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Builder
{
	internal class BooleanDescriptionGenerator : DataDescriptionGeneratorBase
	{
		public static readonly BooleanDescriptionGenerator Instance = new BooleanDescriptionGenerator();

		protected override bool DecideIsMatched(SqlCaluseBuilderItemWithData builderItem)
		{
			return builderItem.Data is Boolean;
		}

		protected override string GetDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder)
		{
			return ((int)Convert.ChangeType(builderItem.Data, typeof(int))).ToString();
		}
	}
}
