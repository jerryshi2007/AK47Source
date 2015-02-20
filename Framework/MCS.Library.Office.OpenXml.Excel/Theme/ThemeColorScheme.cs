using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MCS.Library.Office.OpenXml.Excel.Theme;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class ThemeColorScheme : IDisposable
	{
		private Dictionary<int, IThemeColorScheme> _Colors;
		private ThemeElements _Elements;
		private string _Name;

		public ThemeColorScheme(ThemeElements els)
		{
			this._Elements = els;
		}

		public void Dispose()
		{
			foreach (IThemeColorScheme color in this.Colors.Values)
			{
				color.Dispose();
			}
		}

		public Dictionary<int, IThemeColorScheme> Colors
		{
			get
			{
				if (this._Colors == null)
					this._Colors = new Dictionary<int, IThemeColorScheme>();
				return this._Colors;
			}
		}

		public ThemeElements Elements
		{
			get
			{
				return this._Elements;
			}
		}

		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				_Name = value;
			}
		}
	}
}
