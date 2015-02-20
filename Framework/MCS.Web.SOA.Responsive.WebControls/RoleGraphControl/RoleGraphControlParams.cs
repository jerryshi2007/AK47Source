using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Responsive.WebControls
{
	[Serializable]
	public class RoleGraphControlParams 
	{
		public const string DefaultDialogTitle = "请选择角色";

		private string _SelectedFullCodeName = string.Empty;

		public string SelectedFullCodeName
		{
			get { return this._SelectedFullCodeName; }
			set { this._SelectedFullCodeName = value; }
		}

		//TODO:补充
	}
}
