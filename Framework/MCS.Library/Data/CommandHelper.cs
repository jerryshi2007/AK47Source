using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Data
{
	/// <summary>
	/// 操作ICommand对象的相关类别
	/// </summary>
	public static class CommandHelper
	{
		private static Dictionary<string, ICommand> _Commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// 在全局字典中注册一个Command
		/// </summary>
		/// <param name="command"></param>
		public static void RegisterCommand(ICommand command)
		{
			command.NullCheck("command");

			lock (_Commands)
			{
				_Commands[command.Name] = command;
			}
		}

		/// <summary>
		/// 从字典中获取一个Command
		/// </summary>
		/// <param name="commandName">命令的名称</param>
		/// <returns>得到命令</returns>
		public static ICommand GetCommand(string commandName)
		{
			commandName.CheckStringIsNullOrEmpty("commandName");

			ICommand result = null;

			lock (_Commands)
			{
				_Commands.TryGetValue(commandName, out result);
			}

			return result;
		}

		/// <summary>
		/// 得到按照名称排序后的命令列表
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<ICommand> GetCommandList()
		{
			List<ICommand> result = new List<ICommand>();

			lock (_Commands)
			{
				foreach (KeyValuePair<string, ICommand> kp in _Commands)
					result.Add(kp.Value);
			}

			result.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));

			return result;
		}
	}
}
