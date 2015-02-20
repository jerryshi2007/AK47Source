using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MCS.Library.Data.DataObjects;
using System.Collections;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[DebuggerDisplay("Name = {QueryName}")]
	public class SOARolePropertiesQueryParam
	{
		public string QueryName { get; set; }
		public object QueryValue { get; set; }

		/// <summary>
		/// 比较QueryValue是否等于targetValue。如果QueryValue是集合，则匹配到任何一项均可
		/// </summary>
		/// <param name="targetValue"></param>
		/// <returns></returns>
		public bool MatchQueryValue(object targetValue)
		{
			bool result = false;

			if (targetValue != null)
			{
				if (this.QueryValue is string)
				{
					result = targetValue.Equals(this.QueryValue);
				}
				else
				{
					if (this.QueryValue is IEnumerable)
					{
						foreach (object data in (IEnumerable)this.QueryValue)
						{
							if (data != null)
							{
								if (targetValue.Equals(data.ToString()))
								{
									result = true;
									break;
								}
							}
						}
					}
					else
					{
						if (this.QueryValue != null)
							result = targetValue.Equals(this.QueryValue.ToString());
					}
				}
			}

			return result;
		}
	}

	[Serializable]
	public class SOARolePropertiesQueryParamCollection : EditableDataObjectCollectionBase<SOARolePropertiesQueryParam>
	{
	}
}
