using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace MCS.Library.Validation
{
	/// <summary>
	/// ֵ���ʵĻ���
	/// </summary>
    public abstract class ValueAccess
    {
		/// <summary>
		/// �õ�ֵ
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
        public abstract object GetValue(object target);
    }

    internal class PropertyValueAccess : ValueAccess
    {
        private PropertyInfo propertyInfo;

        public PropertyValueAccess(PropertyInfo pi)
        {
            this.propertyInfo = pi;
        }

        public override object GetValue(object target)
        {
            return this.propertyInfo.GetValue(target, null);
        }
    }

    internal class FieldValueAccess : ValueAccess
    {
        private FieldInfo fieldInfo;

        public FieldValueAccess(FieldInfo fi)
        {
            this.fieldInfo = fi;
        }

        public override object GetValue(object target)
        {
            return this.fieldInfo.GetValue(target);
        }
    }
}
