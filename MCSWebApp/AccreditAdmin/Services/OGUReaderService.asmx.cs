#region using

using System;
using System.Xml;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;

using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.SoapControl;

#endregion

namespace MCS.Applications.AccreditAdmin.Services
{
    /// <summary>
    /// OGUReadServices ��ժҪ˵����
    /// </summary>
    public class OGUReaderService : System.Web.Services.WebService
    {
        public OGUReaderService()
        {
            InitializeComponent();
        }


        #region webService�ṩ�ķ���

        #region GetOrganizationChildren
        /// <summary>
        /// ��ȡָ�������µ������Ӷ���
        /// </summary>
        /// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iLot">Ҫ���ѯ�����ݶ������ͣ��������顢��Ա����ְ����</param>
        /// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա(1����ͨ����2����ֱ��ɾ������4�����ż����߼�ɾ������8����Ա�����߼�ɾ������)</param>
        /// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
        /// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
        /// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
        /// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
        /// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
        /// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
        [WebMethod]
        public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strHideType, string strAttrs)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.GetOrganizationChildren(strOrgValues, soc, iLot, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strHideType, strAttrs);
        }

        /// <summary>
        /// ��ȡָ�������µ������Ӷ���
        /// </summary>
        /// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iLot">Ҫ���ѯ�����ݶ������ͣ��������顢��Ա����ְ����</param>
        /// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա(1����ͨ����2����ֱ��ɾ������4�����ż����߼�ɾ������8����Ա�����߼�ɾ������)</param>
        /// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
        /// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
        /// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
        /// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
        /// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, iLot, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ�������µ������Ӷ���
        /// </summary>
        /// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա(1����ͨ����2����ֱ��ɾ������4�����ż����߼�ɾ������8����Ա�����߼�ɾ������)</param>
        /// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
        /// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
        /// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
        /// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
        /// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strAttrs);			
        //		}

        /// <summary>
        /// ��ȡָ�������µ������Ӷ���
        /// </summary>
        /// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
        /// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
        /// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
        /// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
        /// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, iDepth, strOrgRankCodeName, strUserRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ�������µ������Ӷ���
        /// </summary>
        /// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
        /// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
        /// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
        /// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, string strOrgRankCodeName, string strUserRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, strOrgRankCodeName, strUserRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ�������µ������Ӷ���
        /// </summary>
        /// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
        /// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
        /// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, string strUserRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, strUserRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ�������µ������Ӷ���
        /// </summary>
        /// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
        /// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgValues, int iSoc, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetOrganizationChildren(strOrgValues, soc, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ�������µ������Ӷ���
        /// </summary>
        /// <param name="strOrgGuids">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
        /// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
        /// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgGuids, string strAttrs)
        //		{
        //			return OGUReader.GetOrganizationChildren(strOrgGuids, strAttrs);		
        //		}

        /// <summary>
        /// ��ȡָ�������µ������Ӷ���
        /// </summary>
        /// <param name="strOrgGuids">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
        /// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
        //		[WebMethod]
        //		public DataSet GetOrganizationChildren(string strOrgGuids)
        //		{
        //			return OGUReader.GetOrganizationChildren(strOrgGuids);
        //		}

        #endregion

        #region IsUserInObjects
        /// <summary>
        /// �ж�һ���û��Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="strUserValue">�û�����������ֵ</param>
        /// <param name="socu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
        /// <param name="objectXmlDoc">�ж϶������������ֵ</param>
        /// <param name="soco">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա(1����ͨ����2����ֱ��ɾ������4�����ż����߼�ɾ������8����Ա�����߼�ɾ������)</param>
        /// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
        /// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
        /// <param name="bFitAll">�Ƿ�Ҫ����ȫƥ�䣨������ָ����ÿһ�������У�</param>
        /// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
        [WebMethod]
        public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco, int iLod, string strHideType, bool bDirect, bool bFitAll)
        {
            SearchObjectColumn socu = (SearchObjectColumn)iSocu;
            SearchObjectColumn soco = (SearchObjectColumn)iSoco;
            ListObjectDelete lod = (ListObjectDelete)iLod;
            return OGUReader.IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, lod, strHideType, bDirect, bFitAll);
        }

        /// <summary>
        /// �ж�һ���û��Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="strUserValue">�û�����������ֵ</param>
        /// <param name="socu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
        /// <param name="objectXmlDoc">�ж϶������������ֵ</param>
        /// <param name="soco">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա(1����ͨ����2����ֱ��ɾ������4�����ż����߼�ɾ������8����Ա�����߼�ɾ������)</param>
        /// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
        /// <param name="bFitAll">�Ƿ�Ҫ����ȫƥ�䣨������ָ����ÿһ�������У�</param>
        /// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco, int iLod, bool bDirect, bool bFitAll)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			ListObjectDelete lod = (ListObjectDelete)iLod;
        //			return OGUReader.IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, lod, bDirect, bFitAll);
        //		}

        /// <summary>
        /// �ж�һ���û��Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="strUserValue">�û�����������ֵ</param>
        /// <param name="socu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
        /// <param name="objectXmlDoc">�ж϶������������ֵ</param>
        /// <param name="soco">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
        /// <param name="bFitAll">�Ƿ�Ҫ����ȫƥ�䣨������ָ����ÿһ�������У�</param>
        /// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco, bool bDirect, bool bFitAll)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, bDirect, bFitAll);
        //		}

        /// <summary>
        /// �ж�һ���û��Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="strUserValue">�û�����������ֵ</param>
        /// <param name="socu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
        /// <param name="objectXmlDoc">�ж϶������������ֵ</param>
        /// <param name="soco">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="bFitAll">�Ƿ�Ҫ����ȫƥ�䣨������ָ����ÿһ�������У�</param>
        /// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco, bool bFitAll)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, bFitAll);
        //		}

        /// <summary>
        /// �ж�һ���û��Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="strUserValue">�û�����������ֵ</param>
        /// <param name="socu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
        /// <param name="objectXmlDoc">�ж϶������������ֵ</param>
        /// <param name="soco">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserValue, int iSocu, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco);
        //		}

        /// <summary>
        /// �ж�һ���û��Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="strUserGuid">�û�����������ֵ</param>
        /// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
        /// <param name="objectXmlDoc">�ж϶������������ֵ</param>
        /// <param name="soco">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserGuid, string strUserParentGuid, XmlDocument objectXmlDoc, int iSoco)
        //		{
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.IsUserInObjects(strUserGuid, strUserParentGuid, objectXmlDoc, soco);
        //		}

        /// <summary>
        /// �ж�һ���û��Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="strUserGuid">�û�����������ֵ</param>
        /// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
        /// <param name="objectXmlDoc">�ж϶������������ֵ</param>
        /// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserGuid, string strUserParentGuid, XmlDocument objectXmlDoc)
        //		{
        //			return OGUReader.IsUserInObjects(strUserGuid, strUserParentGuid, objectXmlDoc);
        //		}

        /// <summary>
        /// �ж�һ���û��Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="strUserGuid">�û�����������ֵ</param>
        /// <param name="objectXmlDoc">�ж϶������������ֵ</param>
        /// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
        //		[WebMethod]
        //		public bool IsUserInObjects(string strUserGuid, XmlDocument objectXmlDoc)
        //		{
        //			return OGUReader.IsUserInObjects(strUserGuid, objectXmlDoc);
        //		}

        #endregion

        #region CheckUserInObjects
        /// <summary>
        /// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��</param>
        /// <param name="iSocu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
        /// <param name="iSoc">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա(1����ͨ����2����ֱ��ɾ������4�����ż����߼�ɾ������8����Ա�����߼�ɾ������)</param>
        /// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
        /// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        [WebMethod]
        public XmlDocument CheckUserInObjects(XmlDocument xmlUserDoc, int iSocu, XmlDocument xmlObjDoc, int iSoc, int iLod, string strHideType, bool bDirect)
        {
            SearchObjectColumn socu = (SearchObjectColumn)iSocu;
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            ListObjectDelete lod = (ListObjectDelete)iLod;
            OGUReader.CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, lod, strHideType, bDirect);

            return xmlObjDoc;
        }

        /// <summary>
        /// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��</param>
        /// <param name="iSocu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
        /// <param name="iSoc">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա(1����ͨ����2����ֱ��ɾ������4�����ż����߼�ɾ������8����Ա�����߼�ɾ������)</param>
        /// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        //		[WebMethod]
        //		public void CheckUserInObjects(XmlDocument xmlUserDoc, int iSocu, XmlDocument xmlObjDoc, int iSoc, int iLod, bool bDirect)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			ListObjectDelete lod = (ListObjectDelete)iLod;
        //			OGUReader.CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, lod, bDirect);
        //		}

        /// <summary>
        /// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��</param>
        /// <param name="iSocu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
        /// <param name="iSoc">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        //		[WebMethod]
        //		public void CheckUserInObjects(XmlDocument xmlUserDoc, int iSocu, XmlDocument xmlObjDoc, int iSoc, bool bDirect)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			OGUReader.CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, bDirect);
        //		}

        /// <summary>
        /// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��</param>
        /// <param name="iSocu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
        /// <param name="iSoc">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        //		[WebMethod]
        //		public void CheckUserInObjects(XmlDocument xmlUserDoc, int iSocu, XmlDocument xmlObjDoc, int iSoc)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			OGUReader.CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc);
        //		}

        /// <summary>
        /// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��</param>
        /// <param name="iSocu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        //		[WebMethod]
        //		public void CheckUserInObjects(XmlDocument xmlUserDoc, int iSocu, XmlDocument xmlObjDoc)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			OGUReader.CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc);
        //		}

        /// <summary>
        /// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
        /// </summary>
        /// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��</param>
        /// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
        /// <remarks>
        /// <code>
        /// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
        ///		<USERS>
        ///			<USERS oValue="" parentGuid="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</USERS>
        /// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
        /// <OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</ORGANIZATIONS>
        ///			<GROUPS oValue="" rankCode="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///				<USERS oValue="" parentGuid=""/>
        ///			</GROUPS>
        ///			<USERS oValue="" parentGuid="" >
        ///				<USERS oValue="" parentGuid=""/>
        ///			</USERS>
        ///		</OBJECTS>
        /// </remarks>
        //		[WebMethod]
        //		public void CheckUserInObjects(XmlDocument xmlUserDoc, XmlDocument xmlObjDoc)
        //		{
        //			OGUReader.CheckUserInObjects(xmlUserDoc, xmlObjDoc);
        //		}

        #endregion

        #region GetAllUsersInAllObjects
        /// <summary>
        /// ��ȡָ�������е������û�����
        /// </summary>
        /// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
        /// <param name="iSoco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iLot">Ҫ�󱻲�ѯ�����ݶ������ͣ���Ҫ�����ڱ���Ƿ�Ҫ���ѯ��ְ��Ա1��������2����Ա�飻4���û���8����ְ�û���</param>
        /// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա(1����ͨ����2����ֱ��ɾ������4�����ż����߼�ɾ������8����Ա�����߼�ɾ������)</param>
        /// <param name="strHideType">Ҫ�����ص���������</param>
        /// <param name="strAttrs">Ҫ���õ���������</param>
        /// <returns>��ȡָ�������е������û�����</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc�Ľṹ��
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        [WebMethod]
        public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, int iSoc, string strOrgLimitValues, int iSoco, int iLot, int iLod, string strHideType, string strAttrs)
        {
            SearchObjectColumn soco = (SearchObjectColumn)iSoco;
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            ListObjectType lot = (ListObjectType)iLot;
            ListObjectDelete lod = (ListObjectDelete)iLod;
            return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, lot, lod, strHideType, strAttrs);
        }

        /// <summary>
        /// ��ȡָ�������е������û�����
        /// </summary>
        /// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
        /// <param name="iSoco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iLot">Ҫ�󱻲�ѯ�����ݶ������ͣ���Ҫ�����ڱ���Ƿ�Ҫ���ѯ��ְ��Ա1��������2����Ա�飻4���û���8����ְ�û���</param>
        /// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա(1����ͨ����2����ֱ��ɾ������4�����ż����߼�ɾ������8����Ա�����߼�ɾ������)</param>
        /// <param name="strAttrs">Ҫ���õ���������</param>
        /// <returns>��ȡָ�������е������û�����</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc�Ľṹ��
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, int iSoc, string strOrgLimitValues, int iSoco, int iLot, int iLod, string strAttrs)
        //		{
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			ListObjectType lot = (ListObjectType)iLot;
        //			ListObjectDelete lod = (ListObjectDelete)iLod;
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, lot, lod, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ�������е������û�����
        /// </summary>
        /// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
        /// <param name="iSoco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա(1����ͨ����2����ֱ��ɾ������4�����ż����߼�ɾ������8����Ա�����߼�ɾ������)</param>
        /// <param name="strAttrs">Ҫ���õ���������</param>
        /// <returns>��ȡָ�������е������û�����</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc�Ľṹ��
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, int iSoc, string strOrgLimitValues, int iSoco, int iLod, string strAttrs)
        //		{
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			ListObjectDelete lod = (ListObjectDelete)iLod;
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, lod, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ�������е������û�����
        /// </summary>
        /// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
        /// <param name="iSoco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strAttrs">Ҫ���õ���������</param>
        /// <returns>��ȡָ�������е������û�����</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc�Ľṹ��
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, int iSoc, string strOrgLimitValues, int iSoco, string strAttrs)
        //		{
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ�������е������û�����
        /// </summary>
        /// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
        /// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
        /// <param name="iSoco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strAttrs">Ҫ���õ���������</param>
        /// <returns>��ȡָ�������е������û�����</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc�Ľṹ��
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strOrgLimitValues, int iSoco, string strAttrs)
        //		{
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, strOrgLimitValues, soco, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ�������е������û�����
        /// </summary>
        /// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
        /// <param name="strOrgLimitGuids">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
        /// <param name="strAttrs">Ҫ���õ���������</param>
        /// <returns>��ȡָ�������е������û�����</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc�Ľṹ��
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strOrgLimitGuids, string strAttrs)
        //		{
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, strOrgLimitGuids, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ�������е������û�����
        /// </summary>
        /// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
        /// <param name="strAttrs">Ҫ���õ���������</param>
        /// <returns>��ȡָ�������е������û�����</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc�Ľṹ��
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strAttrs)
        //		{
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ�������е������û�����
        /// </summary>
        /// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
        /// <returns>��ȡָ�������е������û�����</returns>
        /// <remarks>
        /// <code>
        /// xmlObjDoc�Ľṹ��
        ///		<OBJECTS>
        ///			<ORGANIZATIONS oValue="" rankCode="" />
        ///			<GROUPS oValue="" rankCode="" />
        ///			<USERS oValue="" parentGuid="" />
        ///		</OBJECTS>
        /// </code>
        /// </remarks>
        //		[WebMethod]
        //		public DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc)
        //		{
        //			return OGUReader.GetAllUsersInAllObjects(xmlObjDoc);
        //		}

        #endregion

        #region GetObjectsDetail
        /// <summary>
        /// ��ȡָ���������ϸ��������
        /// </summary>
        /// <param name="strObjType">Ҫ���ѯ���������(����Ϊ�գ��漰�����ѯ)</param>
        /// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strParentValues">�������ڵĻ�����ʶ���ڶ���Ϊ��Ա�����ʱ����Ч��һ�㶼Ϊ�գ�</param>
        /// <param name="iSoco">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strExtAttrs">����Ҫ����չ���ԣ���������strObjTypeΪ��ʱ��</param>
        /// <returns>����ԵĲ�ѯ�������</returns>
        [WebMethod]
        public DataSet GetObjectsDetail(string strObjType, string strObjValues, int iSoc, string strParentValues, int iSoco, string strExtAttrs)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            SearchObjectColumn soco = (SearchObjectColumn)iSoco;
            return OGUReader.GetObjectsDetail(strObjType, strObjValues, soc, strParentValues, soco, strExtAttrs);
        }

        /// <summary>
        /// ��ȡָ���������ϸ��������
        /// </summary>
        /// <param name="strObjType">Ҫ���ѯ���������(����Ϊ�գ��漰�����ѯ)</param>
        /// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strParentValues">�������ڵĻ�����ʶ���ڶ���Ϊ��Ա�����ʱ����Ч��һ�㶼Ϊ�գ�</param>
        /// <param name="iSoco">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <returns>����ԵĲ�ѯ�������</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjType, string strObjValues, int iSoc, string strParentValues, int iSoco)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetObjectsDetail(strObjType, strObjValues, soc, strParentValues, soco);
        //		}

        /// <summary>
        /// ��ȡָ���������ϸ��������
        /// </summary>
        /// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strParentValues">�������ڵĻ�����ʶ���ڶ���Ϊ��Ա�����ʱ����Ч��һ�㶼Ϊ�գ�</param>
        /// <param name="iSoco">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strExtAttrs">����Ҫ����չ���ԣ���������strObjTypeΪ��ʱ��</param>
        /// <returns>����ԵĲ�ѯ�������</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjValues, int iSoc, string strParentValues, int iSoco)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetObjectsDetail(strObjValues, soc, strParentValues, soco);
        //		}

        /// <summary>
        /// ��ȡָ���������ϸ��������
        /// </summary>
        /// <param name="strObjType">Ҫ���ѯ���������(����Ϊ�գ��漰�����ѯ)</param>
        /// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <returns>����ԵĲ�ѯ�������</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjType, string strObjValues, int iSoc)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectsDetail(strObjType, strObjValues, soc);
        //		}

        /// <summary>
        /// ��ȡָ���������ϸ��������
        /// </summary>
        /// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <returns>����ԵĲ�ѯ�������</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjValues, int iSoc)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectsDetail(strObjValues, soc);
        //		}

        /// <summary>
        /// ��ȡָ���������ϸ��������
        /// </summary>
        /// <param name="strObjType">Ҫ���ѯ���������(����Ϊ�գ��漰�����ѯ)</param>
        /// <param name="strObjGuids">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
        /// <returns>����ԵĲ�ѯ�������</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjType, string strObjGuids)
        //		{
        //			return OGUReader.GetObjectsDetail(strObjType, strObjGuids);
        //		}

        /// <summary>
        /// ��ȡָ���������ϸ��������
        /// </summary>
        /// <param name="strObjGuids">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
        /// <returns>����ԵĲ�ѯ�������</returns>
        //		[WebMethod]
        //		public DataSet GetObjectsDetail(string strObjGuids)
        //		{
        //			return OGUReader.GetObjectsDetail(strObjGuids);
        //		}

        #endregion

        #region GetRankDefine
        /// <summary>
        /// ��ȡ�������������������
        /// </summary>
        /// <param name="iObjType">��ѯ����������Ϣ�ϵ����(1����������2����Ա����)</param>
        /// <param name="iShowHidden">�Ƿ�չ��ϵͳ�е����ظ��˼�����Ϣ����Щ������Ϣ�ǲ�����չ�ֵģ�Ĭ�������Ϊ0��</param>
        /// <returns>��ȡ�������������������</returns>
        [WebMethod]
        public DataSet GetRankDefine(int iObjType, int iShowHidden)
        {
            return OGUReader.GetRankDefine(iObjType, iShowHidden);
        }

        /// <summary>
        /// ��ȡ�������������������
        /// </summary>
        /// <param name="iObjType">��ѯ����������Ϣ�ϵ����(1����������2����Ա����)</param>
        /// <returns>��ȡ�������������������</returns>
        //		[WebMethod]
        //		public DataSet GetRankDefine(int iObjType)
        //		{
        //			return OGUReader.GetRankDefine(iObjType);
        //		}

        #endregion

        #region QueryOGUByCondition
        /// <summary>
        /// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
        /// </summary>
        /// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
        /// <param name="bLike">�Ƿ����ģ��ƥ��</param>
        /// <param name="bFirstPerson">Ҫ��һ�������</param>
        /// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
        /// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
        /// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
        /// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
        /// <param name="iDep">��ѯ���</param>
        /// <param name="strHideType">Ҫ�����ε���������</param>
        /// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
        [WebMethod]
        public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, bool bLike, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType, int iDep, string strHideType)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, bLike, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep, strHideType);
        }

        //2009-05-06 ɾ�� RANK_DEFINE Լ��������ORIGINAL_SORTԼ�������ӷ�����������
        [WebMethod]
        public DataSet QueryOGUByCondition2(string strOrgValues, int iSoc, string strLikeName, bool bLike, string strAttr, int iListObjType, int iDep, string strHideType, int rtnRowLimit)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.QueryOGUByCondition2(strOrgValues, soc, strLikeName, bLike, strAttr, iListObjType, ListObjectDelete.COMMON, iDep, strHideType, rtnRowLimit);
        }

		[WebMethod]
		public DataSet QueryOGUByCondition3(string strOrgValues, int iSoc, string strLikeName, bool bLike, string strAttr, int iListObjType, int iLod, int iDep, string strHideType, int rtnRowLimit)
		{
			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
			return OGUReader.QueryOGUByCondition2(strOrgValues, soc, strLikeName, bLike, strAttr, iListObjType, (ListObjectDelete)iLod, iDep, strHideType, rtnRowLimit);
		}

        /// <summary>
        /// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
        /// </summary>
        /// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
        /// <param name="bFirstPerson">Ҫ��һ�������</param>
        /// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
        /// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
        /// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
        /// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
        /// <param name="iDep">��ѯ���</param>
        /// <param name="strHideType">Ҫ�����ε���������</param>
        /// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType, int iDep, string strHideType)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep, strHideType);
        //		}

        /// <summary>
        /// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
        /// </summary>
        /// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
        /// <param name="bFirstPerson">Ҫ��һ�������</param>
        /// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
        /// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
        /// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
        /// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
        /// <param name="iDep">��ѯ���</param>
        /// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType, int iDep)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep);
        //		}

        /// <summary>
        /// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
        /// </summary>
        /// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
        /// <param name="bFirstPerson">Ҫ��һ�������</param>
        /// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
        /// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
        /// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
        /// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
        /// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType);
        //		}

        /// <summary>
        /// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
        /// </summary>
        /// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
        /// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
        /// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
        /// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
        /// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
        /// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType);
        //		}

        /// <summary>
        /// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
        /// </summary>
        /// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
        /// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
        /// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
        /// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
        /// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, string strOrgRankCodeName, string strUserRankCodeName, string strAttr)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, strOrgRankCodeName, strUserRankCodeName, strAttr);
        //		}

        /// <summary>
        /// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
        /// </summary>
        /// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
        /// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
        /// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
        /// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, string strUserRankCodeName, string strAttr)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, strUserRankCodeName, strAttr);
        //		}

        /// <summary>
        /// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
        /// </summary>
        /// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
        /// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
        /// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName, string strAttr)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName, strAttr);
        //		}

        /// <summary>
        /// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
        /// </summary>
        /// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
        /// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgValues, int iSoc, string strLikeName)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.QueryOGUByCondition(strOrgValues, soc, strLikeName);
        //		}

        /// <summary>
        /// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
        /// </summary>
        /// <param name="strOrgGuids">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
        /// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
        /// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
        //		[WebMethod]
        //		public DataSet QueryOGUByCondition(string strOrgGuids, string strLikeName)
        //		{
        //			return OGUReader.QueryOGUByCondition(strOrgGuids, strLikeName);
        //		}

        #endregion

        #region GetUsersInGroups
        /// <summary>
        /// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
        /// </summary>
        /// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
        /// <param name="iSocg">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
        /// <param name="strOrgValues">Ҫ���ѯ���ݵĻ�����Χ(���֮�����","�ָ��� �յ�ʱ���ʾ�޻���Ҫ��)</param>
        /// <param name="iSoco">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strUserRankCodeName">����ԱҪ��������������</param>
        /// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
        /// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
        [WebMethod]
        public DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs, string strOrgValues, int iSoco, string strUserRankCodeName, int iLod)
        {
            SearchObjectColumn socg = (SearchObjectColumn)iSocg;
            SearchObjectColumn soco = (SearchObjectColumn)iSoco;
            return OGUReader.GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco, strUserRankCodeName, iLod);
        }

        /// <summary>
        /// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
        /// </summary>
        /// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
        /// <param name="iSocg">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
        /// <param name="strOrgValues">Ҫ���ѯ���ݵĻ�����Χ(���֮�����","�ָ��� �յ�ʱ���ʾ�޻���Ҫ��)</param>
        /// <param name="iSoco">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strUserRankCodeName">����ԱҪ��������������</param>
        /// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
        //		[WebMethod]
        //		public DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs, string strOrgValues, int iSoco, string strUserRankCodeName)
        //		{
        //			SearchObjectColumn socg = (SearchObjectColumn)iSocg;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco, strUserRankCodeName);
        //		}

        /// <summary>
        /// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
        /// </summary>
        /// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
        /// <param name="iSocg">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
        /// <param name="strOrgValues">Ҫ���ѯ���ݵĻ�����Χ(���֮�����","�ָ��� �յ�ʱ���ʾ�޻���Ҫ��)</param>
        /// <param name="iSoco">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
        //		[WebMethod]
        //		public DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs, string strOrgValues, int iSoco)
        //		{
        //			SearchObjectColumn socg = (SearchObjectColumn)iSocg;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco);
        //		}

        /// <summary>
        /// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
        /// </summary>
        /// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
        /// <param name="iSocg">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
        /// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
        //		[WebMethod]
        //		public DataSet GetUsersInGroups(string strGroupValues, int iSocg, string strAttrs)
        //		{
        //			SearchObjectColumn socg = (SearchObjectColumn)iSocg;
        //			return OGUReader.GetUsersInGroups(strGroupValues, socg, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
        /// </summary>
        /// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
        /// <param name="iSocg">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
        //		[WebMethod]
        //		public DataSet GetUsersInGroups(string strGroupValues, int iSocg)
        //		{
        //			SearchObjectColumn socg = (SearchObjectColumn)iSocg;
        //			return OGUReader.GetUsersInGroups(strGroupValues, socg);
        //		}

        /// <summary>
        /// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
        /// </summary>
        /// <param name="strGroupGuids">Ҫ���ѯ����Ա������ʶGUID�����GUID֮�����","�ָ���</param>
        /// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
        //		[WebMethod]
        //		public DataSet GetUsersInGroups(string strGroupGuids)
        //		{
        //			return OGUReader.GetUsersInGroups(strGroupGuids);
        //		}

        #endregion

        #region GetGroupsOfUsers
        /// <summary>
        /// ��ȡָ���û���������"��Ա��"����
        /// </summary>
        /// <param name="strUserValues">ָ�����û���ʶ�����֮����á�,���ָ���</param>
        /// <param name="iSocu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strParentValue">ָ�����û����ڲ��ţ����������ְ���⣩</param>
        /// <param name="iSoco">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strAttrs">��Ҫ���ȡ��������Ϣ</param>
        /// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
        /// <returns>��ȡָ����Ա������"��Ա��"���ϣ�ע�������߼�ɾ�������ݶ���</returns>
        [WebMethod]
        public DataSet GetGroupsOfUsers(string strUserValues, int iSocu, string strParentValue, int iSoco, string strAttrs, int iLod)
        {
            SearchObjectColumn socu = (SearchObjectColumn)iSocu;
            SearchObjectColumn soco = (SearchObjectColumn)iSoco;
            return OGUReader.GetGroupsOfUsers(strUserValues, socu, strParentValue, soco, strAttrs, iLod);
        }

        /// <summary>
        /// ��ȡָ���û���������"��Ա��"����
        /// </summary>
        /// <param name="strUserValues">ָ�����û���ʶ�����֮����á�,���ָ���</param>
        /// <param name="iSocu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strParentValue">ָ�����û����ڲ��ţ����������ְ���⣩</param>
        /// <param name="iSoco">��������������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strAttrs">��Ҫ���ȡ��������Ϣ</param>
        /// <returns>��ȡָ����Ա������"��Ա��"���ϣ�ע�������߼�ɾ�������ݶ���</returns>
        //		[WebMethod]
        //		public DataSet GetGroupsOfUsers(string strUserValues, int iSocu, string strParentValue, int iSoco, string strAttrs)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			SearchObjectColumn soco = (SearchObjectColumn)iSoco;
        //			return OGUReader.GetGroupsOfUsers(strUserValues, socu, strParentValue, soco, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ���û���������"��Ա��"����
        /// </summary>
        /// <param name="strUserValues">ָ�����û���ʶ�����֮����á�,���ָ���</param>
        /// <param name="iSocu">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strAttrs">��Ҫ���ȡ��������Ϣ</param>
        /// <returns>��ȡָ����Ա������"��Ա��"���ϣ�ע�������߼�ɾ�������ݶ���</returns>
        //		[WebMethod]
        //		public DataSet GetGroupsOfUsers(string strUserValues, int iSocu, string strAttrs)
        //		{
        //			SearchObjectColumn socu = (SearchObjectColumn)iSocu;
        //			return OGUReader.GetGroupsOfUsers(strUserValues, socu, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ���û���������"��Ա��"����
        /// </summary>
        /// <param name="strUserGuids">ָ�����û���ʶGUID�����GUID֮����á�,���ָ���</param>
        /// <param name="strAttrs">��Ҫ���ȡ��������Ϣ</param>
        /// <returns>��ȡָ����Ա������"��Ա��"���ϣ�ע�������߼�ɾ�������ݶ���</returns>
        //		[WebMethod]
        //		public DataSet GetGroupsOfUsers(string strUserGuids, string strAttrs)
        //		{
        //			return OGUReader.GetGroupsOfUsers(strUserGuids, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ���û���������"��Ա��"����
        /// </summary>
        /// <param name="strUserGuids">ָ�����û���ʶGUID�����GUID֮����á�,���ָ���</param>
        /// <returns>��ȡָ����Ա������"��Ա��"���ϣ�ע�������߼�ɾ�������ݶ���</returns>
        //		[WebMethod]
        //		public DataSet GetGroupsOfUsers(string strUserGuids)
        //		{
        //			return OGUReader.GetGroupsOfUsers(strUserGuids);
        //		}

        #endregion

        #region GetSecretariesOfLeaders
        /// <summary>
        /// ��ȡָ���쵼�����������˳�Ա
        /// </summary>
        /// <param name="strLeaderValues">ָ���쵼�ı�ʶ�����֮�����","�ָ���</param>
        /// <param name="iSoc">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
        /// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
        /// <returns>��ȡָ���쵼�����������˳�Ա</returns>
        [WebMethod]
        public DataSet GetSecretariesOfLeaders(string strLeaderValues, int iSoc, string strAttrs, int iLod)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.GetSecretariesOfLeaders(strLeaderValues, soc, strAttrs, iLod);
        }

        /// <summary>
        /// ��ȡָ���쵼�����������˳�Ա
        /// </summary>
        /// <param name="strLeaderValues">ָ���쵼�ı�ʶ�����֮�����","�ָ���</param>
        /// <param name="iSoc">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
        /// <returns>��ȡָ���쵼�����������˳�Ա</returns>
        //		[WebMethod]
        //		public DataSet GetSecretariesOfLeaders(string strLeaderValues, int iSoc, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetSecretariesOfLeaders(strLeaderValues, soc, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ���쵼�����������˳�Ա
        /// </summary>
        /// <param name="strLeaderGuids">ָ���쵼�ı�ʶGUID�����GUID֮�����","�ָ���</param>
        /// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
        /// <returns>��ȡָ���쵼�����������˳�Ա</returns>
        //		[WebMethod]
        //		public DataSet GetSecretariesOfLeaders(string strLeaderGuids, string strAttrs)
        //		{
        //			return OGUReader.GetSecretariesOfLeaders(strLeaderGuids, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ���쵼�����������˳�Ա
        /// </summary>
        /// <param name="strLeaderGuids">ָ���쵼�ı�ʶGUID�����GUID֮�����","�ָ���</param>
        /// <returns>��ȡָ���쵼�����������˳�Ա</returns>
        //		[WebMethod]
        //		public DataSet GetSecretariesOfLeaders(string strLeaderGuids)
        //		{
        //			return OGUReader.GetSecretariesOfLeaders(strLeaderGuids);
        //		}

        #endregion

        #region GetLeadersOfSecretaries
        /// <summary>
        /// ��ȡָ������������쵼�˳�Ա
        /// </summary>
        /// <param name="strSecValues">ָ������ı�ʶ�����֮�����","�ָ���</param>
        /// <param name="iSoc">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
        /// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
        /// <returns>��ȡָ������������쵼�˳�Ա</returns>
        [WebMethod]
        public DataSet GetLeadersOfSecretaries(string strSecValues, int iSoc, string strAttrs, int iLod)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.GetLeadersOfSecretaries(strSecValues, soc, strAttrs, iLod);
        }

        /// <summary>
        /// ��ȡָ������������쵼�˳�Ա
        /// </summary>
        /// <param name="strSecValues">ָ������ı�ʶ�����֮�����","�ָ���</param>
        /// <param name="iSoc">�û�����������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����
        /// </param>
        /// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
        /// <returns>��ȡָ������������쵼�˳�Ա</returns>
        //		[WebMethod]
        //		public DataSet GetLeadersOfSecretaries(string strSecValues, int iSoc, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetLeadersOfSecretaries(strSecValues, soc, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ������������쵼�˳�Ա
        /// </summary>
        /// <param name="strSecGuids">ָ������ı�ʶGUID�����GUID֮�����","�ָ���</param>
        /// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
        /// <returns>��ȡָ������������쵼�˳�Ա</returns>
        //		[WebMethod]
        //		public DataSet GetLeadersOfSecretaries(string strSecGuids, string strAttrs)
        //		{
        //			return OGUReader.GetLeadersOfSecretaries(strSecGuids, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ������������쵼�˳�Ա
        /// </summary>
        /// <param name="strSecGuids">ָ������ı�ʶGUID�����GUID֮�����","�ָ���</param>
        /// <returns>��ȡָ������������쵼�˳�Ա</returns>
        //		[WebMethod]
        //		public DataSet GetLeadersOfSecretaries(string strSecGuids)
        //		{
        //			return OGUReader.GetLeadersOfSecretaries(strSecGuids);
        //		}

        #endregion

        #region GetObjectParentOrgs
        /// <summary>
        /// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
        /// </summary>
        /// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
        /// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
        /// <param name="iSoc">���������е��������ͣ�0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="bOnlyDirectly">�Ƿ������ȡ��ӽ��Ļ�������</param>
        /// <param name="bWithVisiual">�Ƿ�Ҫ��������ⲿ��</param>
        /// <param name="strOrgRankCodeName">Ҫ����͵Ļ�����������</param>
        /// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
        /// <returns>��ȡָ������ĸ����Ŷ���</returns>
        [WebMethod]
        public DataSet GetObjectParentOrgs(string strObjType, string strObjValues, int iSoc, bool bOnlyDirectly, bool bWithVisiual, string strOrgRankCodeName, string strAttrs)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.GetObjectParentOrgs(strObjType, strObjValues, soc, bOnlyDirectly, bWithVisiual, strOrgRankCodeName, strAttrs);
        }

        /// <summary>
        /// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
        /// </summary>
        /// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
        /// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
        /// <param name="iSoc">���������е��������ͣ�0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="bWithVisiual">�Ƿ�Ҫ��������ⲿ��</param>
        /// <param name="strOrgRankCodeName">Ҫ����͵Ļ�����������</param>
        /// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
        /// <returns>��ȡָ������ĸ����Ŷ���</returns>
        //		[WebMethod]
        //		public DataSet GetObjectParentOrgs(string strObjType, string strObjValues, int iSoc, bool bWithVisiual, string strOrgRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectParentOrgs(strObjType, strObjValues, soc, bWithVisiual, strOrgRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
        /// </summary>
        /// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
        /// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
        /// <param name="iSoc">���������е��������ͣ�0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strOrgRankCodeName">Ҫ����͵Ļ�����������</param>
        /// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
        /// <returns>��ȡָ������ĸ����Ŷ���</returns>
        //		[WebMethod]
        //		public DataSet GetObjectParentOrgs(string strObjType, string strObjValues, int iSoc, string strOrgRankCodeName, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectParentOrgs(strObjType, strObjValues, soc, strOrgRankCodeName, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
        /// </summary>
        /// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
        /// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
        /// <param name="iSoc">���������е���������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
        /// <returns>��ȡָ������ĸ����Ŷ���</returns>
        //		[WebMethod]
        //		public DataSet GetObjectParentOrgs(string strObjType, string strObjValues, int iSoc, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectParentOrgs(strObjType, strObjValues, soc, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
        /// </summary>
        /// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
        /// <param name="strObjGuids">���������е�����ֵGUID(���GUID֮�����","�ָ�)</param>
        /// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
        /// <returns>��ȡָ������ĸ����Ŷ���</returns>
        //		[WebMethod]
        //		public DataSet GetObjectParentOrgs(string strObjType, string strObjGuids, string strAttrs)
        //		{
        //			return OGUReader.GetObjectParentOrgs(strObjType, strObjGuids, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
        /// </summary>
        /// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
        /// <param name="strObjGuids">���������е�����ֵGUID(���GUID֮�����","�ָ�)</param>
        /// <returns>��ȡָ������ĸ����Ŷ���</returns>
        //		[WebMethod]
        //		public DataSet GetObjectParentOrgs(string strObjType, string strObjGuids)
        //		{
        //			return OGUReader.GetObjectParentOrgs(strObjType, strObjGuids);
        //		}

        #endregion

        #region GetObjectDepOrgs
        /// <summary>
        /// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ�����(ָ�����)��
        /// </summary>
        /// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
        /// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
        /// <param name="iSoc">���������е���������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="iDep">Ҫ���ȡ�����</param>
        /// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
        /// <returns>��ȡָ������ĸ����Ŷ���</returns>
        [WebMethod]
        public DataSet GetObjectDepOrgs(string strObjType, string strObjValues, int iSoc, int iDep, string strAttrs)
        {
            SearchObjectColumn soc = (SearchObjectColumn)iSoc;
            return OGUReader.GetObjectDepOrgs(strObjType, strObjValues, soc, iDep, strAttrs);
        }

        /// <summary>
        /// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ�����(ָ�����)��
        /// </summary>
        /// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
        /// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
        /// <param name="iSoc">���������е���������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
        /// <returns>��ȡָ������ĸ����Ŷ���</returns>
        //		[WebMethod]
        //		public DataSet GetObjectDepOrgs(string strObjType, string strObjValues, int iSoc, string strAttrs)
        //		{
        //			SearchObjectColumn soc = (SearchObjectColumn)iSoc;
        //			return OGUReader.GetObjectDepOrgs(strObjType, strObjValues, soc, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ�����(ָ�����)��
        /// </summary>
        /// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
        /// <param name="strObjGuids">���������е�����ֵGUID(���GUID֮�����","�ָ�)</param>
        /// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
        /// <returns>��ȡָ������ĸ����Ŷ���</returns>
        //		[WebMethod]
        //		public DataSet GetObjectDepOrgs(string strObjType, string strObjGuids, string strAttrs)
        //		{
        //			return OGUReader.GetObjectDepOrgs(strObjType, strObjGuids, strAttrs);
        //		}

        /// <summary>
        /// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ�����(ָ�����)��
        /// </summary>
        /// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
        /// <param name="strObjGuids">���������е�����ֵGUID(���GUID֮�����","�ָ�)</param>
        /// <returns>��ȡָ������ĸ����Ŷ���</returns>
        //		[WebMethod]
        //		public DataSet GetObjectDepOrgs(string strObjType, string strObjGuids)
        //		{
        //			return OGUReader.GetObjectDepOrgs(strObjType, strObjGuids);
        //		}

        #endregion

        #region GetRootDSE
        /// <summary>
        /// ��ȡϵͳָ���ĸ�����
        /// </summary>
        /// <returns>��ȡϵͳָ���ĸ�����</returns>
        [WebMethod]
        public DataSet GetRootDSE()
        {
            return OGUReader.GetRootDSE();
        }
        #endregion

        #region SignInCheck
        /// <summary>
        /// ��֤��Ա��¼���������Ƿ���ȷ
        /// </summary>
        /// <param name="strLogonName">�û���¼��</param>
        /// <param name="strUserPwd">�û�����</param>
        /// <returns>��¼���������Ƿ�ƥ��</returns>
        [WebMethod]
        public bool SignInCheck(string strLogonName, string strUserPwd)
        {
            return OGUReader.SignInCheck(strLogonName, strUserPwd);
        }

        #endregion

        #region UpdateUserPwd
        /// <summary>
        /// �û��޸Ŀ���ӿ�
        /// </summary>
        /// <param name="strUserGuid">Ҫ���޸Ŀ�����û�</param>
        /// <param name="strOldPwd">�û��ľɿ���</param>
        /// <param name="strNewPwd">ʹ�õ��¿���</param>
        /// <param name="strConfirmPwd">�¿����ȷ��</param>
        /// <returns>�����޸��Ƿ�ɹ�</returns>
        /*[WebMethod]
        public bool UpdateUserPwd(string strUserGuid, string strOldPwd, string strNewPwd, string strConfirmPwd)
        {
            return UpdateUserPwd(strUserGuid, (int)SearchObjectColumn.SEARCH_GUID, strOldPwd, strNewPwd, strConfirmPwd);
        }
        // del by cgac\yuan_yong 2004-11-14
        */
        /// <summary>
        /// �û��޸Ŀ���ӿ�
        /// </summary>
        /// <param name="strUserValue">Ҫ���޸Ŀ�����û�</param>
        /// <param name="iSoc">��ѯҪ��Ĳ�ѯ������
        /// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
        /// <param name="strOldPwd">�û��ľɿ���</param>
        /// <param name="strNewPwd">ʹ�õ��¿���</param>
        /// <param name="strConfirmPwd">�¿����ȷ��</param>
        /// <returns>�����޸��Ƿ�ɹ�</returns>
        [WebMethod]
        public bool UpdateUserPwd(string strUserValue, int iSoc, string strOldPwd, string strNewPwd, string strConfirmPwd)
        {
            OGUControler oguControl = new OGUControler();

            SearchObjectColumn soc = (SearchObjectColumn)iSoc;

            return oguControl.UpdateUserPwd(strUserValue, soc, strOldPwd, strNewPwd, strConfirmPwd);
        }

        #endregion

        #region RemoveAllCache
        /// <summary>
        /// �������ݻ���
        /// </summary>
        [WebMethod]
        public void RemoveAllCache()
        {
            OGUReader.RemoveAllCache();
        }
        #endregion
        #endregion

        #region �����������ɵĴ���

        //Web ����������������
        private IContainer components = null;

        /// <summary>
        /// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
        /// �˷��������ݡ�
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// ������������ʹ�õ���Դ��
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

    }
}