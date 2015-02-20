using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// Telephone对象序列化
    /// </summary>
	internal class PhoneNumberConverter : JavaScriptConverter
    {
        /// <summary>
        ///反序列化material
        /// </summary>
        /// <param name="dictionary">对象类型</param>
        /// <param name="type">对象类型</param>
        /// <param name="serializer">JS序列化器</param>
        /// <returns>反序列化出的对象</returns>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
			PhoneNumber phonenumber = new PhoneNumber();

		    phonenumber.ID = DictionaryHelper.GetValue(dictionary, "code", Guid.NewGuid().ToString());
			phonenumber.ResourceID = DictionaryHelper.GetValue(dictionary, "resourceID",string.Empty);
			phonenumber.TelephoneClass = DictionaryHelper.GetValue(dictionary, "telephoneClass", string.Empty);
			phonenumber.StateCode = DictionaryHelper.GetValue(dictionary, "stateCode", string.Empty);
			phonenumber.AreaCode = DictionaryHelper.GetValue(dictionary, "areaCode", string.Empty);
			phonenumber.MainCode = DictionaryHelper.GetValue(dictionary, "mainCode", string.Empty);
			phonenumber.ExtCode = DictionaryHelper.GetValue(dictionary, "extCode", string.Empty);
			phonenumber.InnerSort = DictionaryHelper.GetValue(dictionary, "innerSort", 0);		
			phonenumber.IsDefault = DictionaryHelper.GetValue(dictionary, "isDefault", 0);
			phonenumber.Description = DictionaryHelper.GetValue(dictionary, "description", "");
			phonenumber.Changed = DictionaryHelper.GetValue(dictionary, "changed", false);
			phonenumber.VersionStartTime = DictionaryHelper.GetValue(dictionary, "versionStartTime",DateTime.Now);
			phonenumber.VersionEndTime = DictionaryHelper.GetValue(dictionary, "versionEndTime", ConnectionDefine.MaxVersionEndTime);

			return phonenumber;
        }

        /// <summary>
        /// 序列化telephone
        /// </summary>
        /// <param name="obj">telephone对象</param>
        /// <param name="serializer">序列化器</param>
        /// <returns>属性集合</returns>
        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

			PhoneNumber telephone = (PhoneNumber)obj;

            dictionary.Add("code", telephone.ID);
            dictionary.Add("resourceID", telephone.ResourceID);
            dictionary.Add("telephoneClass", telephone.TelephoneClass);
            dictionary.Add("stateCode", telephone.StateCode);
            dictionary.Add("areaCode", telephone.AreaCode);
            dictionary.Add("mainCode", telephone.MainCode);
            dictionary.Add("extCode", telephone.ExtCode);
			dictionary.Add("innerSort", telephone.InnerSort);
            dictionary.Add("isDefault", telephone.IsDefault);
			dictionary.Add("changed", telephone.Changed);
			dictionary.Add("description", telephone.Description);
			dictionary.Add("versionStartTime", telephone.VersionStartTime);
			dictionary.Add("versionEndTime", telephone.VersionEndTime);
            return dictionary;
        }

        /// <summary>
        /// 获取此Converter支持的类别
        /// </summary>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(PhoneNumber));

                return types;
            }
        }
    }
}
