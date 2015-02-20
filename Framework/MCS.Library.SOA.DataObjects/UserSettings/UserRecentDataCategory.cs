using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Builder;
using MCS.Library.Data;
using MCS.Library.Caching;
using System.Transactions;

namespace MCS.Library.SOA.DataObjects
{

    /// <summary>
    /// 表示用户最近设置的类别
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class UserRecentDataCategory : IXElementSerializable
    {
        internal static readonly XNamespace ns = "http://dataobjects.soa.library.mcs/userRecentData";

        private UserRecentDataCategoryItemCollection _list = null; //表示数据列表
        private PropertyDefineCollection _definedProperties; // 表示数据定义
        private int _defaultSize; //默认显示的最近项的个数

        internal UserRecentDataCategory()
        {
            this._definedProperties = new PropertyDefineCollection();
        }

        internal UserRecentDataCategory(UserRecentDataCategoryConfigurationElement elem)
        {
            InitFromConfiguration(elem);
        }

        /// <summary>
        /// 获取列表显示的最近项目的大小
        /// </summary>
        public int DefaultSize
        {
            get { return _defaultSize; }
        }

        /// <summary>
        /// 类别的名称
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// 类别的描述
        /// </summary>
        public string Description
        {
            get;
            internal set;
        }

        /// <summary>
        /// 类别所对应的属性
        /// </summary>
        public UserRecentDataCategoryItemCollection Items
        {
            get
            {
                if (this._list == null)
                    this._list = new UserRecentDataCategoryItemCollection(this);

                return this._list;
            }
        }

        /// <summary>
        /// 获取属性定义的元数据的集合
        /// </summary>
        internal PropertyDefineCollection MetaData
        {
            get { return _definedProperties; }
        }

        /// <summary>
        /// 从配置信息初始化
        /// </summary>
        /// <param name="categoryName"></param>
        internal void InitFromConfiguration(UserRecentDataCategoryConfigurationElement elem)
        {
            this.Name = elem.Name;
            this.Description = elem.Description;

            this._list = new UserRecentDataCategoryItemCollection(this);

            this._definedProperties = new PropertyDefineCollection();
            this._definedProperties.LoadPropertiesFromConfiguration(elem);
            this._defaultSize = elem.DefaultRecentCount;
        }

        /// <summary>
        /// 导入分类的数据
        /// </summary>
        /// <param name="srcCategory"></param>
        /// <remarks>没有定义的或者定义不一致的被忽略,将不会清空现有数据。数据将被附加到集合的末尾</remarks>
        internal void ImportValues(UserRecentDataCategory srcCategory)
        {
            if (this._definedProperties != null)
            {
                foreach (PropertyValueCollection item in srcCategory.Items)
                {
                    var newitem = this.Items.CreateItem();

                    foreach (var define in _definedProperties)
                    {
                        newitem[define.Name].StringValue = item[define.Name].StringValue;
                    }

                    this.Items.Add(newitem);
                }
            }
        }

        void IXElementSerializable.Deserialize(System.Xml.Linq.XElement node, XmlDeserializeContext context)
        {
            this.Name = node.Attribute(ns + "name", string.Empty);
            this.Description = node.Attribute(ns + "desc", string.Empty);
            this.InitFromConfiguration(UserRecentDataConfigurationSection.GetConfig().Categories[this.Name]);
            if (node.HasElements)
            {
                var items = node.Element(ns + "items");
                if (items != null)
                {
                    foreach (var item in items.Elements(ns + "item"))
                    {
                        PropertyValueCollection value = new PropertyValueCollection();
                        value.Deserialize(item, context);
                        this.Items.Add(value);
                    }
                }
            }
        }

        void IXElementSerializable.Serialize(System.Xml.Linq.XElement node, XmlSerializeContext context)
        {
            XElement items = new XElement(ns + "items");
            node.SetAttributeValue(ns + "name", Name);
            node.SetAttributeValue(ns + "desc", Description);

            foreach (PropertyValueCollection item in Items)
            {
                XElement subitem = new XElement(ns + "item");
                item.Serialize(subitem, context);
                items.Add(subitem);
            }

            node.Add(items);
        }


        public void SaveChanges(string userId)
        {
            XElementFormatter formatter = new XElementFormatter();

            formatter.OutputShortType = false;

            string settings = formatter.Serialize(this).ToString();

            string updateSQL = string.Format("UPDATE USER_RECENT_DATA SET DATA = {0} WHERE USER_ID = {1} AND CATEGORY = {2}",
                TSqlBuilder.Instance.CheckUnicodeQuotationMark(settings),
                TSqlBuilder.Instance.CheckUnicodeQuotationMark(userId),
                TSqlBuilder.Instance.CheckUnicodeQuotationMark(this.Name));

            using (DbContext context = DbContext.GetContext(ConnectionDefine.UserRelativeInfoConnectionName))
            {
                using (TransactionScope scope = TransactionScopeFactory.Create())
                {
                    if (DbHelper.RunSql(updateSQL, ConnectionDefine.UserRelativeInfoConnectionName) == 0)
                    {
                        string insertSQL = string.Format("INSERT INTO USER_RECENT_DATA(USER_ID,CATEGORY, DATA) VALUES({0}, {1}, {2})",
                            TSqlBuilder.Instance.CheckUnicodeQuotationMark(userId),
                            TSqlBuilder.Instance.CheckUnicodeQuotationMark(this.Name),
                            TSqlBuilder.Instance.CheckUnicodeQuotationMark(settings));

                        DbHelper.RunSql(insertSQL, ConnectionDefine.UserRelativeInfoConnectionName);
                    }

                    scope.Complete();
                }
            }

            CacheNotifyData notifyData = new CacheNotifyData(typeof(UserRecentDataCategoryCache), new UserRecentDataCategoryCacheKey(userId, this.Name), CacheNotifyType.Invalid);
            UdpCacheNotifier.Instance.SendNotifyAsync(notifyData);
            MmfCacheNotifier.Instance.SendNotify(notifyData);

            //可有可无，在NLB环境下意义不大。沈峥
            UserRecentDataCategoryCache.Instance.Remove(new UserRecentDataCategoryCacheKey(userId, this.Name));
        }
    }

    /// <summary>
    /// 用户最近数据的类别集合
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class UserRecentDataCategoryCollection : SerializableEditableKeyedDataObjectCollectionBase<string, UserRecentDataCategory>
    {
        /// <summary>
        /// 获取Key
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(UserRecentDataCategory item)
        {
            return item.Name;
        }
    }

    [Serializable]
    [XElementSerializable]
    public class UserRecentDataCategoryItemCollection : LruDataObjectCollectionBase<PropertyValueCollection>//EditableDataObjectCollectionBase<PropertyValueCollection>
    {
        private UserRecentDataCategory owner;

        public UserRecentDataCategoryItemCollection(UserRecentDataCategory owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// 获取指定下标处的<see cref="PropertyValueCollection"/>
        /// </summary>
        /// <param name="index">表示要获取的<see cref="PropertyValueCollection"/>在集合中的下标</param>
        /// <returns><see cref="PropertyValueCollection"/></returns>
        public override PropertyValueCollection this[int index]
        {
            get { return (PropertyValueCollection)List[index]; }
        }


        /// <summary>
        /// 在<see cref="UserRecentDataCategory"/>的指定位置插入项。
        /// </summary>
        /// <param name="index">从0开始的索引，将从此位置插入项。</param>
        /// <param name="item">要添加的<see cref="PropertyValueCollection"/>。</param>
        public void Insert(int index, PropertyValueCollection item)
        {
            List.Insert(index, item);
        }

        public void Remove(PropertyValueCollection item)
        {
            List.Remove(item);
        }

        /// <summary>
        /// 进行插入时，自动修正数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void OnInsert(int index, object value)
        {
            if (value is PropertyValueCollection)
            {
                var values = (PropertyValueCollection)value;

                var defines = owner.MetaData;
                if (defines != null)
                {
                    foreach (var def in defines)
                    {
                        if (values.ContainsKey(def.Name))
                        {
                            if (values[def.Name].Definition.DataType != def.DataType)
                                throw new ArgumentException(string.Format("试图向{0}添加属性值，但属性值的{1}的类型与定义不一致,", this.GetType().ToString(), def.Name));
                        }
                        else
                        {
                            values.Add(new PropertyValue(def));
                        }
                    }
                }
            }
            else
                throw new ArgumentException("参数必须是PropertyValueCollection类型", "value");

            base.OnInsert(index, value);
        }


        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);
            if (this.owner != null && owner.DefaultSize > 0 && this.Count > owner.DefaultSize)
            {
                var removeCount = this.Count - owner.DefaultSize;
                while (removeCount > 0)
                {
                    this.InnerList.RemoveAt(owner.DefaultSize);
                    removeCount--;
                }
            }
        }
        ///// <summary>
        ///// 将列表中由index指定的元素提升到列表的开头，其余元素顺次下移
        ///// </summary>
        ///// <param name="index"></param>
        //public void Advance(int index)
        //{
        //    if (index < 0 || index >= this.Count)
        //    {
        //        throw new ArgumentOutOfRangeException("index");
        //    }
        //    if (index == this.Count - 1 && index >= 0)
        //    {
        //        var obj = InnerList[index];

        //        InnerList.RemoveAt(index);
        //        InnerList.Insert(0, obj);
        //    }
        //    else if (index < this.Count - 1)
        //    {
        //        var obj = InnerList[index];
        //        for (int i = index - 1; i >= 0; i--)
        //        {
        //            InnerList[i + 1] = InnerList[i];
        //        }
        //        InnerList[0] = obj;
        //    }
        //}

        /// <summary>
        /// 基于现有配置创建一个新的<see cref="PropertyValueCollection"/>
        /// </summary>
        /// <returns>返回此Item</returns>
        public PropertyValueCollection CreateItem()
        {
            PropertyDefineCollection defines = null;
            if (this.owner == null || (defines = this.owner.MetaData) == null)
                throw new InvalidOperationException("未找到分类的属性定义");

            PropertyValueCollection collection = new PropertyValueCollection();

            foreach (var item in defines)
            {
                collection.Add(new PropertyValue(item));
            }

            return collection;
        }
    }
}
