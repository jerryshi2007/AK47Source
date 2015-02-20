using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.ServiceModel;
using MCS.Library.SOA.DocServiceContract;
using MCS.Library.Services.Test;
using System.Configuration;

namespace MCS.Library.SOA.DocServiceClient
{
	public class DCSClient
	{
		public static readonly string DefaultDocumentLibraryName = "DocumentCenter";

		private string serviceAddress;

		private DCSClient(string serviceAddress, string documentLibraryName)
		{
			this.serviceAddress = serviceAddress;
			this.BindingInit();
			userBehavior = new UserBehavior(documentLibraryName, "documentServer");
		}

		private DCSClient(string serviceAddress, string documentLibraryName, string documentServerName)
		{
			this.serviceAddress = serviceAddress;
			this.BindingInit();
			userBehavior = new UserBehavior(documentLibraryName, documentServerName);
		}

		private void BindingInit()
		{
			binding = new BasicHttpBinding();
			binding.MaxReceivedMessageSize = int.MaxValue;
			binding.MessageEncoding = WSMessageEncoding.Mtom;
			binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
			binding.ReaderQuotas.MaxArrayLength = int.MaxValue;

			storageEndpointAddress = new EndpointAddress(UriHelper.CombinePath(serviceAddress, "DCSStorageService.svc"));
			wordBuilderEndpointAddress = new EndpointAddress(UriHelper.CombinePath(serviceAddress, "DCSDocumentBuilderService.svc"));
			searchEndpointAddress = new EndpointAddress(UriHelper.CombinePath(serviceAddress, "DCSFetchService.svc"));
			documentAnalyzeEndpointAddress = new EndpointAddress(UriHelper.CombinePath(serviceAddress, "DCSDocumentAnalyzeService.svc"));
		}

		private static ClientInfoConfigurationElement GetConfiguration()
		{
			DocClientConfigSettings section =
				DocClientConfigSettings.GetConfig();

			return section.Servers["documentServer"];
		}

#if DEBUG
		public static DCSClient Create()
		{
			return new DCSClient(ConfigurationManager.AppSettings["DCSServerAddress"], DefaultDocumentLibraryName);
		}

		public static DCSClient Create(string documentLibraryName)
		{
			return new DCSClient(ConfigurationManager.AppSettings["DCSServerAddress"], documentLibraryName);
		}

		public static DCSClient Create(string documentLibraryName, string documentServerName)
		{
			return new DCSClient(ConfigurationManager.AppSettings["DCSServerAddress"], documentLibraryName, documentServerName);
		}
#else
		public static DCSClient Create()
		{
			return new DCSClient(GetConfiguration().ServiceUrl, DefaultDocumentLibraryName);
		}

		public static DCSClient Create(string documentLibraryName)
		{
			return new DCSClient(GetConfiguration().ServiceUrl, documentLibraryName);
		}

		public static DCSClient Create(string documentLibraryName, string documentServerName)
		{
			return new DCSClient(GetConfiguration().ServiceUrl, documentLibraryName, documentServerName);
		}
#endif

		private BasicHttpBinding binding;

		public BasicHttpBinding Binding
		{
			get { return binding; }
			set { binding = value; }
		}

		private EndpointAddress storageEndpointAddress;

		public EndpointAddress StorageEndpointAddress
		{
			get { return storageEndpointAddress; }
			set { storageEndpointAddress = value; }
		}

		private EndpointAddress searchEndpointAddress;

		public EndpointAddress SearchEndpointAddress
		{
			get { return searchEndpointAddress; }
			set { searchEndpointAddress = value; }
		}

		private EndpointAddress documentAnalyzeEndpointAddress;

		public EndpointAddress DocumentAnalyzeEndpointAddress
		{
			get { return documentAnalyzeEndpointAddress; }
			set { documentAnalyzeEndpointAddress = value; }
		}

		private EndpointAddress wordBuilderEndpointAddress;

		public EndpointAddress WordBuilderEndpointAddress
		{
			get { return wordBuilderEndpointAddress; }
			set { wordBuilderEndpointAddress = value; }
		}

		private UserBehavior userBehavior;

		public UserBehavior UserBehavior
		{
			get { return userBehavior; }
			set { userBehavior = value; }
		}

		private DCTClientFolder rootFolder;

		/// <summary>
		/// 根文件夹
		/// </summary>
		public DCTClientFolder RootFolder
		{
			get
			{
				if (null == rootFolder)
				{
					ServiceProxy.SingleCall<IDCSStorageService>(binding, storageEndpointAddress, userBehavior, action =>
						{
							DCTFolder folder = action.DCMGetRootFolder();
							rootFolder = folder.To<DCTClientFolder>();
							rootFolder.Client = this;

						});
				}
				return rootFolder;
			}
			set { rootFolder = value; }
		}

		/// <summary>
		/// 打开文件
		/// </summary>
		/// <param name="id">文件id</param>
		/// <returns></returns>
		public byte[] OpenFile(int id)
		{
			byte[] results = null;
			ServiceProxy.SingleCall<IDCSStorageService>(binding, storageEndpointAddress, userBehavior, action =>
			{
				results = action.DCMGetFileContent(id);
			});
			return results;
		}

		/// <summary>
		/// 获取文件信息
		/// </summary>
		/// <param name="id">文件id</param>
		/// <returns></returns>
		public DCTClientFile GetFile(int id)
		{
			DCTClientFile result = null;
			ServiceProxy.SingleCall<IDCSStorageService>(binding, storageEndpointAddress, userBehavior, action =>
			{
				result = action.DCMGetFileById(id).To<DCTClientFile>();

			});
			return result;
		}

		public DCTClientFile GetFile(string uri)
		{
			DCTClientFile result = null;
			ServiceProxy.SingleCall<IDCSStorageService>(binding, storageEndpointAddress, userBehavior, action =>
			{
				result = action.DCMGetFileByUri(uri).To<DCTClientFile>();
			});
			return result;
		}

		/// <summary>
		/// 获取文件夹
		/// </summary>
		/// <param name="id">文件夹id</param>
		/// <returns></returns>
		public DCTClientFolder GetFolder(int id)
		{
			DCTClientFolder result = null;
			ServiceProxy.SingleCall<IDCSStorageService>(binding, storageEndpointAddress, userBehavior, action =>
			{
				result = action.DCMGetFolderById(id).To<DCTClientFolder>();

			});
			return result;
		}

		/// <summary>
		/// 获取文件夹
		/// </summary>
		/// <param name="folderUri">文件夹uri</param>
		/// <returns></returns>
		public DCTClientFolder GetFolder(string folderUri)
		{
			DCTClientFolder result = null;
			ServiceProxy.SingleCall<IDCSStorageService>(binding, storageEndpointAddress, userBehavior, action =>
			{
				result = action.DCMGetFolderByUri(folderUri).To<DCTClientFolder>();
			});
			return result;
		}

		/// <summary>
		/// 添加字段
		/// </summary>
		/// <param name="fieldname">字段名称</param>
		/// <param name="defaultValue">默认值</param>
		/// <param name="required">是否必填</param>
		/// <param name="fieldType">字段类型</param>
		public void AddField(string fieldname, string defaultValue, bool required, DCTFieldType fieldType)
		{
			ServiceProxy.SingleCall<IDCSStorageService>(binding, storageEndpointAddress, userBehavior, action =>
			{
				action.DCMAddField(new DCTFieldInfo() { Title = fieldname, Required = required, ValueType = fieldType, DefaultValue = defaultValue }, false);

			});
		}

		/// <summary>
		/// 添加字段
		/// </summary>
		/// <param name="fieldname">字段名称</param>
		/// <param name="defaultValue">默认值</param>
		/// <param name="required">是否必填</param>
		/// <param name="fieldType">字段类型</param>
		/// <param name="validationFormula">验证公式</param>
		/// <param name="validationMessage">验证信息</param>
		public void AddField(string fieldname, string defaultValue, bool required, DCTFieldType fieldType, string validationFormula, string validationMessage)
		{
			ServiceProxy.SingleCall<IDCSStorageService>(binding, storageEndpointAddress, userBehavior, action =>
			{
				action.DCMAddField(new DCTFieldInfo() { Title = fieldname, Required = required, ValueType = fieldType, DefaultValue = defaultValue, ValidationFormula = validationFormula, ValidationMessage = validationMessage }, false);

			});
		}

		private DCTFieldInfoDictionary fields;

		public DCTFieldInfoDictionary Fields
		{
			get
			{
				if (null == fields)
				{
					fields = new DCTFieldInfoDictionary();
					ServiceProxy.SingleCall<IDCSStorageService>(binding, storageEndpointAddress, userBehavior, action =>
					{
						var serverfields = action.DCMGetFields();
						foreach (var field in serverfields)
						{
							if (fields.ContainsKey(field.Title))
								continue;
							fields.Add(field);
						}

					});
				}
				return fields;
			}

		}

		/// <summary>
		/// 搜索
		/// </summary>
		/// <param name="keywords">关键字</param>
		/// <returns></returns>
		public BaseCollection<DCTClientSearchResult> Search(params string[] keywords)
		{
			BaseCollection<DCTClientSearchResult> results = new BaseCollection<DCTClientSearchResult>();
			ServiceProxy.SingleCall<IDCSFetchService>(binding, searchEndpointAddress, userBehavior, action =>
			{
				var allDocs = action.DCMSearchDoc(keywords);
				foreach (var doc in allDocs)
				{
					DCTClientSearchResult result = doc.To<DCTClientSearchResult>();
					result.Client = this;
					results.Add(result);
				}
			});
			return results;
		}

		/// <summary>
		/// 查询文件
		/// </summary>
		/// <param name="keyValuePairs">键值对</param>
		/// <returns></returns>
		public BaseCollection<DCTClientFile> QueryFile(Dictionary<string, string> keyValuePairs)
		{
			BaseCollection<DCTFileField> fileFields = new BaseCollection<DCTFileField>();

			foreach (var pair in keyValuePairs)
			{
				var curField = Fields;
				if (!curField.ContainsKey(pair.Key))
					continue;
				fileFields.Add(new DCTFileField() { Field = Fields[pair.Key], FieldValue = pair.Value });

			}

			BaseCollection<DCTFile> files = new BaseCollection<DCTFile>();
			BaseCollection<DCTClientFile> results = new BaseCollection<DCTClientFile>();
			ServiceProxy.SingleCall<IDCSFetchService>(binding, searchEndpointAddress, userBehavior, action =>
			{
				files = action.DCMQueryDocByField(fileFields);
				foreach (var file in files)
				{
					results.Add(file.To<DCTClientFile>());
				}
			});

			return results;
		}

		public DCTWordDataObject AnalyzeDocument(byte[] wordFileContent)
		{
			DCTWordDataObject result = null;
			ServiceProxy.SingleCall<IDCSDocumentAnalyzeService>(binding, documentAnalyzeEndpointAddress, userBehavior, action =>
			{
				result = action.DCMAnalyze(wordFileContent);
			});
			return result;
		}
	}
}
