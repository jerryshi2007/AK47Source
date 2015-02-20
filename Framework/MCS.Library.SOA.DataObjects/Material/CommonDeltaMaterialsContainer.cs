#region
// -------------------------------------------------
// Assembly	：	MCS.Library.SOA.DataObjects
// FileName	：	CommonDeltaMaterialsContainer.cs
// Remark	：	公共DeltaMaterial容器
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			fengll		20140227		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 公共DeltaMaterial容器
    /// </summary>
    public class CommonDeltaMaterialsContainer
    {
        private Dictionary<string, DeltaMaterialList> _DeltaMaterialsDic;

        public CommonDeltaMaterialsContainer(Dictionary<string, DeltaMaterialList> deltaMaterialsDic)
        {
            this._DeltaMaterialsDic = deltaMaterialsDic;
        }

        /// <summary>
        /// 公共DeltaMaterialList字典
        /// </summary>
        public Dictionary<string, DeltaMaterialList> DeltaMaterialsDic
        {
            get
            {
                return _DeltaMaterialsDic;
            }
            set
            {
                _DeltaMaterialsDic = value;
            }
        }

        /// <summary>
        /// 转换为IList集合
        /// </summary>
        /// <returns></returns>
        public IList<DeltaMaterialList> ToList()
        {
            _DeltaMaterialsDic.NullCheck("_DeltaMaterialsDic");

            List<DeltaMaterialList> result = new List<DeltaMaterialList>(); ;
            
            foreach (var deltaMaterialList in _DeltaMaterialsDic.Values)
            {
                result.Add(deltaMaterialList);
            }

            return result;
        }
    }
}
