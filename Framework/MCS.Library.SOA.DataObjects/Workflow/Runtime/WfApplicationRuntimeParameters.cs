using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MCS.Library.Core;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	public class WfApplicationRuntimeParameters : WfContextDictionaryBase<string, object>
	{
		[NonSerialized]
		private IWfProcess _ProcessInstance = null;

		public WfApplicationRuntimeParameters()
		{
		}

		public WfApplicationRuntimeParameters(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public IWfProcess ProcessInstance
		{
			get { return this._ProcessInstance; }
			internal set { this._ProcessInstance = value; }
		}

		/// <summary>
		/// 如果在当前流程中找不到参数，则尝试地去父流程中找，直到找到为止
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="paramName"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public T GetValueRecursively<T>(string paramName, T defaultValue)
		{
			return GetValueRecursively(paramName, WfProbeApplicationRuntimeParameterMode.Auto, defaultValue);
		}

		/// <summary>
		/// 如果在当前流程中找不到参数。根据probeMode参数决定是否查找父流程
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="paramName"></param>
		/// <param name="probeMode"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public T GetValueRecursively<T>(string paramName, WfProbeApplicationRuntimeParameterMode probeMode, T defaultValue)
		{
			object result = null;

			if (InnerGetValueRecursively<T>(this, ProcessInstance, probeMode, paramName, out result) == false)
				result = defaultValue;

			return (T)result;
		}

		private static bool InnerGetValueRecursively<T>(WfApplicationRuntimeParameters parameters, IWfProcess process, WfProbeApplicationRuntimeParameterMode probeMode, string paramName, out object result)
		{
			paramName.CheckStringIsNullOrEmpty("paramName");

			result = null;
			bool existed = false;

			if (parameters.Contains(paramName))
			{
				object dataInARP = parameters[paramName];

				if (dataInARP is T)
					result = (T)dataInARP;
				else
					result = DataConverter.ChangeType(parameters[paramName], typeof(T));

				existed = true;
			}
			else
			{
				if (process != null)
				{
					bool probParent = false;

					switch (probeMode)
					{
						case WfProbeApplicationRuntimeParameterMode.Auto:
							probParent = process.Descriptor.Properties.GetValue("ProbeParentProcessParams", false);
							break;
						case WfProbeApplicationRuntimeParameterMode.NotRecursively:
							probParent = false;
							break;
						case WfProbeApplicationRuntimeParameterMode.Recursively:
							probParent = true;
							break;
					}

					if (probParent && process.HasParentProcess)
						existed = InnerGetValueRecursively<T>(process.EntryInfo.OwnerActivity.Process.ApplicationRuntimeParameters, process.EntryInfo.OwnerActivity.Process, probeMode, paramName, out result);
				}
			}

			return existed;
		}

		/// <summary>
		/// 根据模板，得到参数值
		/// </summary>
		/// <param name="template"></param>
		/// <returns></returns>
		public string GetMatchedString(string template)
		{
			Regex reg = new Regex(@"\$\{[0-9 a-z A-Z]*?\}\$");

			return reg.Replace(template, new MatchEvaluator(MatchEvaluatorHandler));
		}

		private string MatchEvaluatorHandler(Match m)
		{
			string result = string.Empty;

			string paramName = m.Value.Substring(2, m.Length - 4);

			result = GetValueRecursively(paramName, string.Empty);

			return result;
		}
	}
}
