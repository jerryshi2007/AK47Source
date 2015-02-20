using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using System.IO;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 基于文件的流程定义管理器
	/// </summary>
	public class WfFileProcessDescriptorManager : WfProcessDescriptorManagerBase
	{
		public override bool ExsitsProcessKey(string processKey)
		{
			return File.Exists(GetPath(processKey));
		}

		/// <summary>
		/// 从文件加载Xml
		/// </summary>
		/// <param name="processKey"></param>
		/// <returns></returns>
		protected override XElement LoadXml(string processKey)
		{
			return XmlHelper.LoadElement(GetPath(processKey));
		}

		/// <summary>
		/// 将Xml保存到文件
		/// </summary>
		/// <param name="processKey"></param>
		/// <param name="xml"></param>
		protected override void SaveXml(IWfProcessDescriptor processDesp, XElement xml)
		{
			xml.Save(GetPath(processDesp.Key));
		}

		public override void DeleteDescriptor(string processKey)
		{
			if (this.ExsitsProcessKey(processKey))
			{
				File.Delete(GetPath(processKey));
				WfMatrixAdapter.Instance.DeleteByProcessKey(processKey);
			}
		}

		private static string GetPath(string processKey)
		{
			string rootPath = EnvironmentHelper.ReplaceEnvironmentVariablesInString(FileProcessDescriptorSettings.GetConfig().RootPath);

			return Path.Combine(rootPath, processKey + ".xml");
		}
	}
}
