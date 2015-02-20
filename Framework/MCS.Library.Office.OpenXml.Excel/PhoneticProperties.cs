using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class PhoneticProperties : ElementInfo
	{
		public PhoneticProperties()
		{
			this.FontId = 0;
			this.Type = "noConversion";
		}

		public int FontId
		{
			get
			{
				string fontId = base.GetAttribute("fontId");
				if (fontId.IsNotEmpty())
				{
					return int.Parse(fontId);
				}
				return 0;
			}
			set
			{
				base.SetAttribute("fontId", value.ToString());
			}
		}

		public string Type
		{
			get
			{
				return base.GetAttribute("type");
			}
			set
			{
				base.SetAttribute("type", value);
			}
		}

		protected internal override string NodeName
		{
			get { return "phoneticPr"; }
		}
	}
}
