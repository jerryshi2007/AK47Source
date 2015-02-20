using System.Collections.Generic;
using System.Linq;
using MCS.Library.SOA.DataObjects;

namespace MCS.Library.SOA.Web.WebControls.Test.DeluxeSearch.DataObjects
{
    public sealed class SupplierAdapter : UpdatableAndLoadableAdapterBase<Supplier, SupplierCollection>
    {
        /// <summary>
        /// Adapter实例
        /// </summary>
        public static readonly SupplierAdapter Instance = new SupplierAdapter();

        private SupplierAdapter()
        {
        }
        /// <summary>
        /// 根据供应商的Code查询一条供应商
        /// </summary>
        /// <param name="code">供应商的Code</param>
        /// <returns>供应商</returns>
        public Supplier GetSupplierByCode(string code)
        {
            var collection = Load(o => o.AppendItem("Code", code));
            return collection.FirstOrDefault();
        }

        /// <summary>
        /// 根据ID查询所有供应商
        /// </summary>
        /// <returns>供应商集合</returns>
        public SupplierCollection GetSuppliers()
        {
            return Load(p => p.AppendItem("1", 1, "="));
        }
        /// <summary>
        /// 保存供应商
        /// </summary>
        /// <param name="items">供应商集合</param>
        public void UpdateSupplier(SupplierCollection items)
        {
            foreach (var item in items)
            {
                Update(item);
            }
        }
        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="items">供应商的code列表</param>
        public void DeleteSupplier(List<string> items)
        {           
        }
        /// <summary>
        /// 获取连接字串
        /// </summary>
        protected override string GetConnectionName()
        {
            return DatabaseUtils.SinooceanProductresearch;
        }
    }
}