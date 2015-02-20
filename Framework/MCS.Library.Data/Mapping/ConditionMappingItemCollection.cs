using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;

using MCS.Library.Core;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.Data.Mapping
{
	/// <summary>
	/// 条件表达式和对象映射条目集合
	/// </summary>
	public class ConditionMappingItemCollection : DataObjectCollectionBase<ConditionMappingItem>
	{
		/// <summary>
		/// 添加一个条件项
		/// </summary>
		/// <param name="item"></param>
		public void Add(ConditionMappingItem item)
		{
			InnerAdd(item);
		}

		/// <summary>
		/// 按照索引添加或设置一个条件项
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ConditionMappingItem this[int index]
		{
			get
			{
				return (ConditionMappingItem)List[index];
			}
			set
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// 删除一个条件项
		/// </summary>
		/// <param name="item"></param>
		public void Remove(ConditionMappingItem item)
		{
			List.Remove(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		protected override void OnValidate(object value)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");
		}
	}
}
