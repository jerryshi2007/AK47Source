using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Data.Mapping
{
    /// <summary>
    /// �����Ӷ������͵�����
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SubClassTypeAttribute : System.Attribute
    {
        private System.Type type = null;
        private string typeDescription = null;

        /// <summary>
        /// ���췽����ͨ��������Ϣ����
        /// </summary>
        /// <param name="type">������Ϣ</param>
        public SubClassTypeAttribute(System.Type type)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(type != null, "type");

            this.type = type;
        }

        /// <summary>
        /// ���췽����ͨ��������������
        /// </summary>
        /// <param name="typeDesp">��������</param>
        public SubClassTypeAttribute(string typeDesp)
        {
            ExceptionHelper.CheckStringIsNullOrEmpty(typeDesp, "typeDesp");
            this.typeDescription = typeDesp;
        }

        /// <summary>
        /// ������Ϣ
        /// </summary>
        public System.Type Type
        {
            get
            {
                if (this.type == null && string.IsNullOrEmpty(this.typeDescription) == false)
                    this.type = TypeCreator.GetTypeInfo(this.typeDescription);

                return this.type;
            }
        }

        /// <summary>
        /// ��������
        /// </summary>
        public string TypeDescription
        {
            get
            {
                if (string.IsNullOrEmpty(this.typeDescription) == true && this.type != null)
                    this.typeDescription = this.type.FullName + ", " + this.type.Assembly.FullName;

                return this.typeDescription;
            }
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <returns>��������</returns>
        public override string ToString()
        {
            return TypeDescription;
        }
    }
}
