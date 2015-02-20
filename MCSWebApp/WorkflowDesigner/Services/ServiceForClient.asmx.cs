using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;

namespace WorkflowDesigner.Services
{
	/// <summary>
	/// 为客户端调用所提供的服务
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	[ScriptService]
	public class ServiceForClient : System.Web.Services.WebService
	{
		public ServiceForClient()
		{
			JSONSerializerExecute.RegisterConverter(typeof(PropertyValueConverter));
		}

		[WebMethod]
		public string GetGlobalParametersJSON(string key)
		{
			WfApplicationProgramCodeName codeNames = new WfApplicationProgramCodeName(key);

			string programName = "全局参数";

			WfProgramInApplicationCollection programs = WfApplicationAdapter.Instance.LoadProgramsByApplication(codeNames.ApplicationCodeName);

			WfProgram program = programs.Find(p => p.CodeName == codeNames.ProgramCodeName);

			if (program != null)
				programName = program.Name;

			var result = new { Key = key, Name = programName, Properties = WfGlobalParameters.LoadProperties(key).Properties };

			return JSONSerializerExecute.Serialize(result);
		}

		[WebMethod]
		public void UpdateGlobalParameters(string key, string propertiesJson)
		{
			PropertyValueCollection properties = JSONSerializerExecute.Deserialize<PropertyValueCollection>(propertiesJson);

			WfGlobalParameters parameters = WfGlobalParameters.LoadProperties(key);

			parameters.Properties.Clear();
			parameters.Properties.CopyFrom(properties);

			parameters.Update();
		}

		[WebMethod]
		[ScriptMethod]
		public WfServiceAddressDefinitionCollection GetDefaultGlobalServiceAddresses()
		{
			return WfGlobalParameters.Default.ServiceAddressDefs;
		}

		[WebMethod]
		[ScriptMethod]
		public WfServiceAddressDefinition GetDefaultGlobalServiceAddressesByKey(string key)
		{
			return WfGlobalParameters.Default.ServiceAddressDefs[key];
		}
	}
}
