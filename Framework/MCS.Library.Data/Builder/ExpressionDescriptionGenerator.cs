using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Builder
{
	internal class ExpressionDescriptionGenerator : DataDescriptionGeneratorBase
	{
		public static readonly ExpressionDescriptionGenerator Instance = new ExpressionDescriptionGenerator();

		protected override bool DecideIsMatched(SqlCaluseBuilderItemWithData builderItem)
		{
			return builderItem.IsExpression;
		}

		protected override string GetDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder)
		{
			return builderItem.Data.ToString();
		}
	}
}
