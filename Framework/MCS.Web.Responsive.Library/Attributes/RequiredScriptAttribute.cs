// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MCS.Web.Responsive.Library.Script
{
	/// <summary>
	///	�ڿؼ����б�ǣ������ؼ�����Ľű�
	/// </summary>
	/// <remarks>�ڿؼ����б�ǣ������ؼ�����Ľű�</remarks>
	/// <example>[RequiredScript(typeof(ControlBaseScript))]</example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class RequiredScriptAttribute : Attribute
    {
        private int _order;
        private Type _extenderType;
        private string _scriptName;

		/// <summary>
		/// ��ű���Դ�����������
		/// </summary>
        public Type ExtenderType
        {
            get { return _extenderType; }
        }

		/// <summary>
		/// �ű���Դ����
		/// </summary>
        public string ScriptName
        {
            get { return _scriptName; }
        }

		/// <summary>
		/// ���ؽű���Դ˳��
		/// </summary>
        public int LoadOrder
        {
            get { return _order; }
        }

		/// <summary>
		/// ���캯��
		/// </summary>
        public RequiredScriptAttribute()
        {
        }

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="scriptName">�ű���Դ����</param>
        public RequiredScriptAttribute(string scriptName)            
        {
            _scriptName = scriptName;
        }

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="extenderType">��ű���Դ�����������</param>
        public RequiredScriptAttribute(Type extenderType): this(extenderType, 0) {
        }

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="extenderType">��ű���Դ�����������</param>
		/// <param name="loadOrder">���ؽű���Դ˳��</param>
        public RequiredScriptAttribute(Type extenderType, int loadOrder) 
        {
            _extenderType = extenderType;
            _order = loadOrder;
        }
    }
}
