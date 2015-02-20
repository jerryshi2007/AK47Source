using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Office.OpenXml.Excel.Encryption;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class SheetProtection : ElementInfo
	{
		protected internal override string NodeName
		{
			get { return "sheetProtection"; }
		}

		public bool AutoFilter
		{
			get
			{
				return !base.GetBooleanAttribute("autoFilter").Equals("1");
			}
			set
			{
				this.Set_BoolAttribute("autoFilter", value);
			}
		}

		public bool AllowDeleteColumns
		{
			get
			{
				return !base.GetBooleanAttribute("deleteColumns");
			}
			set
			{
				this.Set_BoolAttribute("deleteColumns", value);
			}
		}

		public bool AllowDeleteRows
		{
			get
			{
				return !base.GetBooleanAttribute("deleteRows");
			}
			set
			{
				this.Set_BoolAttribute("deleteRows", value);
			}
		}

		/// <summary>
		/// 允许格式化所有的单元格
		/// </summary>
		public bool AllowFormatCells
		{
			get
			{
				return !base.GetBooleanAttribute("formatCells");
			}
			set
			{
				this.Set_BoolAttribute("formatCells", value);
			}
		}

		/// <summary>
		/// 允许用户设置列格式
		/// </summary>
		public bool AllowFormatColumns
		{
			get
			{
				return !base.GetBooleanAttribute("formatColumns");
			}
			set
			{
				this.Set_BoolAttribute("formatColumns", value);
			}
		}

		public bool AllowFormatRows
		{
			get
			{
				return !base.GetBooleanAttribute("formatRows");
			}
			set
			{
				this.Set_BoolAttribute("formatRows", value);
			}
		}

		public bool AllowInsertColumns
		{
			get
			{
				return !base.GetBooleanAttribute("insertColumns");
			}
			set
			{
				this.Set_BoolAttribute("insertColumns", value);
			}
		}

		public bool AllowInsertHyperlinks
		{
			get
			{
				return !base.GetBooleanAttribute("insertHyperlinks");
			}
			set
			{
				this.Set_BoolAttribute("insertHyperlinks", value);
			}
		}

		public bool AllowInsertRows
		{
			get
			{
				return !base.GetBooleanAttribute("insertRows");
			}
			set
			{
				this.Set_BoolAttribute("insertRows", value);
			}
		}

		/// <summary>
		/// 允许用户编辑对象
		/// </summary>
		public bool AllowEditObject
		{
			get
			{
				return !base.GetBooleanAttribute("objects");
			}
			set
			{
				this.Set_BoolAttribute("objects", value);
			}
		}

		/*public string Password
		{
			get
			{
				return base.GetAttribute("password");
			}
			set
			{
				base.SetAttribute("password", value);
			}
		}*/

		public void SetPassword(string password)
		{
			if (this.IsProtected == false)
				this.IsProtected = true;
		
			password = password.Trim();
			if (string.IsNullOrEmpty(password))
			{
				base.Attributes.Remove("password");
				return;
			}
			int hash = EncryptedPackageHandler.CalculatePasswordHash(password);
			base.SetAttribute("password", ((int)hash).ToString("x"));
		}

		public bool PivotTables
		{
			get
			{
				return !base.GetBooleanAttribute("pivotTables");
			}
			set
			{
				this.Set_BoolAttribute("pivotTables", value);
			}
		}

		public bool Scenarios
		{
			get
			{
				return !base.GetBooleanAttribute("scenarios");
			}
			set
			{
				this.Set_BoolAttribute("scenarios", value);
			}
		}

		/// <summary>
		/// 允许用户选择锁定的单元格
		/// </summary>
		public bool AllowSelectLockedCells
		{
			get
			{
				return !base.GetBooleanAttribute("selectLockedCells");
			}
			set
			{
				this.Set_BoolAttribute("selectLockedCells", value);
			}
		}

		/// <summary>
		/// 允许用户选择锁定单元格 
		/// </summary>
		public bool SelectUnlockedCells
		{
			get
			{
				return !base.GetBooleanAttribute("selectUnlockedCells");
			}
			set
			{
				this.Set_BoolAttribute("selectUnlockedCells", value);
			}
		}

		/// <summary>
		/// 是否保护
		/// </summary>
		public bool IsProtected
		{
			get
			{
				return base.GetBooleanAttribute("sheet");
			}
			set
			{
				if (value)
				{
					base.SetAttribute("sheet", "1");
					base.SetAttribute("objects", "0");
					base.SetAttribute("scenarios", "0");
				}
				else
				{
					base.Attributes.Remove("sheet");
				}
			}
		}

		/// <summary>
		/// 排序保护
		/// </summary>
		public bool AllowSort
		{
			get
			{
				return !base.GetBooleanAttribute("sort");
			}
			set
			{
				this.Set_BoolAttribute("sort", value);
			}
		}

		private void Set_BoolAttribute(string key, bool value)
		{
			if (value)
			{
				this.Attributes[key] = "0";
			}
			else
			{
				this.Attributes.Remove(key);
			}
		}
	}
}
