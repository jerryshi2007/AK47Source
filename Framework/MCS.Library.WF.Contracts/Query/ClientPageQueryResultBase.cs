using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Query
{
    [Serializable]
    [DataContract]
    [DebuggerDisplay("TotalCount = {TotalCount}")]
    public abstract class ClientPageQueryResultBase<T, TCollection>
        where T : new()
        where TCollection : EditableDataObjectCollectionBase<T>, new()
    {
        private TCollection _QueryResult = null;

        public TCollection QueryResult
        {
            get
            {
                if (this._QueryResult == null)
                    this._QueryResult = new TCollection();

                return this._QueryResult;
            }
        }

        /// <summary>
        /// 查询结果中总行数
        /// </summary>
        public int TotalCount
        {
            get;
            set;
        }
    }
}
