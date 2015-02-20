using System;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Configuration;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 和工作流控件有关的配置
	/// </summary>
	public class WfControlSettings : ConfigurationSection
	{
		private WfControlSettings()
		{
		}

		/// <summary>
		/// 得到工作流控件的配置信息
		/// </summary>
		/// <returns></returns>
		public static WfControlSettings GetConfig()
		{
			WfControlSettings settings = (WfControlSettings)ConfigurationBroker.GetSection("wfControlSettings");

			if (settings == null)
				settings = new WfControlSettings();

			return settings;
		}

		[ConfigurationProperty("activityEditorVariables", IsRequired = false)]
		public WfActivityEditorVariableCollection ActivityEditorVariables
		{
			get
			{
				return (WfActivityEditorVariableCollection)this["activityEditorVariables"];
			}
		}

		/// <summary>
		/// 正常加签的时候，从原始点需要复制的变量
		/// </summary>
		[ConfigurationProperty("normalAddActivityCopyActivities", IsRequired = false)]
		public WfActivityEditorVariableCollection NormalAddActivityCopyActivities
		{
			get
			{
				return (WfActivityEditorVariableCollection)this["normalAddActivityCopyActivities"];
			}
		}

		/// <summary>
		/// 退件加签的时候，从原始点需要复制的变量
		/// </summary>
		[ConfigurationProperty("returnAddActivityCopyActivities", IsRequired = false)]
		public WfActivityEditorVariableCollection ReturnAddActivityCopyActivities
		{
			get
			{
				return (WfActivityEditorVariableCollection)this["returnAddActivityCopyActivities"];
			}
		}

		[ConfigurationProperty("enabled", IsRequired = false, DefaultValue = false)]
		public bool Enabled
		{
			get
			{
				return (bool)this["enabled"];
			}
			set
			{
				this["enabled"] = value;
			}
		}

		public WfActivityDescriptorEditorBase CreateActivityDescriptorEditor()
		{
			WfActivityDescriptorEditorBase result = null;

			if (Impls.ContainsKey("activityDescriptorEditor"))
				result = (WfActivityDescriptorEditorBase)Impls["activityDescriptorEditor"].CreateInstance();
			else
				result = new WfActivityDescriptorEditor();

			return result;
		}

		[ConfigurationProperty("impls", IsRequired = false)]
		private TypeConfigurationCollection Impls
		{
			get
			{
				return (TypeConfigurationCollection)this["impls"];
			}
		}
	}

	/// <summary>
	/// 节点编辑时，变量定义的集合
	/// </summary>
	public class WfActivityEditorVariableCollection : NamedConfigurationElementCollection<WfActivityEditorVariableElement>
	{
		/// <summary>
		/// 直接提取变量名
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> ToVariableNames()
		{
			List<string> result = new List<string>();

			foreach (WfActivityEditorVariableElement elem in this)
			{
				result.Add(elem.Name);
			}

			return result;
		}
	}

	/// <summary>
	/// 节点编辑时，变量定义
	/// </summary>
	public class WfActivityEditorVariableElement : NamedConfigurationElement
	{
		/// <summary>
		/// 是否可见
		/// </summary>
		[ConfigurationProperty("visible", IsRequired = false, DefaultValue = true)]
		public bool Visible
		{
			get
			{
				return (bool)this["visible"];
			}
			set
			{
				this["visible"] = value;
			}
		}

		/// <summary>
		/// 缺省值
		/// </summary>
		[ConfigurationProperty("defaultValue", IsRequired = false, DefaultValue = true)]
		public bool DefaultValue
		{
			get
			{
				return (bool)this["defaultValue"];
			}
			set
			{
				this["defaultValue"] = value;
			}
		}
	}
}
