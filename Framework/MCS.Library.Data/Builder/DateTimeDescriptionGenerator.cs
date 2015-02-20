using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data.Builder
{
	internal class DateTimeDescriptionGenerator : DataDescriptionGeneratorBase
	{
		public static readonly DateTimeDescriptionGenerator Instance = new DateTimeDescriptionGenerator();

		protected override bool DecideIsMatched(SqlCaluseBuilderItemWithData builderItem)
		{
			return builderItem.Data is DateTime;
		}

		protected override string GetDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder)
		{
			string result = string.Empty;

			DateTime minDate = new DateTime(1753, 1, 1);

			if ((DateTime)builderItem.Data < minDate)
				result = "NULL";
			else
				result = builder.FormatDateTime((DateTime)builderItem.Data);

			return result;
		}
	}
}
