using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace MCS.Library.Accredit.ExpParsing
{
	/// <summary>
	/// ���ʽ�༭��
	/// </summary>
	public interface IExpEditor
	{
		/// <summary>
		/// ��ȡ����
		/// </summary>
		/// <returns></returns>
		XmlNode GetNameSpaceNode();
	}
}
