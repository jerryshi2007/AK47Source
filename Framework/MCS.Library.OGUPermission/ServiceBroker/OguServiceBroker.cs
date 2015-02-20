#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.OGUPermission
// FileName	��	OguServiceBroker.cs
// Remark	��	������Ա�����WebService����
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
namespace MCS.Library.OGUPermission
{
	using System.Diagnostics;
	using System.Web.Services;
	using System.ComponentModel;
	using System.Web.Services.Protocols;
	using System;
	using System.Xml.Serialization;
	using System.Data;

	/// <summary>
	/// ������Ա�����WebService����
	/// </summary>
	//[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Web.Services.WebServiceBindingAttribute(Name = "OGUReaderServiceSoap", Namespace = "http://tempuri.org/")]
	public sealed class OguReaderServiceBroker : System.Web.Services.Protocols.SoapHttpClientProtocol
	{
		/// <summary>
		/// ������Ա�����WebService����ʵ��
		/// </summary>
		public static OguReaderServiceBroker Instance
		{
			get
			{
				return new OguReaderServiceBroker();
			}
		}

		private OguReaderServiceBroker()
		{
			ServiceBrokerContext.Current.InitWebClientProtocol(this);
			this.Url = OguPermissionSettings.GetConfig().OguServiceAddress.ToString();
		}

		/// <summary>
		/// Web Service ��ַ��
		/// </summary>
		public new string Url
		{
			get
			{
				return base.Url;
			}
			set
			{
				base.Url = value;
			}
		}
		/// <summary>
		/// ����һ���Ĳ�ѯ������ѯϵͳ�е����ݶ���
		/// </summary>
		/// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <param name="iSoc">��ѯҪ��Ĳ�ѯ�����ƣ�0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME�����۸ò������strOrgValuesʹ�ã�����ָ��strOrgValues��Ӧ���ݿ�����ֶ����ƣ�</param>
		/// <param name="iLot">Ҫ���ѯ�����ݶ������ͣ��������顢��Ա����ְ����</param>
		/// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
		/// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
		/// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
		/// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		/// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		/// <returns>��ѯ�����</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetOrganizationChildren", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strHideType, string strAttrs)
		{
			object[] results = this.Invoke("GetOrganizationChildren", new object[] {
                        strOrgValues,
                        iSoc,
                        iLot,
                        iLod,
                        iDepth,
                        strOrgRankCodeName,
                        strUserRankCodeName,
                        strHideType,
                        strAttrs});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// �ж�һ���û��Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="strUserValue">�û�����������ֵ</param>
		/// <param name="iSocu">�û����������ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
		/// <param name="objectXmlDoc">�ж϶������������ֵ</param>
		/// <param name="iSoco">�������������ƣ��ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		/// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
		/// <param name="bFitAll">�Ƿ�Ҫ����ȫƥ�䣨������ָ����ÿһ�������У�</param>
		/// <returns>�û��Ƿ������ָ���Ķ������֮��</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IsUserInObjects", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, System.Xml.XmlNode objectXmlDoc, int iSoco, int iLod, string strHideType, bool bDirect, bool bFitAll)
		{
			object[] results = this.Invoke("IsUserInObjects", new object[] {
                        strUserValue,
                        iSocu,
                        strUserParentGuid,
                        objectXmlDoc,
                        iSoco,
                        iLod,
                        strHideType,
                        bDirect,
                        bFitAll});
			return ((bool)(results[0]));
		}

		/// <summary>
		/// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="xmlUserDoc">�û�Ⱥ��ʶ�����֮�����","�ָ���</param>
		/// <param name="iSocu">�û����������ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
		/// <param name="iSoc">�������������ƣ��ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		/// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
		/// <returns>�û�Ⱥ�Ƿ������ָ���Ķ������֮��</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CheckUserInObjects", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Xml.XmlNode CheckUserInObjects(System.Xml.XmlNode xmlUserDoc, int iSocu, System.Xml.XmlNode xmlObjDoc, int iSoc, int iLod, string strHideType, bool bDirect)
		{
			object[] results = this.Invoke("CheckUserInObjects", new object[] {
                        xmlUserDoc,
                        iSocu,
                        xmlObjDoc,
                        iSoc,
                        iLod,
                        strHideType,
                        bDirect});
			return ((System.Xml.XmlNode)(results[0]));
		}

		/// <summary>
		/// ��ȡָ�������е������û�����
		/// </summary>
		/// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
		/// <param name="iSoc">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
		/// <param name="iSoco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="iLot">Ҫ�󱻲�ѯ�����ݶ������ͣ���Ҫ�����ڱ���Ƿ�Ҫ���ѯ��ְ��Ա��</param>
		/// <param name="iLod">ϵͳ�б��߼�ɾ�������Ƿ��ѯȡ��</param>
		/// <param name="strHideType">Ҫ�����ص���������</param>
		/// <param name="strAttrs">Ҫ���õ���������</param>
		/// <returns>ָ�������е������û�����</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetAllUsersInAllObjects", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetAllUsersInAllObjects(System.Xml.XmlNode xmlObjDoc, int iSoc, string strOrgLimitValues, int iSoco, int iLot, int iLod, string strHideType, string strAttrs)
		{
			object[] results = this.Invoke("GetAllUsersInAllObjects", new object[] {
                        xmlObjDoc,
                        iSoc,
                        strOrgLimitValues,
                        iSoco,
                        iLot,
                        iLod,
                        strHideType,
                        strAttrs});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ��ȡָ���������ϸ��������
		/// </summary>
		/// <param name="strObjType">Ҫ���ѯ���������(����Ϊ�գ�������û�ϲ�ѯ)</param>
		/// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
		/// <param name="iSoc">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strParentValues">�������ڵĻ�����ʶ���ڶ���Ϊ��Ա�����ʱ����Ч��һ�㶼Ϊ�գ�</param>
		/// <param name="iSoco">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strExtAttrs">����Ҫ����չ���ԣ���������strObjTypeΪ��ʱ��</param>
		/// <returns>ָ���������ϸ��������</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetObjectsDetail", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetObjectsDetail(string strObjType, string strObjValues, int iSoc, string strParentValues, int iSoco, string strExtAttrs)
		{
			object[] results = this.Invoke("GetObjectsDetail", new object[] {
                        strObjType,
                        strObjValues,
                        iSoc,
                        strParentValues,
                        iSoco,
                        strExtAttrs});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ��ȡ�������������������
		/// </summary>
		/// <param name="iObjType">��ѯ����������Ϣ�ϵ����(1����������2����Ա����)</param>
		/// <param name="iShowHidden">�Ƿ�չ��ϵͳ�е����ظ��˼�����Ϣ����Щ������Ϣ�ǲ�����չ�ֵģ�Ĭ�������Ϊ0��</param>
		/// <returns>�������������������</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetRankDefine", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetRankDefine(int iObjType, int iShowHidden)
		{
			object[] results = this.Invoke("GetRankDefine", new object[] {
                        iObjType,
                        iShowHidden});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="iSoc">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <param name="bLike">�Ƿ����ģ��ƥ��</param>
		/// <param name="bFirstPerson">Ҫ��һ�������</param>
		/// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
		/// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		/// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
		/// <param name="iDep">��ѯ���</param>
		/// <param name="strHideType">Ҫ�����ε���������</param>
		/// <returns>��ѯ���</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/QueryOGUByCondition", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, bool bLike, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType, int iDep, string strHideType)
		{
			object[] results = this.Invoke("QueryOGUByCondition", new object[] {
                        strOrgValues,
                        iSoc,
                        strLikeName,
                        bLike,
                        bFirstPerson,
                        strOrgRankCodeName,
                        strUserRankCodeName,
                        strAttr,
                        iListObjType,
                        iDep,
                        strHideType});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="iSoc">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <param name="bLike">�Ƿ����ģ��ƥ��</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		/// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
		/// <param name="iDep">��ѯ���</param>
		/// <param name="strHideType">Ҫ�����ε���������</param>
		/// <param name="rtnRowLimit">���صļ�¼��</param>
		/// <returns>��ѯ���</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/QueryOGUByCondition2", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet QueryOGUByCondition2(string strOrgValues, int iSoc, string strLikeName, bool bLike, string strAttr, int iListObjType, int iDep, string strHideType, int rtnRowLimit)
		{
			object[] results = this.Invoke("QueryOGUByCondition2", new object[] {
                        strOrgValues,
                        iSoc,
                        strLikeName,
                        bLike,
                        strAttr,
                        iListObjType,
                        iDep,
                        strHideType,
						rtnRowLimit});

			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strOrgValues"></param>
		/// <param name="iSoc"></param>
		/// <param name="strLikeName"></param>
		/// <param name="bLike"></param>
		/// <param name="strAttr"></param>
		/// <param name="iListObjType"></param>
		/// <param name="iLod"></param>
		/// <param name="iDep"></param>
		/// <param name="strHideType"></param>
		/// <param name="rtnRowLimit"></param>
		/// <returns></returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/QueryOGUByCondition3", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet QueryOGUByCondition3(string strOrgValues, int iSoc, string strLikeName, bool bLike, string strAttr, int iListObjType, int iLod, int iDep, string strHideType, int rtnRowLimit)
		{
			object[] results = this.Invoke("QueryOGUByCondition3", new object[] {
                        strOrgValues,
                        iSoc,
                        strLikeName,
                        bLike,
                        strAttr,
                        iListObjType,
						iLod,
                        iDep,
                        strHideType,
						rtnRowLimit});

			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
		/// </summary>
		/// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
		/// <param name="iSocg">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <param name="strOrgValues">Ҫ���ѯ���ݵĻ�����Χ(���֮�����","�ָ��� �յ�ʱ���ʾ�޻���Ҫ��)</param>
		/// <param name="iSoco">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strUserRankCodeName">����ԱҪ��������������</param>
		/// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		/// <returns>ָ����Ա���е����г�Ա</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetUsersInGroups", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs, string strOrgValues, int iSoco, string strUserRankCodeName, int iLod)
		{
			object[] results = this.Invoke("GetUsersInGroups", new object[] {
                        strGroupValues,
                        iSocg,
                        strAttrs,
                        strOrgValues,
                        iSoco,
                        strUserRankCodeName,
                        iLod});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ��ȡָ���û���������"��Ա��"����
		/// </summary>
		/// <param name="strUserValues">ָ�����û���ʶ�����֮����á�,���ָ���</param>
		/// <param name="iSocu">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strParentValue">ָ�����û����ڲ��ţ����������ְ���⣩</param>
		/// <param name="iSoco">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strAttrs">��Ҫ���ȡ��������Ϣ</param>
		/// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		/// <returns>ָ���û���������"��Ա��"����</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetGroupsOfUsers", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetGroupsOfUsers(string strUserValues, int iSocu, string strParentValue, int iSoco, string strAttrs, int iLod)
		{
			object[] results = this.Invoke("GetGroupsOfUsers", new object[] {
                        strUserValues,
                        iSocu,
                        strParentValue,
                        iSoco,
                        strAttrs,
                        iLod});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ��ȡָ���쵼�����������˳�Ա
		/// </summary>
		/// <param name="strLeaderValues">ָ���쵼�ı�ʶ�����֮�����","�ָ���</param>
		/// <param name="iSoc">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		/// <returns>ָ���쵼�����������˳�Ա</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetSecretariesOfLeaders", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetSecretariesOfLeaders(string strLeaderValues, int iSoc, string strAttrs, int iLod)
		{
			object[] results = this.Invoke("GetSecretariesOfLeaders", new object[] {
                        strLeaderValues,
                        iSoc,
                        strAttrs,
                        iLod});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ��ȡָ������������쵼�˳�Ա
		/// </summary>
		/// <param name="strSecValues">ָ������ı�ʶ�����֮�����","�ָ���</param>
		/// <param name="iSoc">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		/// <returns>ָ������������쵼�˳�Ա</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetLeadersOfSecretaries", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetLeadersOfSecretaries(string strSecValues, int iSoc, string strAttrs, int iLod)
		{
			object[] results = this.Invoke("GetLeadersOfSecretaries", new object[] {
                        strSecValues,
                        iSoc,
                        strAttrs,
                        iLod});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjValues">���������е�����ֵ</param>
		/// <param name="iSoc">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="bOnlyDirectly">�Ƿ������ȡ��ӽ��Ļ�������</param>
		/// <param name="bWithVisiual">�Ƿ�Ҫ��������ⲿ��</param>
		/// <param name="strOrgRankCodeName">Ҫ����͵Ļ�����������</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>ָ������ĸ���������</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetObjectParentOrgs", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetObjectParentOrgs(string strObjType, string strObjValues, int iSoc, bool bOnlyDirectly, bool bWithVisiual, string strOrgRankCodeName, string strAttrs)
		{
			object[] results = this.Invoke("GetObjectParentOrgs", new object[] {
                        strObjType,
                        strObjValues,
                        iSoc,
                        bOnlyDirectly,
                        bWithVisiual,
                        strOrgRankCodeName,
                        strAttrs});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ�����(ָ�����)��
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjValues">���������е�����ֵ</param>
		/// <param name="iSoc">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="iDep">Ҫ���ȡ�����</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>ָ������ĸ���������</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetObjectDepOrgs", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetObjectDepOrgs(string strObjType, string strObjValues, int iSoc, int iDep, string strAttrs)
		{
			object[] results = this.Invoke("GetObjectDepOrgs", new object[] {
                        strObjType,
                        strObjValues,
                        iSoc,
                        iDep,
                        strAttrs});
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ��ȡϵͳָ���ĸ����Ŷ���
		/// </summary>
		/// <returns>ϵͳָ���ĸ����Ŷ���</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetRootDSE", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public System.Data.DataSet GetRootDSE()
		{
			object[] results = this.Invoke("GetRootDSE", new object[0]);
			return ((System.Data.DataSet)(results[0]));
		}

		/// <summary>
		/// ��¼��
		/// </summary>
		/// <param name="strLogonName">��¼���ơ�</param>
		/// <param name="strUserPwd">���롣</param>
		/// <returns>��¼�Ƿ�ɹ���</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SignInCheck", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool SignInCheck(string strLogonName, string strUserPwd)
		{
			object[] results = this.Invoke("SignInCheck", new object[] {
                        strLogonName,
                        strUserPwd});
			return ((bool)(results[0]));
		}

		/// <summary>
		/// �޸����롣
		/// </summary>
		/// <param name="strUserValue">ָ�����û���ʶ</param>
		/// <param name="iSoc">��ѯҪ��Ĳ�ѯ�����ƣ��ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strOldPwd">������</param>
		/// <param name="strNewPwd">������</param>
		/// <param name="strConfirmPwd">ȷ������</param>
		/// <returns>�Ƿ��޸ĳɹ�</returns>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/UpdateUserPwd", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public bool UpdateUserPwd(string strUserValue, int iSoc, string strOldPwd, string strNewPwd, string strConfirmPwd)
		{
			object[] results = this.Invoke("UpdateUserPwd", new object[] {
                        strUserValue,
                        iSoc,
                        strOldPwd,
                        strNewPwd,
                        strConfirmPwd});
			return ((bool)(results[0]));
		}

		/// <summary>
		/// ȥ�����л��档
		/// </summary>
		[ServiceBrokerExtension]
		[System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/RemoveAllCache", RequestNamespace = "http://tempuri.org/", ResponseNamespace = "http://tempuri.org/", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
		public void RemoveAllCache()
		{
			this.Invoke("RemoveAllCache", new object[0]);
		}
	}
}
