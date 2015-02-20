using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 直接调用Web Service的任务。
	/// </summary>
	[Serializable]
	public class InvokeServiceTask : SysTask
	{
		[NonSerialized]
		private WfServiceOperationDefinitionCollection _SvcOperationDefs;

		public InvokeServiceTask()
		{
			this.TaskType = "InvokeServiceDirectly";
		}

		public InvokeServiceTask(SysTask other)
			: base(other)
		{
			this.AfterLoad();
		}

		[NoMapping]
		public WfServiceOperationDefinitionCollection SvcOperationDefs
		{
			get
			{
				if (this._SvcOperationDefs == null)
					this._SvcOperationDefs = new WfServiceOperationDefinitionCollection();

				return this._SvcOperationDefs;
			}
		}

		public override void FillData(Dictionary<string, string> extraData)
		{
			if (extraData == null)
				extraData = new Dictionary<string, string>();

			if (this._SvcOperationDefs != null)
				extraData["SvcOperationDefs"] = GetSerilizedServiceDefs(this._SvcOperationDefs);

			extraData["ContextData"] = GetSerilizedContextData(this.Context);

			base.FillData(extraData);
		}

		public override void AfterLoad()
		{
			if (this.Data.IsNotEmpty())
			{
				XmlDocument xmlDoc = XmlHelper.CreateDomDocument(this.Data);

				string serilizedSvcDefData = XmlHelper.GetSingleNodeText(xmlDoc.DocumentElement, "SvcOperationDefs");

				this._SvcOperationDefs = GetDeserilizedServiceDefs(serilizedSvcDefData);

				string serilizedContextData = XmlHelper.GetSingleNodeText(xmlDoc.DocumentElement, "ContextData");

				Dictionary<string, object> contextData = GetDeserilizedContextData(serilizedContextData);

				if (contextData != null)
				{
					foreach (KeyValuePair<string, object> kp in contextData)
					{
						this.Context[kp.Key] = kp.Value;
					}
				}
			}
		}

		private static WfServiceOperationDefinitionCollection GetDeserilizedServiceDefs(string data)
		{
			WfServiceOperationDefinitionCollection result = null;

			if (data.IsNotEmpty())
			{
				XElement root = XElement.Parse(data);

				XElementFormatter formatter = new XElementFormatter();

				result = (WfServiceOperationDefinitionCollection)formatter.Deserialize(root);
			}

			return result;
		}

		private static Dictionary<string, object> GetDeserilizedContextData(string contextData)
		{
			Dictionary<string, object> result = new Dictionary<string, object>();

			if (contextData.IsNotEmpty())
			{
				XElement root = null;

				if (TryParseXElement(contextData, out root))
				{
					XElementFormatter formatter = new XElementFormatter();
					result = (Dictionary<string, object>)formatter.Deserialize(root);
				}
				else
					result = JSONSerializerExecute.Deserialize<Dictionary<string, object>>(contextData);
			}

			return result;
		}

		private static string GetSerilizedServiceDefs(WfServiceOperationDefinitionCollection svcOperationDefs)
		{
			XElementFormatter formatter = new XElementFormatter();

			return formatter.Serialize(svcOperationDefs).ToString();
		}

		private static string GetSerilizedContextData(Dictionary<string, object> context)
		{
			return JSONSerializerExecute.Serialize(context);
		}

		private static bool TryParseXElement(string data, out XElement root)
		{
			bool result = false;

			try
			{
				root = XElement.Parse(data);
				result = true;
			}
			catch (XmlException)
			{
				root = null;
			}

			return result;
		}
	}
}
