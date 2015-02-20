using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MCS.Library.SOA.DataObjects.Security.Validators;
using MCS.Library.SOA.DataObjects.Security.AUObjects;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;

namespace AUCenter.Services
{
	/// <summary>
	/// CommonServices 的摘要说明
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
	[System.Web.Script.Services.ScriptService]
	public class CommonServices : System.Web.Services.WebService
	{
		[WebMethod]
		public bool ValidateCodeNameUnique(string schemaType, string id, string parentID, string currentValue, bool includingDeleted)
		{
			return AUCommon.DoDbProcess<bool>(() => CodeNameUniqueValidatorFacade.Validate(currentValue, id, schemaType, parentID, includingDeleted == false, false, DateTime.MinValue));
		}

		[WebMethod]
		public string GetPinYin(string schemaType, string id, string parentID, string currentValue, bool includingDeleted)
		{
			string result = string.Empty;

			List<string> strPinYin = SCSnapshotAdapter.Instance.GetPinYin(currentValue);

			AUCommon.DoDbAction(() =>
			{
				if (strPinYin.Count > 0)
				{
					result = strPinYin[0];
					if (CodeNameUniqueValidatorFacade.Validate(result, id, schemaType, parentID, includingDeleted == false, false, DateTime.MinValue) == false)
						result = GetCodeName(schemaType, id, parentID, result, includingDeleted, 1);
				}
			});

			return result;
		}

		[WebMethod]
		public string GetSchemaCategoryName(string key)
		{
			var cate = AU.Adapters.SchemaCategoryAdapter.Instance.LoadByID(key);
			if (cate != null && cate.Status == MCS.Library.SOA.DataObjects.Schemas.SchemaProperties.SchemaObjectStatus.Normal)
			{
				return cate.Name; ;
			}
			else
			{
				return "<无效的分类>";
			}
		}

		[WebMethod]
		public AUAdminScopeTypeCollection GetScopeTypes()
		{
			return AUCommon.GetAdminScopeTypes();
		}

		private static string GetCodeName(string schemaType, string id, string parentID, string currentValue, bool includingDeleted, int index)
		{
			string result = string.Format("{0}{1}", currentValue, index);

			if (CodeNameUniqueValidatorFacade.Validate(result, id, schemaType, parentID, includingDeleted == false, false, DateTime.MinValue) == false)
			{
				index++;
				result = GetCodeName(schemaType, id, parentID, currentValue, includingDeleted, index);
			}

			return result;
		}
	}
}
