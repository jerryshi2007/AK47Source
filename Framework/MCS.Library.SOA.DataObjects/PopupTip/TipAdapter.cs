using System.Data;
using System.Linq;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Data.Mapping;

namespace MCS.Library.SOA.DataObjects
{
    public sealed class TipAdapter : UpdatableAndLoadableAdapterBase<Tip, TipCollection>
    {
        /// <summary>
        /// Adapter实例
        /// </summary>
        public static readonly TipAdapter Instance = new TipAdapter();

        private TipAdapter()
        {
        }

        public Tip GetTipByID(string tipID)
        {
            TipCollection collection = Load(o => o.AppendItem("TIP_ID", tipID));

            return collection.FirstOrDefault();
        }

        /// <summary>
        /// 根据提示表的CodeName查询一条提示表
        /// </summary>
        /// <param name="codeName">提示表的name</param>
        /// <param name="cultureName"> 区域</param>
        /// <returns>提示表</returns>
        public Tip GetTipByName(string codeName, string cultureName)
        {
            TipCollection collection = Load(o =>
                                      {
                                          o.AppendItem("CODE_NAME", codeName);
                                          o.AppendItem("CULTURE", cultureName);
                                          o.AppendItem("ENABLE", "1");
                                      }
                );

            return collection.FirstOrDefault();
        }

        /// <summary>
        /// 根据提示信息的编码得到所有提示信息
        /// </summary>
        /// <param name="tipName">编码</param>
        /// <param name="cultureName"> 区域</param>
        /// <returns></returns>
        public TipCollection GetTips(string tipName, string cultureName)
        {
            if (string.IsNullOrEmpty(tipName))
            {
                return Load(p => p.AppendItem("1", "1", "="));
            }

            return Load(p =>
                            {
                                p.AppendItem("CODE_NAME", tipName, "=");
                                p.AppendItem("CULTURE", cultureName, "=");
                            });
        }

        /// <summary>
        /// 根据ID查询所有提示表
        /// </summary>
        /// <param name="codeArray">CodeName的数组 </param>
        /// <param name="cultureName"> 区域</param>
        /// <returns>提示信息集合</returns>
        public TipCollection GetTips(string[] codeArray, string cultureName)
        {
            (codeArray != null).FalseThrow("提示信息列表不能为null");

            InSqlClauseBuilder builder = new InSqlClauseBuilder("CODE_NAME");

            builder.AppendItem(codeArray);

            var result = new TipCollection();

            if (builder.Count > 0)
            {
                string sql = string.Format("SELECT * FROM KB.TIP WHERE ENABLE ='1' AND CULTURE='{1}' AND {0} ",
                    builder.AppendTenantCodeSqlClause(typeof(Tip)).ToSqlString(TSqlBuilder.Instance), cultureName);

                DataTable table = null;
                DbHelper.RunSql(db => table = db.ExecuteDataSet(CommandType.Text, sql).Tables[0], ConnectionDefine.KBConnectionName);

                ORMapping.DataViewToCollection(result, table.DefaultView);
            }

            return result;
        }

        /// <summary>
        /// 获取连接字串
        /// </summary>
        protected override string GetConnectionName()
        {
            return ConnectionDefine.KBConnectionName;
        }
    }
}

