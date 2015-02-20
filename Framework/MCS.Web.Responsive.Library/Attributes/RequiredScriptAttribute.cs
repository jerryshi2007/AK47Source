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
	///	在控件类中标记，表明控件所需的脚本
	/// </summary>
	/// <remarks>在控件类中标记，表明控件所需的脚本</remarks>
	/// <example>[RequiredScript(typeof(ControlBaseScript))]</example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public sealed class RequiredScriptAttribute : Attribute
    {
        private int _order;
        private Type _extenderType;
        private string _scriptName;

		/// <summary>
		/// 与脚本资源相关联的类型
		/// </summary>
        public Type ExtenderType
        {
            get { return _extenderType; }
        }

		/// <summary>
		/// 脚本资源名称
		/// </summary>
        public string ScriptName
        {
            get { return _scriptName; }
        }

		/// <summary>
		/// 加载脚本资源顺序
		/// </summary>
        public int LoadOrder
        {
            get { return _order; }
        }

		/// <summary>
		/// 构造函数
		/// </summary>
        public RequiredScriptAttribute()
        {
        }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="scriptName">脚本资源名称</param>
        public RequiredScriptAttribute(string scriptName)            
        {
            _scriptName = scriptName;
        }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="extenderType">与脚本资源相关联的类型</param>
        public RequiredScriptAttribute(Type extenderType): this(extenderType, 0) {
        }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="extenderType">与脚本资源相关联的类型</param>
		/// <param name="loadOrder">加载脚本资源顺序</param>
        public RequiredScriptAttribute(Type extenderType, int loadOrder) 
        {
            _extenderType = extenderType;
            _order = loadOrder;
        }
    }
}
