#region
// -------------------------------------------------
// Assembly	��	HB.DataObjects
// FileName	��	ObjectCompareAttribute.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    2008-03-17		����
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Data.DataObjects
{
    /// <summary>
    /// �����˶���Ƚϵ������Ϣ
    /// </summary>
    /// <remarks>
    /// ��Щ��Ϣ������keyField���Ƿ�ΪList��
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ObjectCompareAttribute : Attribute, IObjectCompareInfo
    {
        /// <summary>
        /// ��Ҫ�Ƚϵ��ֶΣ����Ի��Ƕ���ֶΣ��ɶ��Ż�ֺŷָ�
        /// </summary>
        public string KeyFields
        {
            get;
            set;
        }

        /// <summary>
        /// �Ƿ�ΪList
        /// </summary>
        public bool IsList
        {
            get;
            set;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="keyFieldsName"></param>
        public ObjectCompareAttribute(string keyFieldsName)
        {
            this.KeyFields = keyFieldsName;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="list"></param>
        public ObjectCompareAttribute(bool list)
        {
            this.IsList = list;
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="keyFieldName"></param>
        /// <param name="list"></param>
        public ObjectCompareAttribute(string keyFieldName, bool list)
        {
            this.KeyFields = keyFieldName;
            this.IsList = list;
        }
    }
}
