using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// 表达式编辑器
	/// </summary>
	public interface IExpEditor
	{
		/// <summary>
		/// 获取命名
		/// </summary>
		/// <returns></returns>
		XmlNode GetNameSpaceNode();
	}
}
