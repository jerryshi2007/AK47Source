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
	/// �������ʽ�Ͷ���ӳ����Ŀ����
	/// </summary>
	public class ConditionMappingItemCollection : DataObjectCollectionBase<ConditionMappingItem>
	{
		/// <summary>
		/// ���һ��������
		/// </summary>
		/// <param name="item"></param>
		public void Add(ConditionMappingItem item)
		{
			InnerAdd(item);
		}

		/// <summary>
		/// ����������ӻ�����һ��������
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
		/// ɾ��һ��������
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
