using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;

namespace MCS.Web.Apps.WeChat.Commands
{
	/// <summary>
	/// 
	/// </summary>
	public class HelpCommand : CommandBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public HelpCommand(string name)
			: base(name)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argument"></param>
		public override void Execute(string argument)
		{
			Console.WriteLine("命令帮助:");
			foreach (ICommand command in CommandHelper.GetCommandList())
			{
				Console.WriteLine(command);
			}

			Console.WriteLine("\nType 'exit' to quit this program.");
		}

		/// <summary>
		/// 
		/// </summary>
		public override string HelperString
		{
			get
			{
				return "?";
			}
		}
	}
}
