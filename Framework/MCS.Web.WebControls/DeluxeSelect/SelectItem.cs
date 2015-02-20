#region
// -------------------------------------------------
// Assembly	��	
// FileName	��	ButtonItem.cs
// Remark	��  �б�Collection��
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		songhaojie	    20070706		����
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
    /// ѡ����
    /// </summary>
    /// <remarks>ѡ����</remarks>
    public class SelectItem
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <remarks>���캯��</remarks>
        public SelectItem()
        {
        }
        private string selectListBoxText;
        /// <summary>
        /// �б����ı�
        /// </summary>
        /// <remarks>�б����ı�</remarks>
        [Description("�б����ı�")]
        public string SelectListBoxText
        {
            get { return this.selectListBoxText; }
            set { this.selectListBoxText = value; }
        }
        private string selectListBoxValue;
        /// <summary>
        /// �б����ı�ֵ
        /// </summary>
        /// <remarks>�б����ı�ֵ</remarks>
        [Description("�б����ı�ֵ")]
        public string SelectListBoxValue
        {
            get { return this.selectListBoxValue; }
            set { this.selectListBoxValue = value; }
        }
        private string selectListBoxSortColumn;
        /// <summary>
        /// �����ֶ�
        /// </summary>
        /// <remarks>�����ֶ�</remarks>
        [Description("�����ֶ�")]
        public string SelectListBoxSortColumn
        {
            get { return this.selectListBoxSortColumn; }
            set { this.selectListBoxSortColumn = value; }
        }

        private string commonListBoxSortColumn;
        /// <summary>
        /// ͨ�������ֶ�.���������ߵ������ֶβ�һ����ʱ�򣬴����ѡ������ʱ���ͻ�ʹ�ñ��ֶζ�������������
        /// </summary>
        /// <remarks>ͨ�������ֶ�</remarks>
        [Description("ͨ�������ֶ�")]
        public string CommonListBoxSortColumn
        {
            get { return this.commonListBoxSortColumn; }
            set { this.commonListBoxSortColumn = value; }
        }

        private string selectItemType;
        /// <summary>
        /// �б����������
        /// </summary>
        /// <remarks>�б����������</remarks>
        [Browsable(false)]
        [Description("�б����������")]
        public string SelectItemType
        {
            get { return this.selectItemType; }
            set { this.selectItemType = value; }
        }
        private bool locked;
        /// <summary>
        /// �Ƿ�����
        /// </summary>
        /// <remarks>�Ƿ�����</remarks>
        [Description("�Ƿ�����")]
        public bool Locked
        {
            get { return this.locked; }
            set { this.locked = value; }
        }
        private bool selected;
        /// <summary>
        /// �Ƿ�ѡ��
        /// </summary>
        /// <remarks>�Ƿ�ѡ��</remarks>
        [Description("�Ƿ�ѡ��")]
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
    /// �����б����Collection��
    /// </summary>
    /// <remarks>�����б����Collection,�̳���Collection</remarks>
    public class SelectItemCollection : CollectionBase
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <remarks>���캯��</remarks>
        public SelectItemCollection()
        {
        }
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="index">����ֵ</param>
        /// <returns>��������</returns>
        /// <remarks>��������</remarks>
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
        /// ���Item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>���Item</returns>
        /// <remarks>���Item</remarks>
        public int Add(SelectItem item)
        {
            return (List.Add(item));
        }
        /// <summary>
        /// ȡ��Item����ֵ
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>ȡ��Item������ֵ</returns>
        /// <remarks>ȡ��Item����ֵ</remarks>
        public int IndexOf(SelectItem item)
        {
            return (List.IndexOf(item));
        }
        /// <summary>
        /// ��ָ��λ�����Item
        /// </summary>
        /// <param name="index">����λ��</param>
        /// <param name="item">Item</param>
        /// <remarks>��ָ��λ�����Item</remarks>
        public void Insert(int index, SelectItem item)
        {
            List.Insert(index, item);
        }
        /// <summary>
        /// �Ƴ�Item
        /// </summary>
        /// <param name="item">Item</param>
        /// <remarks>�Ƴ�Item</remarks>
        public void Remove(SelectItem item)
        {
            List.Remove(item);
        }
        /// <summary>
        /// ���Items
        /// </summary>
        /// <remarks>���Items</remarks>
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
