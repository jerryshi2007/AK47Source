using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.OGUPermission.Commands;
using System.Text;
using System.IO;

namespace PermissionCenter.CommandConsole
{
	/// <summary>
	/// Summary description for CommondService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	[System.Web.Script.Services.ScriptService]
	public class CommandService : System.Web.Services.WebService
	{
		static CommandService()
		{
			OguCommandHelper.RegisterAllCommands();
		}

		[WebMethod]
		public string ExecCommand(string cmdString)
		{
			if (cmdString.IsNullOrEmpty())
				throw new InvalidOperationException("Empty command name.");

			StringBuilder strB = new StringBuilder();

			using (StringWriter sw = new StringWriter(strB))
			{
				Console.SetOut(sw);

				KeyValuePair<string, string> cmdParams = ParseCommandString(cmdString);

				if (cmdParams.Key.IsNotEmpty())
				{
					ICommand cmd = CommandHelper.GetCommand(cmdParams.Key);

					if (cmd != null)
					{
						cmd.Execute(cmdParams.Value);
					}
					else
						throw new InvalidOperationException(string.Format("Invalid command name: '{0}'.", cmdParams.Key));
				}

				return strB.ToString();
			}
		}

		private static KeyValuePair<string, string> ParseCommandString(string cmdString)
		{
			string cmdName = cmdString;
			string argument = string.Empty;

			int index = cmdString.IndexOf(' ');

			if (index != -1)
			{
				cmdName = cmdString.Substring(0, index);
				argument = cmdString.Substring(index + 1).Trim(' ');
			}

			return new KeyValuePair<string, string>(cmdName, argument);
		}
	}
}
