using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security
{
    /// <summary>
    /// 表示组织和Schema对象的关系（父）
    /// </summary>
    [Serializable]
    public class SCWrappedSchemaOwnership
    {
        private SCSimpleObject schemaObject;
        private string refId;

        /// <summary>
        /// 获取父级容器的ID(在某些情况下，临时作子对象的ID)
        /// </summary>
        public string ReferenceId
        {
            get { return refId; }
        }

        /// <summary>
        /// 获取Schema对象
        /// </summary>
        public SCSimpleObject SchemaObject
        {
            get { return schemaObject; }
        }

        /// <summary>
        /// 获取Schema对象的ID
        /// </summary>
        public string ID
        {
            get { return this.schemaObject.ID; }
        }

        /// <summary>
        /// 获取对象的名称
        /// </summary>
        public string Name
        {
            get { return this.schemaObject.Name; }
        }

        /// <summary>
        /// 获取对象的显示名称
        /// </summary>
        public string DisplayName
        {
            get { return this.schemaObject.DisplayName; }
        }

        /// <summary>
        /// 获取一个<see cref="System.String"/>，表示对象的类型
        /// </summary>
        public string SchemaType
        {
            get
            {
                return this.schemaObject.SchemaType;
            }
        }

        /// <summary>
        /// 使用指定的<see cref="SCSimpleObject"/>和引用对象ID初始化<see cref="SCWrappedSchemaOwnership"/>。
        /// </summary>
        /// <param name="schemaObject"></param>
        /// <param name="refId"></param>
        public SCWrappedSchemaOwnership(SCSimpleObject schemaObject, string refId)
        {
            if (schemaObject == null)
                throw new ArgumentNullException("org");
            this.schemaObject = schemaObject;
            this.refId = refId;
        }
    }
}
