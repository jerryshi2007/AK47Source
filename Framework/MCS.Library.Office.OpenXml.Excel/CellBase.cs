using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public abstract class CellBase
	{
		#region properties
		internal object _Value = null;

		/// <summary>
		/// 获取或设置单元格的值。
		/// </summary>
		public object Value
		{
			get
			{
				return this._Value;
			}
			set
			{
				this._Value = value;
			}
		}
		

		internal CellStyleXmlWrapper _Style = null;

		public virtual CellStyleXmlWrapper Style
		{
			get
			{
				if (this._Style == null)
					this._Style = new CellStyleXmlWrapper();

				return this._Style;
			}
			set
			{
				this._Style = value;
			}
		}
		#endregion

		/// <summary>
		/// 根据据类型拿值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetTypedValue<T>()
		{
			return ExcelHelper.GetTypedValue<T>(this.Value);
		}
	}
}
