#region
// -------------------------------------------------
// Assembly	：	
// FileName	：	ButtonItem.cs
// Remark	：  列表Collection类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		songhaojie	    20070706		创建
// -------------------------------------------------
#endregion
using System;
using System.Data;
using System.Text;
using System.Configuration;
using System.Drawing.Design;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.UI.Design.WebControls;
using System.Web.Script;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Web.Script.Serialization;

using MCS.Web.Library.Script;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 选择项
    /// </summary>
    /// <remarks>选择项</remarks>
    public class SelectItem
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>构造函数</remarks>
        public SelectItem()
        {
        }
        private string selectListBoxText;
        /// <summary>
        /// 列表项文本
        /// </summary>
        /// <remarks>列表项文本</remarks>
        [Description("列表项文本")]
        public string SelectListBoxText
        {
            get { return this.selectListBoxText; }
            set { this.selectListBoxText = value; }
        }
        private string selectListBoxValue;
        /// <summary>
        /// 列表项文本值
        /// </summary>
        /// <remarks>列表项文本值</remarks>
        [Description("列表项文本值")]
        public string SelectListBoxValue
        {
            get { return this.selectListBoxValue; }
            set { this.selectListBoxValue = value; }
        }
        private string selectListBoxSortColumn;
        /// <summary>
        /// 排序字段
        /// </summary>
        /// <remarks>排序字段</remarks>
        [Description("排序字段")]
        public string SelectListBoxSortColumn
        {
            get { return this.selectListBoxSortColumn; }
            set { this.selectListBoxSortColumn = value; }
        }

        private string commonListBoxSortColumn;
        /// <summary>
        /// 通用排序字段.当左右两边的排序字段不一样的时候，从左边选到右面时，就会使用本字段对左面重新排序
        /// </summary>
        /// <remarks>通用排序字段</remarks>
        [Description("通用排序字段")]
        public string CommonListBoxSortColumn
        {
            get { return this.commonListBoxSortColumn; }
            set { this.commonListBoxSortColumn = value; }
        }

        private string selectItemType;
        /// <summary>
        /// 列表项所属类别
        /// </summary>
        /// <remarks>列表项所属类别</remarks>
        [Browsable(false)]
        [Description("列表项所属类别")]
        public string SelectItemType
        {
            get { return this.selectItemType; }
            set { this.selectItemType = value; }
        }
        private bool locked;
        /// <summary>
        /// 是否锁定
        /// </summary>
        /// <remarks>是否锁定</remarks>
        [Description("是否锁定")]
        public bool Locked
        {
            get { return this.locked; }
            set { this.locked = value; }
        }
        private bool selected;
        /// <summary>
        /// 是否被选中
        /// </summary>
        /// <remarks>是否被选中</remarks>
        [Description("是否被选中")]
        public bool Selected
        {
            get { return this.selected; }
            set { this.selected = value; }
        }
        private string title = string.Empty;
        /// <summary>
        /// Title
        /// </summary>
        /// <remarks>Title</remarks>
        [Description("Title")]
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }         
    }
    /// <summary>
    /// 数据列表类的Collection类
    /// </summary>
    /// <remarks>数据列表类的Collection,继承自Collection</remarks>
    public class SelectItemCollection : CollectionBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>构造函数</remarks>
        public SelectItemCollection()
        {
        }
        /// <summary>
        /// 数组索引
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>数组索引</returns>
        /// <remarks>数组索引</remarks>
        public SelectItem this[int index]
        {
            get
            {
                return ((SelectItem)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }
        /// <summary>
        /// 添加Item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>添加Item</returns>
        /// <remarks>添加Item</remarks>
        public int Add(SelectItem item)
        {
            return (List.Add(item));
        }
        /// <summary>
        /// 取得Item索引值
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>取得Item的索引值</returns>
        /// <remarks>取得Item索引值</remarks>
        public int IndexOf(SelectItem item)
        {
            return (List.IndexOf(item));
        }
        /// <summary>
        /// 在指定位置添加Item
        /// </summary>
        /// <param name="index">索引位置</param>
        /// <param name="item">Item</param>
        /// <remarks>在指定位置添加Item</remarks>
        public void Insert(int index, SelectItem item)
        {
            List.Insert(index, item);
        }
        /// <summary>
        /// 移除Item
        /// </summary>
        /// <param name="item">Item</param>
        /// <remarks>移除Item</remarks>
        public void Remove(SelectItem item)
        {
            List.Remove(item);
        }
        /// <summary>
        /// 清空Items
        /// </summary>
        /// <remarks>清空Items</remarks>
        public new void Clear()
        {
            List.Clear();
        }
    }

    /// <summary>
    /// DeltaItemCollection
    /// </summary>
    public class DeltaItemCollection
    {
        SelectItemCollection insertedItems = new SelectItemCollection();

        /// <summary>
        /// InsertedItems
        /// </summary>
        public SelectItemCollection InsertedItems
        {
            get { return this.insertedItems; }
            set { this.insertedItems = value; }
        }

        SelectItemCollection deletedItems = new SelectItemCollection();

        /// <summary>
        /// DeletedItems
        /// </summary>
        public SelectItemCollection DeletedItems
        {
            get { return this.deletedItems; }
            set { this.deletedItems = value; }
        }

        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {
            this.insertedItems.Clear();
            this.deletedItems.Clear();
        }
    }
}
