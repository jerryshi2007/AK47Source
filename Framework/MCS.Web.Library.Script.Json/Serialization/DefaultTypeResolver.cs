using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace MCS.Web.Library.Script
{
    /// <summary>
    /// ����Ĭ������JSON���л�������������������������޷�������������ʹ��Ĭ��������
    /// </summary>
	/// <remarks>����Ĭ������JSON���л�������������������������޷�������������ʹ��Ĭ��������</remarks>
    public class DefaultTypeResolver : JavaScriptTypeResolver
    {
        private Type _DefaultType;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="defaultType">��������Ĭ�����</param>
		/// <remarks>���캯��</remarks>
        public DefaultTypeResolver(Type defaultType)
        {
            _DefaultType = defaultType;
        }

        /// <summary>
        /// ͨ�����id���������
        /// </summary>
        /// <param name="id">���id</param>
        /// <returns>���</returns>
		/// <remarks>ͨ�����id���������</remarks>
        public override Type ResolveType(string id)
        {
            Type t = Type.GetType(id, false);

            return t ?? _DefaultType;
        }

        /// <summary>
        /// ͨ�������������id
        /// </summary>
        /// <param name="type">���</param>
        /// <returns>���id</returns>
		/// <remarks>ͨ�������������id</remarks>
        public override string ResolveTypeId(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.AssemblyQualifiedName;
        }
    }
}
