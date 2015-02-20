using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class CommonNodeInfo<T> : ElementInfo
	{
		public CommonNodeInfo(T instance)
		{
			this._InnerXml = string.Empty;
			this._Instance = instance;
		}

		private string _InnerXml;
		public string InnerXml
		{
			get
			{
				return this._InnerXml;
			}
			set
			{
				this._InnerXml = value;
			}
		}

		private T _Instance;

		private string _NodeName = string.Empty;
		protected internal override string NodeName
		{
			get
			{
				return this._NodeName;
			}
		}
	}
}
