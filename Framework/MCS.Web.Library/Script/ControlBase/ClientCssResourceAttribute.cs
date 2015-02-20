using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Web.Library.Script
{
	/// <summary>
	/// �ͻ���Css��Դ�ļ�����Attribute
	/// </summary>
	/// <remarks>�ͻ���Css��Դ�ļ�����Attribute</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "The composition of baseType and resourceName is available as ResourcePath")]
    public sealed class ClientCssResourceAttribute : Attribute
    {
        private string _resourcePath;
        private int _loadOrder;

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="fullResourceName">css��Դ�ļ�����</param>
		/// <remarks>���캯��</remarks>
        public ClientCssResourceAttribute(string fullResourceName)
        {
            if (fullResourceName == null) throw new ArgumentNullException("fullResourceName");
            _resourcePath = fullResourceName;
        }

		/// <summary>
		/// css��Դ�ļ�ȫ���ƣ������ռ�+�ļ�����
		/// </summary>
		/// <remarks>css��Դ�ļ�ȫ���ƣ������ռ�+�ļ�����</remarks>
        public string ResourcePath
        {
            get { return _resourcePath; }
        }

		/// <summary>
		/// ����˳��
		/// </summary>
		/// <remarks>����˳��</remarks>
        public int LoadOrder
        {
            get { return _loadOrder; }
            set { _loadOrder = value; }
        }
    }
}
