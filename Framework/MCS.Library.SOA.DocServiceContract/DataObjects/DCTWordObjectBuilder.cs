using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Reflection;

namespace MCS.Library.SOA.DocServiceContract.DataObjects
{
    public class DCTWordObjectBuilder
    {
        public static DCTWordDataObject Build(object source,string tagID)
        {
            ExceptionHelper.TrueThrow(source == null, "源对象不能为空");
            DCTWordDataObject wordObj = new DCTWordDataObject();

            Type sourceType = source.GetType();

             PropertyInfo[] fieldInfos = sourceType.GetProperties();

             foreach (PropertyInfo propInfo in fieldInfos)
             {
                 if (propInfo.PropertyType.Name != typeof(object).Name)
                 {
                     DCTSimpleProperty simpleProp = ConvertToSimpleProperty(propInfo, source,"");
                     if (simpleProp != null)
                         wordObj.PropertyCollection.Add(simpleProp);
                 }
             }
            return wordObj;
        }

        private static DCTSimpleProperty ConvertToSimpleProperty(PropertyInfo propInfo,object source,string tagID)
        {
            ExceptionHelper.TrueThrow(propInfo == null, "DCTSimpleProperty源对象不能为空");

            DCTSimpleProperty simpleProp = null;
            var value = propInfo.GetValue(source,null);
            Type type = value.GetType();
            PropertyInfo[] propInfos = type.GetProperties();
            object[] attrs = propInfo.GetCustomAttributes(typeof(WordPropertyAttribute), true);
            
            if (attrs.Length > 0)
            {
                simpleProp = new DCTSimpleProperty();
                WordPropertyAttribute attr = (WordPropertyAttribute)attrs[0];
                simpleProp.TagID = string.IsNullOrEmpty(tagID) ? tagID : attr.TagID;
                simpleProp.FormatString = attr.FormatString;
                simpleProp.Value = propInfo.GetValue(source, null);
            }
                //WordPropertyAttribute attrs = attrs.Where(p => p is WordPropertyAttribute).ToArray();
            return simpleProp;
        }

        private static DCTComplexProperty ConvertToComplexProperty(PropertyInfo propInfo, object source)
        {
            DCTComplexProperty complexProp = new DCTComplexProperty();
            foreach (PropertyInfo item in propInfo.GetType().GetProperties())
	        {

	        }
            return complexProp;
        }
    }
}