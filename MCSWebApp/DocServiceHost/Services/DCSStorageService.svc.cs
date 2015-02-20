using System;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using System.Configuration;
using MCS.Library.SOA.DocServiceContract;
using MCS.Library.SOA.DocServiceContract.Exceptions;
using Microsoft.SharePoint.Client;
using MCS.Library.Configuration;
using MCS.Library.Core;
using System.ServiceModel.Activation;
using MCS.Library.Services.Converters;
using MCS.Library.CamlBuilder;

namespace MCS.Library.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class DCSStorageService : IDCSStorageService
    {
        public DCTFolder DCMGetRootFolder()
        {
            using (DocLibContext clientContext = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                Folder rootFolder = clientContext.BaseList.RootFolder;

                clientContext.Load(rootFolder);
                clientContext.ExecuteQuery();

                DCTFolder result = new DCTFolder() { ID = 0, IsRootFolder = true };

                DCTConverterHelper.Convert(rootFolder, result);

                return result;
            }
        }

        public DCTFolder DCMGetFolderById(int id)
        {
            using (DocLibContext clientContext = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                if (id == 0)
                    return DCMGetRootFolder();
                ListItem item = GetListItemById(id, clientContext);

                (item == null).TrueThrow<TargetNotFoundException>("无法根据ID:{0}找到对应的目录。", id);
                DCTFolder folder = new DCTFolder();
                DCTConverterHelper.Convert(item, folder);

                return folder;
            }
        }

        private static ListItem GetListItemById(int id, DocLibContext clientContext)
        {
            List list = clientContext.BaseList;
            CamlQuery query = new CamlQuery();
            Caml caml = Caml.SimpleView(Caml.Field("ID").Eq(Caml.Value("Counter", id.ToString())), CamlBuilder.ViewType.RecursiveAll);
            query.ViewXml = caml.ToCamlString();
            ListItemCollection items = list.GetItems(query);

            clientContext.Load(items);
            clientContext.ExecuteQuery();

            if (items.Count == 0)
                return null;

            return items[0];
        }

        public DCTFolder DCMGetFolderByUri(string uri)
        {
            using (DocLibContext clientContext = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                Folder folder = clientContext.Web.GetFolderByServerRelativeUrl(uri);
                clientContext.Load(folder);

                CamlQuery query = new CamlQuery();
                query.ViewXml = Caml.SimpleView(Caml.Field("FileRef").Eq(Caml.Value("Text", uri)), CamlBuilder.ViewType.RecursiveAll).ToCamlString();

                ListItemCollection items = clientContext.BaseList.GetItems(query);

                clientContext.Load(items);

                DCTFolder result;
                try
                {
                    clientContext.ExecuteQuery();
                    if (string.Compare(folder.ServerRelativeUrl, this.DCMGetRootFolder().Uri, true) == 0)
                        result = new DCTFolder() { ID = 0, IsRootFolder = true };
                    else
                        result = new DCTFolder() { ID = items[0].Id, IsRootFolder = false };

                    DCTConverterHelper.Convert(folder, result);
                }
                catch
                {

                    result = null;
                }

                return result;
            }
        }

        public DCTFolder DCMGetParentFolder(DCTStorageObject storageObject)
        {
            using (DocLibContext clientContext = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                ListItem listItem = LoadItem(storageObject, clientContext);
                (listItem == null).TrueThrow<ArgumentException>("无法找到文件夹,传入的为根目录或文件、文件夹不存在。");

                DCTFolder folder = null;

                clientContext.Load(clientContext.BaseList.RootFolder);

                clientContext.ExecuteQuery();

                if (string.Compare(listItem.FieldValues["FileDirRef"].ToString(), clientContext.BaseList.RootFolder.ServerRelativeUrl, true) == 0)
                {
                    folder = new DCTFolder() { ID = 0, IsRootFolder = true };

                    DCTConverterHelper.Convert(clientContext.BaseList.RootFolder, folder);
                }
                else
                {
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = Caml.SimpleView(Caml.Field("ServerUrl").Eq(Caml.Value("Text", listItem.FieldValues["FileDirRef"].ToString())), CamlBuilder.ViewType.RecursiveAll).ToCamlString();

                    ListItemCollection parentListItem = clientContext.BaseList.GetItems(query);
                    clientContext.Load(parentListItem);
                    clientContext.ExecuteQuery();

                    folder = new DCTFolder();

                    DCTConverterHelper.Convert(parentListItem[0], folder);
                }

                return folder;
            }
        }

        public BaseCollection<DCTStorageObject> DCMGetChildren(DCTFolder folder, DCTContentType contentType)
        {
            BaseCollection<DCTStorageObject> childs = new BaseCollection<DCTStorageObject>();
            BaseCollection<DCTFile> childFiles = new BaseCollection<DCTFile>();
            BaseCollection<DCTFolder> childFolders = new BaseCollection<DCTFolder>();

            using (DocLibContext clientContext = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                Folder clientOMFolder = clientContext.Web.GetFolderByServerRelativeUrl(folder.Uri);
                CamlQuery query = new CamlQuery();
                query.FolderServerRelativeUrl = LoadUri(folder, clientContext);
                query.ViewXml = Caml.SimpleView(Caml.EmptyCaml(), CamlBuilder.ViewType.Default).ToCamlString();

                ListItemCollection items = clientContext.BaseList.GetItems(query);

                clientContext.Load(items);
                clientContext.ExecuteQuery();

                foreach (ListItem item in items)
                {
                    switch (item.FileSystemObjectType)
                    {
                        case FileSystemObjectType.File:
                            if ((contentType & DCTContentType.File) != DCTContentType.None)
                            {
                                DCTFile itemFile = new DCTFile();
                                DCTConverterHelper.Convert(item, itemFile);

                                childs.Add(itemFile);
                                //childFiles.Add(itemFile);
                            }
                            break;
                        case FileSystemObjectType.Folder:
                            if ((contentType & DCTContentType.Folder) != DCTContentType.None)
                            {
                                DCTFolder itemFolder = new DCTFolder();

                                DCTConverterHelper.Convert(item, itemFolder);

                                childs.Add(itemFolder);
                                //childFolders.Add(itemFolder);
                            }
                            break;
                        case FileSystemObjectType.Invalid:
                            break;
                        case FileSystemObjectType.Web:
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (DCTFolder item in childFolders)
                childs.Add(item);

            foreach (DCTFile item in childFiles)
                childs.Add(item);

            return childs;
        }

        public DCTFolder DCMCreateFolder(string name, DCTFolder parentFolder)
        {
            DCTFolder folder = new DCTFolder();
            using (DocLibContext clientContext = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                Folder cOMParentFolder = clientContext.Web.GetFolderByServerRelativeUrl(LoadUri(parentFolder, clientContext));
                if (cOMParentFolder != null)
                {

                    cOMParentFolder.Folders.Add(UriHelper.CombinePath(parentFolder.Uri, name));
                }

                clientContext.ExecuteQuery();

                CamlQuery query = new CamlQuery() { FolderServerRelativeUrl = parentFolder.Uri };
                query.ViewXml = Caml.SimpleView(Caml.Field("ServerUrl").Eq(Caml.Value("Text", (parentFolder.Uri[parentFolder.Uri.Length - 1] != '/' ? parentFolder.Uri + "/" : parentFolder.Uri) + name)), CamlBuilder.ViewType.Default).ToCamlString();
                ListItemCollection items = clientContext.BaseList.GetItems(query);
                clientContext.Load(items);

                clientContext.ExecuteQuery();

                if (items.Count != 0)
                {
                    folder.AbsoluteUri = UriHelper.CombinePath(parentFolder.AbsoluteUri, name);
                    folder.Name = name;
                    folder.Uri = UriHelper.CombinePath(parentFolder.Uri, name);
                    folder.IsRootFolder = false;
                    folder.ID = items[0].Id;
                }
            }

            return folder;
        }

        public void UpdateFolder(DCTFolder folder)
        {
            using (DocLibContext clientContext = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                ListItem item = LoadItem(folder, clientContext);

                (item == null).TrueThrow<TargetNotFoundException>("文件夹（ID={0})不存在。", folder.ID);

                item.FieldValues["Title"] = folder.Name;
                item.FieldValues["FileLeafRef"] = folder.Name;
                item.Update();

                clientContext.ExecuteQuery();
            }
        }

        public void DCMDelete(DCTStorageObject storageObject)
        {
            using (DocLibContext clientContext = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                string uri = LoadUri(storageObject, clientContext);
                if (storageObject is DCTFile)
                {
                    ClientObject clientObject = (File)clientContext.Web.GetFileByServerRelativeUrl(uri);
                    ((File)clientObject).DeleteObject();
                }
                else
                    if (storageObject is DCTFolder)
                    {
                        ClientObject clientObject = clientContext.Web.GetFolderByServerRelativeUrl(uri);
                        ((Folder)clientObject).DeleteObject();
                    }

                clientContext.ExecuteQuery();
            }
        }

        public void DCMRemove(DCTStorageObject storageObject)
        {
            using (DocLibContext clientContext = new DocLibContext(ServiceHelper.GetDocumentLibraryName()))
            {
                string uri = LoadUri(storageObject, clientContext);
                if (storageObject is DCTFile)
                {
                    ClientObject clientObject = (File)clientContext.Web.GetFileByServerRelativeUrl(uri);
                    ((File)clientObject).Recycle();

                }
                else
                    if (storageObject is DCTFolder)
                    {
                        ClientObject clientObject = clientContext.Web.GetFolderByServerRelativeUrl(uri);
                        ((Folder)clientObject).Recycle();
                    }

                clientContext.ExecuteQuery();
            }
          
        }
    }
}
