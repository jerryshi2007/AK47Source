using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.Core;
using System.Data;

namespace MCS.Library.SOA.DataObjects
{
    [Serializable]
    [ORTableMapping("WF.ROLE_PROPERTIES_CELLS")]
    [TenantRelativeObject]
    public sealed class SOARolePropertyValue : RowValueBase<SOARolePropertyDefinition, string>
    {
        public SOARolePropertyValue()
        {
        }

        public SOARolePropertyValue(SOARolePropertyDefinition rpd)
        {
            rpd.NullCheck("rpd");

            Column = rpd;
        }

        /// <summary>
        /// 设置列信息
        /// </summary>
        /// <param name="rpd"></param>
        public void SetColumnInfo(SOARolePropertyDefinition rpd)
        {
            rpd.NullCheck("rpd");

            this.Column = rpd;
        }

        [ORFieldMapping("STRING_VALUE")]
        public override string Value
        {
            get;
            set;
        }
    }

    [Serializable]
    public class SOARolePropertyValueCollection : RowValueCollectionBase<SOARolePropertyDefinition, SOARolePropertyValue, string>
    {
        /// <summary>
        /// 将所有行的单元格信息按照列定义，形成每一行的Values的内容
        /// </summary>
        /// <param name="view">ROLE_PROPERTIES_CELLS表的每一行信息，每一行表示一个单元格的值</param>
        /// <param name="definition">列定义信息</param>
        /// <returns></returns>
        internal static Dictionary<int, SOARolePropertyValueCollection> LoadAndGroup(DataView view, SOARolePropertyDefinitionCollection definition)
        {
            Dictionary<int, SOARolePropertyValueCollection> result = new Dictionary<int, SOARolePropertyValueCollection>();

            foreach (DataRowView drv in view)
            {
                SOARolePropertyDefinition rpd = definition[drv["PROPERTY_NAME"].ToString()];

                //如果找到了列定义...
                if (rpd != null)
                {
                    SOARolePropertyValue pv = new SOARolePropertyValue(rpd);

                    pv.Value = drv["STRING_VALUE"].ToString();

                    object objRowNumber = drv["PROPERTIES_ROW_ID"];
                    int rowNumber = 0;

                    if (objRowNumber is int)
                        rowNumber = (int)objRowNumber;

                    SOARolePropertyValueCollection values = null;

                    if (result.TryGetValue(rowNumber, out values) == false)
                    {
                        values = new SOARolePropertyValueCollection();
                        result.Add(rowNumber, values);
                    }

                    values.Add(pv);
                }
            }

            foreach (KeyValuePair<int, SOARolePropertyValueCollection> kp in result)
            {
                kp.Value.Sort((v1, v2) => v1.Column.SortOrder - v2.Column.SortOrder);
            }

            return result;
        }

        /// <summary>
        /// 匹配一个条件
        /// </summary>
        /// <param name="queryParam"></param>
        /// <returns></returns>
        public bool MatchQueryValue(SOARolePropertiesQueryParam queryParam)
        {
            bool matched = false;

            if (queryParam != null)
            {
                string cellValue = this.GetValue(queryParam.QueryName, (string)null);

                matched = queryParam.MatchQueryValue(cellValue);
            }

            return matched;
        }

        /// <summary>
        /// 匹配一组条件
        /// </summary>
        /// <param name="queryParams"></param>
        /// <param name="matchAny">任意一项匹配就返回True还是全部匹配返回True</param>
        /// <returns></returns>
        public bool MatchQueryValues(IEnumerable<SOARolePropertiesQueryParam> queryParams, bool matchAny = false)
        {
            bool matched = false;

            if (queryParams.FirstOrDefault() != null)
            {
                if (matchAny == false)
                    matched = queryParams.AllAndNotEmpty(queryParam => this.MatchQueryValue(queryParam));
                else
                    matched = queryParams.Any(queryParam => this.MatchQueryValue(queryParam));
            }
            else
                matched = true;

            return matched;
        }
    }
}
