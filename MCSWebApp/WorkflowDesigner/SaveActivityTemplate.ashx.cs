using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using MCS.Library.SOA.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;
using MCS.Web.Library.Script;
using MCS.Library.Data.Builder;
using MCS.Library.Principal;

namespace WorkflowDesigner
{
	/// <summary>
	/// Summary description for ActivityTemplate
	/// </summary>
	public class SaveActivityTemplate : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			string action = context.Request["action"];

			try
			{
				if (action == "insert")
				{
					DoInsert(context);
				}

				if (action == "delete")
				{
					DoDelete(context);
				}
			}
			catch (Exception ex)
			{
				OutputException(context, ex);
			}
		}

		private static void DoDelete(HttpContext context)
		{
			var idArr = context.Request["idstr"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (idArr.Length == 0) return;

			OutputResult(context, true, WfActivityTemplateAdpter.Instance.DeleteTemplates(idArr).ToString());
		}

		private static void OutputResult(HttpContext context, bool isSuccess, string message)
		{
			string rtnString = JSONSerializerExecute.Serialize(new
			{
				Success = isSuccess,
				Message = message
			});

			context.Response.Write(rtnString);
		}

		private static void OutputException(HttpContext context, System.Exception ex)
		{
			string rtnString = JSONSerializerExecute.Serialize(new
			{
				Success = false,
				Message = ex.Message + "\n" + ex.StackTrace
			});

			context.Response.Write(rtnString);
		}

		private static void DoInsert(HttpContext context)
		{
			string id = context.Request["id"];
			string category = context.Request["category"];
			string content = context.Request["content"];

			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(content)) return;

			WfConverterHelper.RegisterConverters();
			WfActivityDescriptor activityDesc = JSONSerializerExecute.Deserialize<WfActivityDescriptor>(content);

			activityDesc.Key = id;
			activityDesc.FromTransitions.Clear();
			activityDesc.ToTransitions.Clear();

			WfActivityTemplate template = new WfActivityTemplate()
			{
				ID = id,
				Name = activityDesc.Name,
				Category = activityDesc.ActivityType.ToString(),
				CreateTime = DateTime.Now,
				Content = JSONSerializerExecute.Serialize(activityDesc),
				Available = true,
				CreatorID = DeluxeIdentity.CurrentUser.ID,
				CreatorName = DeluxeIdentity.CurrentUser.Name
			};

			WfActivityTemplateAdpter.Instance.Update(template);

			OutputResult(context, true, activityDesc.Key);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}