using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MCS.Library.SOA.DocServiceContract;
using Microsoft.SharePoint.Client;
using MCS.Library.CamlBuilder;
using MCS.Library.Services.Converters;
using System.ServiceModel.Activation;
using MCS.Library.Services.MossQuery;
using System.Net;
using System.Data;
using System.Web.Services.Protocols;
using MCS.Library.SOA.DocServiceContract.Exceptions;


namespace MCS.Library.Services
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class DCSFetchService : IDCSFetchService
	{
		public BaseCollection<DCTFile> DCMQueryDocByField(BaseCollection<DCTFileField> fields)
		{
			BaseCollection<DCTFile> files = new BaseCollection<DCTFile>();
			using (DocLibContext clientContext = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				CamlQuery query = new CamlQuery();
				CamlExpression caml = null;
				foreach (DCTFileField item in fields)
				{
					string valueText = string.Empty;
					switch (item.Field.ValueType)
					{
						case DCTFieldType.Boolean:
							break;
						case DCTFieldType.DateTime:
							break;
						case DCTFieldType.Decimal:
							break;
						case DCTFieldType.Integer:
							valueText = "Counter";
							break;
						case DCTFieldType.Note:
							break;
						case DCTFieldType.Text:
							valueText = "Text";
							break;
						case DCTFieldType.User:
							break;
						default:
							break;
					}
					if (caml == null)
					{
						caml = Caml.Field(item.Field.InternalName).Eq(Caml.Value(valueText, item.FieldValue));
					}
					else
					{
						caml = caml.And(Caml.Field(item.Field.InternalName).Eq(Caml.Value(valueText, item.FieldValue)));
					}
				}

				query.ViewXml = Caml.SimpleView(caml, CamlBuilder.ViewType.RecursiveAll).ToCamlString();

				ListItemCollection items = clientContext.BaseList.GetItems(query);
				clientContext.Load(items);

				clientContext.ExecuteQuery();

				foreach (ListItem item in items)
				{
					DCTFile file = new DCTFile();
					DCTConverterHelper.Convert(item, file);
					files.Add(file);
				}
			}
			return files;
		}

		public BaseCollection<DCTFile> DCMQueryDocByCaml(string camlText)
		{
			BaseCollection<DCTFile> files = new BaseCollection<DCTFile>();

			using (DocLibContext clientContext = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
			{
				CamlQuery query = new CamlQuery();
				query.ViewXml = camlText;
				ListItemCollection items = clientContext.BaseList.GetItems(query);
				clientContext.Load(items);
				clientContext.ExecuteQuery();
				foreach (ListItem item in items)
				{
					DCTFile file = new DCTFile();
					DCTConverterHelper.Convert(item, file);
					files.Add(file);
				}
			}
			return files;
		}

		public BaseCollection<DCTSearchResult> DCMSearchDoc(string[] keyWords)
		{
			MCS.Library.Core.ServerInfo serverInfo = MossServerInfoConfigurationSettings.GetConfig().Servers["documentServer"].ToServerInfo();
			System.Net.NetworkCredential Credentials = new System.Net.NetworkCredential(serverInfo.Identity.LogOnName, serverInfo.Identity.Password, serverInfo.Identity.Domain);
			string searchServiceUrl = MCS.Library.Core.UriHelper.CombinePath(MossServerInfoConfigurationSettings.GetConfig().Servers["documentServer"].ServerName, MossServerInfoConfigurationSettings.GetConfig().Servers["documentServer"].MossSearchServiceUrl);
			QueryService queryService = new QueryService(searchServiceUrl, Credentials);

			BaseCollection<DCTSearchResult> result = new BaseCollection<DCTSearchResult>();
			StringBuilder queryXml = new StringBuilder();
			queryXml.Append("<QueryPacket xmlns=\"urn:Microsoft.Search.Query\" Revision=\"1000\">");
			queryXml.Append("<Query domain=\"QDomain\">");
			queryXml.Append("<SupportedFormats>");
			queryXml.Append("<Format>");
			queryXml.Append("urn:Microsoft.Search.Response.Document.Document");
			queryXml.Append("</Format>");
			queryXml.Append("</SupportedFormats>");
			queryXml.Append("<Range>");
			queryXml.Append("<Count>50</Count>");
			queryXml.Append("</Range>");
			queryXml.Append("<Context>");
			queryXml.Append("<QueryText language=\"en-US\" type=\"STRING\">");
			foreach (string item in keyWords)
			{
				queryXml.Append(string.Format("{0} ", item));
			}

			queryXml.Append("</QueryText>");
			queryXml.Append("</Context>");
			queryXml.Append("</Query>");

			queryXml.Append("</QueryPacket>");
			try
			{
				using (DocLibContext context = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
				{
					DataSet dsQueryResult = queryService.QueryEx(queryXml.ToString());
					IEnumerable<DataRow> drQueryResult = from row in dsQueryResult.Tables[0].AsEnumerable()
														 where row.Field<bool>("IsDocument") == true
														 select row;
					foreach (var item in drQueryResult)
					{
						DCTSearchResult searchResult = new DCTSearchResult();
						searchResult.Title = item["Title"].ToString();
						searchResult.Size = int.Parse(item["Size"].ToString());
						searchResult.HitHighlightedSummary = item["HitHighlightedSummary"].ToString();
						searchResult.LastModifiedDate = DateTime.Parse(item["Write"].ToString());
						searchResult.Path = item["Path"].ToString().ToLower().Replace(context.Url.ToLower(), "");
						result.Add(searchResult);
					}
					return result;
				}
			}
			catch (SoapException ex)
			{
				throw ex;
			}
		}
	}
}
