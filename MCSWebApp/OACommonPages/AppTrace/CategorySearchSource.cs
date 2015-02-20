using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Library.Data;
using System.Data;
using MCS.Library.Data.Builder;

namespace MCS.OA.CommonPages.AppTrace
{
	public static class CategorySearchSource
	{
		public static IEnumerable<WfApplication> QueryAvaliableApplicationNames()
		{
			var result = WfApplicationAdapter.Instance.LoadAll();
			yield return new WfApplication() { CodeName = "", Name = "(全部)", Sort = -999 };
			//result.Sort((a, b) => a.Sort.CompareTo(b.Sort));

			foreach (var item in result)
			{
				yield return item;

			}
		}
		public static IEnumerable<WfProgram> QueryAvaliableProgramNames(string appName)
		{
			var result = string.IsNullOrEmpty(appName) ? new WfProgramInApplicationCollection() : WfApplicationAdapter.Instance.LoadProgramsByApplication(appName);
			yield return new WfProgram() { ApplicationCodeName = appName, CodeName = "", Name = "(全部)", Sort = -999 };

			foreach (var item in result)
			{
				yield return item;
			}
		}

		private static string GetConnectionName()
		{
			return WorkflowSettings.GetConfig().ConnectionName;
		}
	}
}