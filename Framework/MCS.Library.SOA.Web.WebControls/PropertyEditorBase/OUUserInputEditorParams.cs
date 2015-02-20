using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 内部使用OUUserInput控件的创建参数
	/// </summary>
	internal class OUUserInputEditorParams
	{
		private UserControlObjectMask _selectMask = UserControlObjectMask.All;
		private UserControlObjectMask _listMask = UserControlObjectMask.All;

		public UserControlObjectMask selectMask
		{
			get
			{
				return this._selectMask;
			}
			set
			{
				this._selectMask = value;
			}
		}

		public UserControlObjectMask listMask
		{
			get
			{
				return this._listMask;
			}
			set
			{
				this._listMask = value;
			}
		}

		public bool multiSelect
		{
			get;
			set;
		}

		public bool allowSelectDuplicateObj
		{
			get;
			set;
		}

		public string checkingText
		{
			get;
			set;
		}
	}
}
