using System.Linq;

namespace MCS.Library.SOA.DataObjects
{
    public class RelativeLinkGroupAdapter : UpdatableAndLoadableAdapterBase<RelativeLinkGroup, RelativeLinkGroupCollection>
    {
        /// <summary>
        /// Adapter实例
        /// </summary>
        public static readonly RelativeLinkGroupAdapter Instance = new RelativeLinkGroupAdapter();

        private RelativeLinkGroupAdapter()
        {
        }

        /// <summary>
        /// 根据RELATIVE_LINK_GROUP的Code查询一条RELATIVE_LINK_GROUP
        /// </summary>
        /// <param name="relativeGroupId">RELATIVE_LINK_GROUP的ID </param>
        /// <returns>RELATIVE_LINK_GROUP</returns>
        public RelativeLinkGroup GetRelativeLinkGroupByID(string relativeGroupId)
        {
            RelativeLinkGroupCollection collection = Load(o => o.AppendItem("RELATIVE_LINK_GROUP_ID", relativeGroupId));

            return collection.FirstOrDefault();
        }
        /// <summary>
        /// 根据ID查询所有RELATIVE_LINK_GROUP
        /// </summary>
        /// <param name="groupName">RELATIVE_LINK_GROUP的Code</param>
        /// <returns>RELATIVE_LINK_GROUP集合</returns>
        public RelativeLinkGroupCollection GetRelativeLinkGroups(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return Load(p => p.AppendItem("1", "1", "="));
            }

            return Load(p => p.AppendItem("CODE_NAME", groupName, "="));
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
