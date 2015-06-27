using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 存放机构人员对象的集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [XElementSerializable]
    public class OguDataCollection<T> : DataObjectCollectionBase<T>, IConvertible, IEnumerable<T> where T : IOguObject
    {
        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public OguDataCollection()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="source"></param>
        public OguDataCollection(OguDataCollection<T> source)
        {
            for (int i = 0; i < source.Count; i++)
                this.Add(source[i]);
        }

        /// <summary>
        /// 从IList构造并且填充元素
        /// </summary>
        /// <param name="inputList"></param>
        public OguDataCollection(IEnumerable<T> inputList)
        {
            this.CopyFrom(inputList);
        }
        #endregion 构造方法

        #region Array Operations
        /// <summary>
        /// 按照指定的规则排序
        /// </summary>
        /// <param name="orderByProperty"></param>
        /// <param name="direction"></param>
        public void Sort(OrderByPropertyType orderByProperty, SortDirectionType direction)
        {
            T[] objArray = this.ToArray();

            Array.Sort(objArray, delegate(T x, T y)
                                    {
                                        int result = 0;

                                        switch (orderByProperty)
                                        {
                                            case OrderByPropertyType.FullPath:
                                                result = x.FullPath.CompareTo(y.FullPath);
                                                break;
                                            case OrderByPropertyType.GlobalSortID:
                                                result = x.GlobalSortID.CompareTo(y.GlobalSortID);
                                                break;
                                            case OrderByPropertyType.Name:
                                                result = x.Name.CompareTo(y.Name);
                                                break;
                                        }

                                        if (direction == SortDirectionType.Descending)
                                            result = -result;

                                        return result;
                                    });

            this.Clear();
            this.CopyFrom(objArray);
        }

        /// <summary>
        /// 根据fullpath查找符合条件的第一个对象
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public T FindSingleObjectByFullPath(string fullPath)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(fullPath, "fullPath");

            return this.Find(delegate(T item) { return string.Compare(item.FullPath, fullPath, true) == 0; });
        }

        /// <summary>
        /// 根据fullpath查找符合的对象
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public IList<T> FindObjectsByFullPath(string fullPath)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(fullPath, "fullPath");

            return this.FindAll(delegate(T item) { return string.Compare(item.FullPath, fullPath, true) == 0; });
        }

        /// <summary>
        /// 根据id查找符合条件的第一个对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IOguObject FindSingleObjectByID(string id)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(id, "id");

            return this.Find(delegate(T item) { return string.Compare(item.ID, id, true) == 0; });
        }

        /// <summary>
        /// 根据id查找符合的对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IList<T> FindObjectsByID(string id)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(id, "id");

            return this.FindAll(delegate(T item) { return string.Compare(item.ID, id, true) == 0; });
        }
        #endregion

        #region Collection Operations
        /// <summary>
        /// 增加不存在的数据（已经存在的将被忽略）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="predicate"></param>
        /// <returns>是否增加了数据</returns>
        public virtual bool AddNotExistsItem(T data, Predicate<T> predicate)
        {
            data.NullCheck("data");

            bool needToAdd = predicate == null || this.Exists(predicate) == false;

            if (needToAdd)
                this.Add(data);

            return needToAdd;
        }

        public void Add(T obj)
        {
            InnerAdd(obj);
        }

        public T this[int index]
        {
            get
            {
                return (T)List[index];
            }
            set
            {
                ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "value");

                IOguObject data = value;

                if ((value is OguBase) == false)
                    data = OguBase.CreateWrapperObject(value);

                List[index] = data;
            }
        }

        public void Remove(T obj)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(obj != null, "obj");

            List.Remove(obj);
        }

        protected override void InnerAdd(T obj)
        {
            obj = (T)OguBase.CreateWrapperObject(obj);
            base.InnerAdd(obj);
        }
        #endregion Collection Operations

        #region IConvertible Members

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            return this.GetType().ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            object result = null;

            if (typeof(OguDataCollection<IUser>) == conversionType)
            {
                result = new OguDataCollection<IUser>();
                this.ForEach(obj =>
                    { if (obj is IUser) ((OguDataCollection<IUser>)result).Add((IUser)obj); });
            }
            else
                if (typeof(OguDataCollection<IOrganization>) == conversionType)
                {
                    result = new OguDataCollection<IOrganization>();
                    this.ForEach(obj =>
                        { if (obj is IOrganization) ((OguDataCollection<IOrganization>)result).Add((IOrganization)obj); });
                }
                else
                    if (typeof(OguDataCollection<IGroup>) == conversionType)
                    {
                        result = new OguDataCollection<IGroup>();
                        this.ForEach(obj =>
                            { if (obj is IGroup) ((OguDataCollection<IGroup>)result).Add((IGroup)obj); });
                    }
                    else
                        if (typeof(OguDataCollection<IOguObject>) == conversionType)
                        {
                            result = new OguDataCollection<IOguObject>();
                            this.ForEach(obj => ((OguDataCollection<IOguObject>)result).Add(obj));
                        }

            return result;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
