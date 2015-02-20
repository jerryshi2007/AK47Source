using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DocServiceContract;
using MCS.Library.Core;

namespace MCS.Library.SOA.DocServiceClient
{
	public class DCTClientFolder : DCTFolder, IDCSClientStorageObject
	{
		private DCSClient client;

		public DCSClient Client
		{
			get { return client; }
			set { client = value; }
		}

		/// <summary>
		/// 文件
		/// </summary>
		public BaseCollection<DCTClientFile> Files
		{
			get
			{
				BaseCollection<DCTClientFile> results = new BaseCollection<DCTClientFile>();
				ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
				{

					BaseCollection<DCTStorageObject> storageObjects = action.DCMGetChildren(this.To<DCTFolder>(), DCTContentType.File);
					foreach (DCTStorageObject storageObject in storageObjects)
					{
						DCTClientFile result = null;
						result = storageObject.To<DCTClientFile>();
						result.Client = this.Client;
						results.Add(result);
					}
				});
				return results;
			}
		}

		/// <summary>
		/// 获取文件
		/// </summary>
		/// <param name="filename">文件名</param>
		/// <returns></returns>
		public DCTClientFile GetFile(string filename)
		{
			DCTClientFile result = null;
			ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
				{
					result = action.DCMGetFileInFolder(this.To<DCTFolder>(), filename).To<DCTClientFile>();
				});
			if (null != result)
				result.Client = this.client;
			return result;
		}

		/// <summary>
		/// 创建文件夹
		/// </summary>
		/// <param name="foldername">文件夹名称</param>
		/// <returns></returns>
		public DCTClientFolder CreateFolder(string foldername)
		{
			DCTClientFolder result = null;
			ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
			{
				result = action.DCMCreateFolder(foldername, this.To<DCTFolder>()).To<DCTClientFolder>();
			});
			return result;
		}

		/// <summary>
		/// 获取子文件夹
		/// </summary>
		/// <param name="foldername">文件夹名称</param>
		/// <returns></returns>
		public DCTClientFolder GetFolder(string foldername)
		{
			DCTClientFolder result = null;
			ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
			{
				var temp = action.DCMGetFolderByUri(UriHelper.CombinePath(this.Uri, foldername));
				result = temp == null ? null : temp.To<DCTClientFolder>();
			});
			return result;
		}

		public void Delete()
		{
			ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
				{
					action.DCMDelete(this.To<DCTFolder>());
				});
		}

		/// <summary>
		/// 子文件夹
		/// </summary>
		public BaseCollection<DCTClientFolder> SubFolders
		{
			get
			{
				BaseCollection<DCTClientFolder> results = new BaseCollection<DCTClientFolder>();
				ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
				{

					BaseCollection<DCTStorageObject> storageObjects = action.DCMGetChildren(this.To<DCTFolder>(), DCTContentType.Folder);
					foreach (DCTStorageObject storageObject in storageObjects)
					{
						DCTClientFolder result = null;
						result = storageObject.To<DCTClientFolder>();
						result.Client = this.Client;
						results.Add(result);
					}

				});
				return results;
			}
		}

		/// <summary>
		/// 子文件或文件夹
		/// </summary>
		public BaseCollection<IDCSClientStorageObject> Children
		{
			get
			{
				BaseCollection<IDCSClientStorageObject> results = new BaseCollection<IDCSClientStorageObject>();
				ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
				{

					BaseCollection<DCTStorageObject> storageObjects = action.DCMGetChildren(this.To<DCTFolder>(), DCTContentType.All);
					foreach (DCTStorageObject storageObject in storageObjects)
					{
						IDCSClientStorageObject result = null;
						if (storageObject is DCTFolder)
							result = storageObject.To<DCTClientFolder>();
						else
							result = storageObject.To<DCTClientFile>();
						result.Client = this.Client;
						results.Add(result);
					}

				});
				return results;
			}
		}

		private DCTClientFolder parent;

		/// <summary>
		/// 父文件夹
		/// </summary>
		public DCTClientFolder Parent
		{
			get
			{
				ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
				{
					parent = action.DCMGetParentFolder(this.To<DCTFolder>()).To<DCTClientFolder>();
					parent.Client = this.Client;
				});
				return parent;
			}
		}

		/// <summary>
		/// 保存
		/// </summary>
		/// <param name="content">内容</param>
		/// <param name="filename">文件名</param>
		/// <param name="overwrite">是否覆盖</param>
		public DCTClientFile Save(byte[] content, string filename, bool overwrite)
		{
			DCTClientFile result = null;
			ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
			{
				result = action.DCMSave(this.To<DCTFolder>(), content, filename, overwrite).To<DCTClientFile>();
			});
			return result;
		}
	}
}
