using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data
{
	/// <summary>
	/// 描述命令的接口
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// 命令名称
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// 执行命令
		/// </summary>
		/// <param name="argument"></param>
		void Execute(string argument);
		
		/// <summary>
		/// 命令的描述
		/// </summary>
		string HelperString { get; }
	}
}
