using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Data
{
	/// <summary>
	/// 实现ICommand接口的虚基类。为Command提供了基础的数据结构
	/// </summary>
	public abstract class CommandBase : ICommand
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="name"></param>
		public CommandBase(string name)
		{
			this.Name = name;
		}

		#region ICommand Members

		/// <summary>
		/// 命令的名称
		/// </summary>
		public string Name
		{
			get;
			protected set;
		}

		/// <summary>
		/// 执行命令
		/// </summary>
		/// <param name="argument"></param>
		public abstract void Execute(string argument);

		/// <summary>
		/// 帮助信息
		/// </summary>
		public virtual string HelperString
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// 转换成String
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("\t{0}: {1}", this.Name, this.HelperString);
		}
		#endregion
	}
}
