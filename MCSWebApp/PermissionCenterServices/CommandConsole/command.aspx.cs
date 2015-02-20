using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.OGUPermission.Commands;

namespace PermissionCenter.CommandConsole
{
	public partial class command : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			extenderTemplate.DataTextFieldList = new string[] { "Text" };
		}

		protected void extenderTemplate_GetDataSource(string sPrefix, int iCount, object context, ref IEnumerable result)
		{
			OguCommandHelper.RegisterAllCommands();

			DataTable dtTrans = new DataTable();

			dtTrans.Columns.Add("ID", typeof(string));
			dtTrans.Columns.Add("Text", typeof(string));
			dtTrans.Columns.Add("Value", typeof(string));

			int i = 0;
			foreach (ICommand command in FilterCommands(sPrefix))
			{
				DataRow row = dtTrans.NewRow();

				row["ID"] = string.Empty;
				row["Text"] = command.HelperString;
				row["Value"] = command.Name;

				dtTrans.Rows.Add(row);

				i++;
			}

			result = (IEnumerable)dtTrans.DefaultView;
		}

		private static List<ICommand> FilterCommands(string sPrefix)
		{
			List<ICommand> commands = new List<ICommand>();

			if (sPrefix.IsNotEmpty())
			{
				foreach (ICommand command in CommandHelper.GetCommandList())
				{
					if (command.Name.IndexOf(sPrefix, StringComparison.OrdinalIgnoreCase) == 0)
						commands.Add(command);
				}
			}

			return commands;
		}
	}
}