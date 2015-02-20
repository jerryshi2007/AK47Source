using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using MCS.Library.Core;
using System.Runtime.InteropServices;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// Delta数据的公共抽象基类
    /// </summary>
	[Serializable, ComVisible(true)]
    public abstract class DeltaDataCollectionBase
    {
        protected abstract DeltaDataCollectionBase CreateNewInstance();

        public virtual DeltaDataCollectionBase Clone()
        {
            DeltaDataCollectionBase result = CreateNewInstance();

            result.Append(this);

            return result;
        }

        public abstract void Append(DeltaDataCollectionBase data);

        public abstract bool IsEmpty();
    }

    /// <summary>
    /// Delta数据的泛型抽象基类，各控件的Delta数据从该类派生
    /// </summary>
    /// <typeparam name="T">业务实体的集合对象</typeparam>
	[Serializable, ComVisible(true)]
    public abstract class DeltaDataCollectionBase<T> : DeltaDataCollectionBase where T : IList, new()
    {
        private T inserted = default(T);
        private T updated = default(T);
        private T deleted = default(T);

        /// <summary>
        /// 新增的集合
        /// </summary>
        public T Inserted
        {
            get
            {
                if (this.inserted == null)
                    this.inserted = new T();

                return this.inserted;
            }
            set
            {
                ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "inserted");
                this.inserted = value;
            }
        }

        /// <summary>
        /// 更新的集合
        /// </summary>
        public T Updated
        {
            get
            {
                if (this.updated == null)
                    this.updated = new T();

                return updated;
            }
            set
            {
                ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "updated");
                this.updated = value;
            }
        }

        /// <summary>
        /// 删除的集合
        /// </summary>
        public T Deleted
        {
            get
            {
                if (this.deleted == null)
                    this.deleted = new T();

                return this.deleted;
            }
            set
            {
                ExceptionHelper.FalseThrow<ArgumentNullException>(value != null, "deleted");
                this.deleted = value;
            }
        }

        /// <summary>
        /// Clone方法
        /// </summary>
        /// <returns>新对象</returns>
        public override DeltaDataCollectionBase Clone()
        {
            DeltaDataCollectionBase<T> result = (DeltaDataCollectionBase<T>)base.Clone();

            result.Append(this);

            return result;
        }

        /// <summary>
        /// 清空各子集合对象
        /// </summary>
        public void Clear()
        {
            if (this.inserted != null)
                this.inserted.Clear();

            if (this.updated != null)
                this.updated.Clear();

            if (this.deleted != null)
                this.deleted.Clear();
        }

        /// <summary>
        /// 附加srcData对象中各子集合到目标对象中
        /// </summary>
        /// <param name="srcData">待附加的delta对象</param>
        public override void Append(DeltaDataCollectionBase srcData)
        {
            DeltaDataCollectionBase<T> data = (DeltaDataCollectionBase<T>)srcData;

            foreach (object obj in data.Inserted)
                this.Inserted.Add(obj);

            foreach (object obj in data.Updated)
                this.Updated.Add(obj);

            foreach (object obj in data.Deleted)
                this.Deleted.Add(obj);
        }
    }

    /// <summary>
    /// 含DeltaData的控件接口
    /// </summary>
    public interface IDeltaDataControl
    {
        DeltaDataCollectionBase DeltaData
        {
            get;
        }

        void AcceptDeltaData();
    }

    /// <summary>
    /// DeltaData的访问器
    /// </summary>
    public static class DeltaDataControlHelper
    {
        private const string DeltaDataKey = "DeltaData";

        /// <summary>
        /// 注册实现IDeltaDataControl接口的控件
        /// </summary>
        /// <param name="control">待注册的控件</param>
        public static void RegisterDeltaDataControl(IDeltaDataControl control)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(control != null, "control");

            DeltaDataControls.Add(control);
        }

        /// <summary>
        /// 根据业务实体的类型，返回该类业务实体的DeltaDataCollection对象
        /// </summary>
        /// <typeparam name="T">业务实体类</typeparam>
        /// <param name="type">业务对象类型</param>
        /// <returns>Delta数据对象</returns>
        public static DeltaDataCollectionBase<T> GetDeltaData<T>(Type type) where T : IList, new()
        {
            DeltaDataCollectionBase<T> result = null;

            foreach (IDeltaDataControl control in DeltaDataControls)
            {
                if (typeof(T) == type)
                {
                    if (result == null)
                        result = (DeltaDataCollectionBase<T>)control.DeltaData.Clone();
                    else
                        result.Append(control.DeltaData);
                }
            }

            return result;
        }

        /// <summary>
        /// 接受所有注册控件的Delta数据
        /// </summary>
        public static void AcceptAll()
        {
            foreach (IDeltaDataControl control in DeltaDataControls)
                control.AcceptDeltaData();
        }

        private static IList<IDeltaDataControl> DeltaDataControls
        {
            get
            {
                IList<IDeltaDataControl> result =
                    (IList<IDeltaDataControl>)HttpContext.Current.Items[DeltaDataKey]; ;

                if (result == null)
                {
                    result = new List<IDeltaDataControl>();
                    HttpContext.Current.Items[DeltaDataKey] = result;
                }

                return result;
            }
        }
    }

}
