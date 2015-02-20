using System;
using System.Collections.Generic;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace PermissionCenter.Handlers
{
	/// <summary>
	/// TimelineData 的摘要说明
	/// </summary>
	public class TimelineData : IHttpHandler
	{
		private static readonly DateTime infinit = new DateTime(9999, 9, 9);

		public void ProcessRequest(HttpContext context)
		{
			HistoryAdapter adapter = new HistoryAdapter();

			string id = context.Request.QueryString["id"];
			if (string.IsNullOrEmpty(id))
				throw new HttpException("必须提供有效的ID参数");

			TimePointContext timeContext = TimePointContext.GetCurrentState();
			TimePointContext.Current.UseCurrentTime = true;

			SchemaObjectBase obj;
			try
			{
				obj = SchemaObjectAdapter.Instance.Load(id);
			}
			finally
			{
				TimePointContext.RestoreCurrentState(timeContext);
			}

			if (obj == null)
				throw new ObjectNotFoundException("不存在id为" + id + "的对象");

			context.Response.ContentType = "text/xml";
			context.Response.CacheControl = "no-cache";
			using (System.IO.Stream output = context.Response.OutputStream)
			{
				System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(output);
				writer.WriteStartElement("data");

				this.WriteObjectHistory(adapter.GetObjectHistoryEntries(id), writer, id);

				this.WriteReferenceHistory(adapter.GetReferenceHistoryEntries(id), writer, id);

				this.WriteMembershipHistory(adapter.GetMembershipHistoryEntries(id), writer, id);

				writer.WriteEndElement();

				writer.Close();
			}
		}

		private void WriteMembershipHistory(IEnumerable<MembershipHistoryEntry> objects, System.Xml.XmlWriter writer, string id)
		{
			foreach (var entry in objects)
			{
				this.BeginEvent(writer);

				WriteStartTime(writer, entry);

				this.WriteEndTime(writer, entry);

				this.WriteMembershipChangeTitle(writer, entry);

				this.WriteMembershipChangeContent(writer, entry, id);

				this.EndEvent(writer);
			}
		}

		private void WriteReferenceHistory(IEnumerable<ReferenceHistoryEntry> objects, System.Xml.XmlWriter writer, string id)
		{
			foreach (var entry in objects)
			{
				this.BeginEvent(writer);

				WriteStartTime(writer, entry);

				this.WriteEndTime(writer, entry);

				this.WriteReferenceChangeTitle(writer, entry);

				this.WriteReferenceChangeContent(writer, entry, id);

				this.EndEvent(writer);
			}
		}

		private void WriteObjectHistory(IEnumerable<HistoryEntry> objects, System.Xml.XmlWriter writer, string id)
		{
			foreach (var entry in objects)
			{
				this.BeginEvent(writer);

				WriteStartTime(writer, entry);

				this.WriteEndTime(writer, entry);

				this.WriteObjectChangeTitle(writer, entry);

				this.WriteObjectChangeContent(writer, entry, id);

				this.EndEvent(writer);
			}
		}

		private void WriteObjectChangeTitle(System.Xml.XmlWriter writer, HistoryEntry entry)
		{
			writer.WriteStartAttribute("title");
			if (entry.Status == SchemaObjectStatus.Normal)
				writer.WriteValue("属性编辑");
			else
			{
				writer.WriteValue("状态变更：" + entry.Status.ToString());
			}

			writer.WriteEndAttribute();
		}

		private void WriteReferenceChangeTitle(System.Xml.XmlWriter writer, ReferenceHistoryEntry entry)
		{
			writer.WriteStartAttribute("title");

			if (entry.Status == SchemaObjectStatus.Normal)
			{
				writer.WriteString("建立关系");
			}
			else
			{
				writer.WriteString("解除关系");
			}

			writer.WriteEndAttribute();
		}

		private void WriteMembershipChangeTitle(System.Xml.XmlWriter writer, MembershipHistoryEntry entry)
		{
			writer.WriteStartAttribute("title");

			if (entry.Status == SchemaObjectStatus.Normal)
			{
				writer.WriteString("建立成员关系 ");
			}
			else
			{
				writer.WriteString("解除成员关系");
			}

			writer.WriteEndAttribute();
		}

		private void WriteObjectChangeContent(System.Xml.XmlWriter writer, HistoryEntry entry, string id)
		{
			if (entry.Status == SchemaObjectStatus.Normal)
			{
				writer.WriteString(HttpUtility.HtmlEncode(entry.Name) + "(" + HttpUtility.HtmlEncode(entry.DisplayName) + ") 变更属性 <a href=\"javascript:void(0);\" onclick=\"$pc.popups.historyProperty('" + Util.HtmlAttributeEncode(id) + "','" + Util.HtmlAttributeEncode(entry.VersionStartTime.ToUniversalTime().ToString("O")) + "');\">点此查看</a>");
			}
			else
			{
				writer.WriteString(HttpUtility.HtmlEncode(entry.Name) + "(" + HttpUtility.HtmlEncode(entry.DisplayName) + ") 被删除");
			}
		}

		private void WriteReferenceChangeContent(System.Xml.XmlWriter writer, ReferenceHistoryEntry entry, string id)
		{
			if (entry.Status == SchemaObjectStatus.Normal)
			{
				writer.WriteString("与" + HttpUtility.HtmlEncode(entry.Name) + "(" + HttpUtility.HtmlEncode(entry.DisplayName) + ") 建立关系");
			}
			else
			{
				writer.WriteString("与" + HttpUtility.HtmlEncode(entry.Name) + "(" + HttpUtility.HtmlEncode(entry.DisplayName) + ") 解除关系");
			}
		}

		private void WriteMembershipChangeContent(System.Xml.XmlWriter writer, MembershipHistoryEntry entry, string id)
		{
			if (entry.Status == SchemaObjectStatus.Normal)
			{
				writer.WriteString("与" + HttpUtility.HtmlEncode(entry.Name) + "(" + HttpUtility.HtmlEncode(entry.DisplayName) + ") 建立成员关系");
			}
			else
			{
				writer.WriteString("与" + HttpUtility.HtmlEncode(entry.Name) + "(" + HttpUtility.HtmlEncode(entry.DisplayName) + ") 解除成员关系");
			}
		}

		private static void WriteStartTime(System.Xml.XmlWriter writer, HistoryEntry entry)
		{
			writer.WriteStartAttribute("start");
			writer.WriteValue(entry.VersionStartTime.ToUniversalTime().ToString("R"));
			writer.WriteEndAttribute();
		}

		private void WriteEndTime(System.Xml.XmlWriter writer, HistoryEntry entry)
		{
			if (entry.VersionEndTime != infinit)
			{
				writer.WriteStartAttribute("end");
				writer.WriteValue(entry.VersionEndTime.ToUniversalTime().ToString("R"));
				writer.WriteEndAttribute();

				writer.WriteAttributeString("isDuration", "true");
			}
		}

		private void EndEvent(System.Xml.XmlWriter writer)
		{
			writer.WriteEndElement();
		}

		private void BeginEvent(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("event");
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