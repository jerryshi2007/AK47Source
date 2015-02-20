using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	internal struct DataTypeConverterPair
	{
		public Type SourceType
		{
			get;
			set;
		}

		public Type DestinationType
		{
			get;
			set;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public static class DataTypeConverterHelper
	{
		private static Dictionary<DataTypeConverterPair, DataTypeConverterBase> _Converters = new Dictionary<DataTypeConverterPair, DataTypeConverterBase>();

		/// <summary>
		/// 注册数据类型转换器
		/// </summary>
		/// <param name="sourceType"></param>
		/// <param name="destType"></param>
		/// <param name="converter"></param>
		public static void RegisterConverter(Type sourceType, Type destType, DataTypeConverterBase converter)
		{
			lock (_Converters)
			{
				DataTypeConverterPair pair = new DataTypeConverterPair();

				pair.SourceType = sourceType;
				pair.DestinationType = destType;

				if (_Converters.ContainsKey(pair) == false)
					_Converters[pair] = converter;
			}
		}

		/// <summary>
		/// 根据类型得到转换器
		/// </summary>
		/// <param name="sourceType"></param>
		/// <param name="destType"></param>
		/// <returns></returns>
		public static DataTypeConverterBase GetConverter(Type sourceType, Type destType)
		{
			sourceType.NullCheck("sourceType");
			destType.NullCheck("destType");

			lock (_Converters)
			{
				DataTypeConverterPair pair = new DataTypeConverterPair();

				pair.SourceType = sourceType;
				pair.DestinationType = destType;

				DataTypeConverterBase converter = null;

				_Converters.TryGetValue(pair, out converter).FalseThrow<KeyNotFoundException>(
					"不能找到从类型{0}转换到类型{1}的转换器", sourceType, destType);

				return converter;
			}
		}
	}
}
