using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Web.UI;
using System.IO;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 仿真上下文
	/// </summary>
	public class WfSimulationContext : Dictionary<string, object>
	{
		private StringBuilder _Builder = null;
		private HtmlTextWriter _Writer = null;

		private WfSimulationParameters _SimulationParameters = null;

		public WfSimulationContext()
		{
			Initialize();
		}

		public WfSimulationParameters SimulationParameters
		{
			get
			{
				if (this._SimulationParameters == null)
					this._SimulationParameters = new WfSimulationParameters();

				return this._SimulationParameters;
			}
			internal set
			{
				this._SimulationParameters = value;
			}
		}

		/// <summary>
		/// 得到模拟的连接名，如果Enabled则使用模拟的连接名称
		/// </summary>
		/// <param name="origianlConnectionName">如果不仿真，使用原来的连接串</param>
		/// <returns></returns>
		public string GetConnectionName(string origianlConnectionName)
		{
			string result = origianlConnectionName;

			if (WfRuntime.ProcessContext.EnableSimulation && WfSimulationSettings.GetConfig().ConnectionName.IsNotEmpty())
				result = WfSimulationSettings.GetConfig().ConnectionName;

			return result;
		}

		public HtmlTextWriter Writer
		{
			get
			{
				return this._Writer;
			}
		}

		/// <summary>
		/// 流转次数
		/// </summary>
		public int MoveToCount
		{
			get;
			internal set;
		}

		public string GetOutputString()
		{
			if (this._Writer != null)
				this._Writer.Flush();

			string result = string.Empty;

			if (this._Builder != null)
				result = this._Builder.ToString();

			return result;
		}

		public void Initialize()
		{
			this._Builder = new StringBuilder(1024);

			StringWriter writer = new StringWriter(this._Builder);

			this._Writer = new HtmlTextWriter(writer);
		}
	}
}
