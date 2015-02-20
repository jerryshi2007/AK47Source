using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DocServiceContract;
using MCS.Library.Core;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.DocServiceClient
{
    public class DCTClientFile : DCTFile, IDCSClientStorageObject
    {
        public DCTClientFile()
        {
            effectedFileField = new List<DCTFileField>();
        }

        private DCSClient client;

        public DCSClient Client
        {
            get { return client; }
            set { client = value; }
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
                        parent = action.DCMGetParentFolder(this.To<DCTFile>()).To<DCTClientFolder>();
                        parent.Client = this.Client;
                    });
                return parent;
            }
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <returns></returns>
        public byte[] GetFileContent()
        {
            byte[] results = null;
            ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
            {
                results = action.DCMGetFileContent(this.ID);
            });
            return results;
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Delete()
        {
            ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
            {
                action.DCMDelete(this.To<DCTFile>());
            });
        }

        private List<DCTFileField> effectedFileField;

        public string this[string index]
        {
            get
            {
                return Fields[index].FieldValue;
            }
            set
            {
                if (null == fields)
                {
                    fields = new FileFieldCollection();
                }
                if (!fields.ContainsKey(index))
                {
                    ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
                    {
                        BaseCollection<DCTFileField> fileFields = action.DCMGetFileFields(this.ID, new BaseCollection<DCTFieldInfo>() { new DCTFieldInfo() { Title = index } });
                        foreach (DCTFileField fileField in fileFields)
                        {
                            fields.Add(fileField);
                        }
                    });
                }
                Fields[index].FieldValue = value;
                effectedFileField.Add(fields[index]);
            }
        }

        /// <summary>
        /// 更新字段
        /// </summary>
        public void Update()
        {
            ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
                    {
                        BaseCollection<DCTFileField> fileFields = new BaseCollection<DCTFileField>();
                        foreach (DCTFileField filefield in effectedFileField)
                        {
                            fileFields.Add(filefield);
                        }
                        action.DCMSetFileFields(this.ID, fileFields);
                    });
        }

        private FileFieldCollection fields;

        protected FileFieldCollection Fields
        {
            get
            {
                if (null == fields)
                {
                    fields = new FileFieldCollection();
                    ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
                    {
                        BaseCollection<DCTFileField> fileFields = action.DCMGetAllFileFields(this.ID);
                        foreach (DCTFileField fileField in fileFields)
                        {
                            fields.Add(fileField);
                        }
                    });

                }
                return fields;
            }

        }

        /// <summary>
        /// 作为模板
        /// </summary>
        /// <returns></returns>
        public WordTemplate AsTemplate()
        {
            WordTemplate template = new WordTemplate();
            template.Client = this.client;
            template.Uri = this.Uri;
            template.Client = this.client;
            return template;
        }


        /// <summary>
        /// 签入
        /// </summary>
        /// <param name="comment">备注</param>
        /// <param name="checkInType">签入类型</param>
        public void CheckIn(string comment, DCTCheckinType checkInType)
        {
            ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
                    {
                        action.DCMCheckIn(this.ID, comment, checkInType);
                    });
        }

        /// <summary>
        /// 签出
        /// </summary>
        public void CheckOut()
        {
            ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
            {
                action.DCMCheckOut(this.ID);
            });
        }

        /// <summary>
        /// 撤销签出
        /// </summary>
        public void UndoCheckOut()
        {
            ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
            {
                action.DCMUndoCheckOut(this.ID);
            });
        }

        private BaseCollection<DCTClientFileVersion> fileVersions;

        /// <summary>
        /// 版本
        /// </summary>
        public BaseCollection<DCTClientFileVersion> FileVersions
        {
            get
            {
                if (null == fileVersions)
                {
                    fileVersions = new BaseCollection<DCTClientFileVersion>();
                    ServiceProxy.SingleCall<IDCSStorageService>(client.Binding, client.StorageEndpointAddress, client.UserBehavior, action =>
                    {
                        var versions = action.DCMGetVersions(this.ID);
                        foreach (DCTFileVersion fileVersion in versions)
                        {
                            var newFileVersion = fileVersion.To<DCTClientFileVersion>();
                            newFileVersion.Client = this.client;
                            newFileVersion.ClientFile = this;
                            fileVersions.Add(newFileVersion);
                        }
                    });
                }
                return fileVersions;
            }
        }

    }
}
