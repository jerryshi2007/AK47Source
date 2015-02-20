using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MCS.Library.Office.OpenXml.Excel
{
    /// <summary>
    /// 样式集合基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StyleCollection<T> : IEnumerable<T>
    {
        private bool _SetNextIdManual;

        public StyleCollection()
        {
            this._SetNextIdManual = false;
        }


        public StyleCollection(bool SetNextIdManual)
        {
            this._SetNextIdManual = SetNextIdManual;
        }

        internal int NextId = 0;
        private List<T> _List = new List<T>();
        private Dictionary<string, int> _Dic = new Dictionary<string, int>();

        #region “实现IEnumerable<T> 方法”
        public IEnumerator<T> GetEnumerator()
        {
            return this._List.GetEnumerator();
        }

        #endregion

        #region “IEnumerable方法”
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._List.GetEnumerator();
        }
        #endregion

        public T this[int PositionID]
        {
            get
            {
                return this._List[PositionID];
            }
        }

        public int Count
        {
            get
            {
                return this._List.Count;
            }
        }

        internal int Add(T item)
        {
            this._List.Add(item);

            if (this._SetNextIdManual)
            {
                NextId++;
            }

            return this._List.Count - 1;
        }

        internal int Add(string key, T item)
        {
            this._List.Add(item);
            if (!this._Dic.ContainsKey(key))
            {
                this._Dic.Add(key, _List.Count - 1);
            }
            if (this._SetNextIdManual)
            {
                NextId++;
            }

            return _List.Count - 1;
        }

        /// <summary>
        /// 根据key查找
        /// </summary>
        internal bool FindByID(string key, ref T obj)
        {
            if (this._Dic.ContainsKey(key))
            {
                obj = this._List[_Dic[key]];

                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Find Index
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal int FindIndexByID(string key)
        {
            if (this._Dic.ContainsKey(key))
            {
                return this._Dic[key];
            }
            else
            {
                return int.MinValue;
            }
        }

        internal bool ExistsKey(string key)
        {
            return this._Dic.ContainsKey(key);
        }

        internal void Sort(Comparison<T> c)
        {
            this._List.Sort(c);
        }
    }
}
