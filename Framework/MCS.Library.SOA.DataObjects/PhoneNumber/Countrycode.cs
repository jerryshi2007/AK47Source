using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 国家（国别号）字典类
    /// </summary>
    [Serializable]
    [ORTableMapping("WF.COUNTRY_CODE")]
    public class Countrycode
    {
        /// <summary>
        /// 构造方法
        /// </summary>
		public Countrycode()
        {
        }

        /// <summary>
        /// 编号
        /// </summary>
        [ORFieldMapping("Code")]
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// 国家中文名称
        /// </summary>
        [ORFieldMapping("CnName")]
        public string CnName
        {
            get;
            set;
        }

        /// <summary>
        /// 国家英文名称
        /// </summary>
        [ORFieldMapping("EnName")]
        public string EnName
        {
            get;
            set;
        }

        /// <summary>
        /// 国家英文名称缩写
        /// </summary>
        [ORFieldMapping("Abbreviation")]
        public string Abbreviation
        {
            get;
            set;
        }

        /// <summary>
        /// 是否有效
        /// </summary>
        [ORFieldMapping("ValidStatus")]
        public bool ValidStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 排序号
        /// </summary>
        [ORFieldMapping("SortNo")]
        public int SortNo
        {
            get;
            set;
        }
    }

    [Serializable]
    public class CountrycodeCollection : EditableDataObjectCollectionBase<Countrycode>
    {

    }

    public class CountrycodeAdapter : UpdatableAndLoadableAdapterBase<Countrycode, CountrycodeCollection>
    {
		public static readonly CountrycodeAdapter Instance = new CountrycodeAdapter();

        /// <summary>
        /// 得到连接串的名称
        /// </summary>
        /// <returns></returns>
        protected override string GetConnectionName()
        {
            return ConnectionDefine.DBConnectionName;
        }

		public CountrycodeCollection LoadAllFromCache()
		{
			object result;
			CountrycodeCollection coll;

			if (ObjectCacheQueue.Instance.TryGetValue("CountrycodeCollection", out result) == false){
				coll = this.Load(p => p.AppendItem("ValidStatus", true));
				ObjectCacheQueue.Instance.Add("CountrycodeCollection", coll, new SlidingTimeDependency(TimeSpan.FromHours(1)));
			}
			else{
				coll = (CountrycodeCollection)result;
			}

			return coll;
		}

    }


	public class PropertyFormCountrySource
	{
		public IList<DropdownLitsItem> GetPropertiesList()
		{
			List<DropdownLitsItem> result = new List<DropdownLitsItem>();

			CountrycodeCollection countries = CountrycodeAdapter.Instance.LoadAllFromCache();

			foreach (Countrycode country in countries)
			{
				DropdownLitsItem item = new DropdownLitsItem() { Code = string.Format("{0}", country.Code), CnName = string.Format("{0}", country.CnName) };
				result.Add(item);
			}

			return result;
		}
	}
	public class DropdownLitsItem
	{
		public string Code { get; set; }

		public string CnName { get; set; }
	}

}
