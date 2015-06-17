
using System.Linq;

namespace MCS.Library.SOA.DataObjects
{
    public sealed class RelativeLinkAdapter : UpdatableAndLoadableAdapterBase<RelativeLink, RelativeLinkCollection>
    {
        /// <summary>
        /// Adapter实例
        /// </summary>
        public static readonly RelativeLinkAdapter Instance = new RelativeLinkAdapter();

        private RelativeLinkAdapter()
        {
        }

        /// <summary>
        /// 根据相关链接的Code查询一条相关链接
        /// </summary>
        /// <param name="relativeLinkID"> </param>
        /// <returns>相关链接</returns>
        public RelativeLink GetRelativeLinkByID(string relativeLinkID)
        {
            RelativeLinkCollection collection = Load(o => o.AppendItem("RELATIVE_LINK_ID", relativeLinkID));

            return collection.FirstOrDefault();
        }

        /// <summary>
        /// 根据相关链接的CodeName查询一条相关链接
        /// </summary>
        /// <param name="codeName">相关链接的CodeName</param>
        /// <returns>相关链接</returns>
        public RelativeLink GetRelativeLinkByName(string codeName)
        {
            RelativeLinkCollection collection = Load(o => o.AppendItem("CODE_NAME", codeName));

            return collection.FirstOrDefault();
        }

        /// <summary>
        /// 根据KBID查询所有相关链接
        /// </summary>
        /// <param name="groupName">相关链接的组名</param>
        /// <returns>相关链接集合</returns>
        public RelativeLinkCollection GetRelativeLinks(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return Load(p => p.AppendItem("1", "1", "="));
            }

            return Load(p =>
                            {
                                p.AppendItem("RELATIVE_LINK_GROUP_CODE_NAME", groupName, "=");
                                p.AppendItem("ENABLE", "1", "=");
                            }
                );
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

