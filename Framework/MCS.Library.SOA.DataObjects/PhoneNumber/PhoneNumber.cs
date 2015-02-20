using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.Builder;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 联系信息
    /// </summary>
    [Serializable]
    [ORTableMapping("WF.PHONENUMBER")]
	public class PhoneNumber:IVersionDataObject
    {
        /// <summary>
        /// 转为字符串
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return format.Replace("C", StateCode).Replace("c", StateCode).Replace("A", AreaCode).Replace("a", AreaCode).Replace("T", MainCode).Replace("t", MainCode).Replace("E", ExtCode).Replace("e", ExtCode);

            //format = format.ToUpper();

            //string formatData = string.Empty;

            //string[] formatArray = format.Split('-');

            //for (int i = 0; i < formatArray.Length; i++)
            //{
            //    string firstWord = formatArray[i].Substring(0,1);

            //    int number = Convert.ToInt32(formatArray[i].Substring(1));

            //    string innerFormatStr = string.Format("0:D{0}", number);
   
            //    switch (firstWord)
            //    {
            //        case "C":
            //            formatData += string.Format(string.Format("{{{0}}}-", innerFormatStr),this.StateCode == ""?0:Convert.ToInt32(this.StateCode));
            //            break;
            //        case "A":
            //            formatData += string.Format(string.Format("{{{0}}}-", innerFormatStr),this.AreaCode =="" ?0:Convert.ToInt32(this.AreaCode));
            //            break;
            //        case "T":
            //            formatData += string.Format(string.Format("{{{0}}}-", innerFormatStr),this.MainCode =="" ?0:Convert.ToInt32(this.MainCode));
            //            break;
            //        case "E":
            //            formatData += string.Format(string.Format("{{{0}}}-", innerFormatStr),this.ExtCode == "" ?0:Convert.ToInt32(this.ExtCode));
            //            break;
            //        default:
            //            throw new InvalidCastException("格式无效！");
            //    }
            //}

            //formatData = formatData.TrimEnd('-');

            //return formatData;
        }

        /// <summary>
        /// 编号
        /// </summary>
        [ORFieldMapping("Code", PrimaryKey = true, IsNullable = false)]
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// 人员、公司等的ID
        /// </summary>
        [ORFieldMapping("ResourceID")]
        public string ResourceID
        {
            get;
            set;
        }

        /// <summary>
        /// 类别
        /// </summary>
        [ORFieldMapping("Class")]
        public string TelephoneClass
        {
            get;
            set;
        }

		/// <summary>
		/// 国别号
		/// </summary>
		[ORFieldMapping("StateCode")]
		public string StateCode
		{
			get;
			set;
		}

        /// <summary>
        /// 区号
        /// </summary>
        [ORFieldMapping("AreaCode")]
        public string AreaCode
        {
            get;
            set;
        }

        /// <summary>
        /// 电话号码
        /// </summary>
        [ORFieldMapping("MainCode")]
        public string MainCode
        {
            get;
            set;
        }

        /// <summary>
        /// 分机号
        /// </summary>
        [ORFieldMapping("ExtCode")]
        public string ExtCode
        {
            get;
            set;
        }

        /// <summary>
        /// 内部排序号
        /// </summary>
        [ORFieldMapping("InnerSort")]
        public int InnerSort
        {
            get;
            set;
        }

        /// <summary>
        /// 版本开始时间
        /// </summary>
        [ORFieldMapping("VersionStartTime", PrimaryKey = true)]
        public DateTime VersionStartTime
        {
            get;
            set;
        }

        /// <summary>
        /// 版本结束时间
        /// </summary>
        [ORFieldMapping("VersionEndTime")]
        public DateTime VersionEndTime
        {
            get;
            set;
        }

        /// <summary>
        /// 默认值
        /// </summary>
        [ORFieldMapping("IsDefault")]
        public int IsDefault
        {
            get;
            set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        [ORFieldMapping("Description")]
        public string Description
        {
            get;
            set;
        }

		/// <summary>
		/// 电话号码更改状态
		/// </summary>
		[NoMapping]
		public bool Changed
		{
			get;
			set;
		}

    }

    /// <summary>
	/// PhoneNumber集合类
	/// </summary>
    [Serializable]
	public class PhoneNumberList : EditableDataObjectCollectionBase<PhoneNumber>
    {
 
    }

}
