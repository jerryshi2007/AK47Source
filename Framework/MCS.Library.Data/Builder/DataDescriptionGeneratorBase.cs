using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Data.Builder
{
	/// <summary>
	/// 数据转换成Sql描述的生成器的基类
	/// </summary>
	internal abstract class DataDescriptionGeneratorBase
	{
		public string ToDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder)
		{
			builderItem.NullCheck("builderItem");
			builder.NullCheck("builder");

			return GetDescription(builderItem, builder);
		}

		/// <summary>
		/// 是否匹配。确定是否由自己这个类处理
		/// </summary>
		/// <param name="builderItem"></param>
		/// <returns></returns>
		public bool IsMatched(SqlCaluseBuilderItemWithData builderItem)
		{
			builderItem.NullCheck("builderItem");

			return DecideIsMatched(builderItem);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="builderItem"></param>
		/// <returns></returns>
		protected abstract bool DecideIsMatched(SqlCaluseBuilderItemWithData builderItem);

		/// <summary>
		/// 需要重载
		/// </summary>
		/// <param name="builderItem"></param>
		/// <param name="builder"></param>
		/// <returns></returns>
		protected abstract string GetDescription(SqlCaluseBuilderItemWithData builderItem, ISqlBuilder builder);
	}
}
