#region using

using System;
using System.Xml;
using System.Data;
using System.Text;
using System.Web;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Transactions;

using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.LogAdmin;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.OguAdmin.Interfaces;
using MCS.Library.Accredit.Configuration;
using MCS.Applications.AccreditAdmin.Properties;
using MCS.Library.Accredit.AppAdmin;
using MCS.Library.Accredit.WebBase;
using MCS.Library.Accredit;
#endregion

namespace MCS.Applications.AccreditAdmin.classLib
{
	/// <summary>
	/// ������Ա����ϵͳ�е�����ά������ʵ��
	/// </summary>
	internal class OGUWriter
	{
		/// <summary>
		/// 
		/// </summary>
		public OGUWriter()
		{
		}

		#region �������ʵ��
		/// <summary>
		/// ϵͳ��Ҫ��ʵ�ֵ�Ȩ�޼�⣨��⵱ǰ�û��Ƿ����㹻��Ȩ�޶�xmlDoc�е����ݽ���xmlDoc��ָ���Ĳ�����
		/// </summary>
		/// <param name="xmlDoc">����������Ҫ����е����в�������</param>
		public static void CheckUserOperation(XmlDocument xmlDoc)
		{
			XmlElement root = xmlDoc.DocumentElement;
			string strCommandName = root.LocalName;
			bool IsCustomsAuthentication = AccreditSection.GetConfig().AccreditSettings.CustomsAuthentication;
			if (IsCustomsAuthentication)
			{
				switch (strCommandName)
				{
					case "Insert": CheckPermissionInsert(root);
						break;
					case "Update": CheckPermissionUpdate(root);
						break;
					case "logicDelete": CheckPermissionLogicDelete(root);
						break;
					case "furbishDelete": CheckPermissionFurbishDelete(root);
						break;
					case "realDelete": CheckPermissionRealDelete(root);
						break;
					case "Move": CheckPermissionMove(root);
						break;
					case "Sort": CheckPermissionSort(root);
						break;
					case "GroupSort": CheckPermissionGroupSort(root);
						break;
					case "addObjectsToGroups": CheckPermissionAddObjectsToGroups(root);
						break;
					case "delUsersFromGroups": CheckPermissionDelUsersFromGroups(root);
						break;
					case "setMainDuty": CheckPermissionSetUserMainDuty(root);
						break;
					case "ResetPassword": CheckPermissionResetPassword(root);
						break;
					case "InitPassword": CheckPermissionInitPassword(root);
						break;
					case "addSecsToLeader": CheckPermissionAddSecsToLeader(root);
						break;
					case "delSecsOfLeader": CheckPermissionDelSecsOfLeader(root);
						break;
					default: ExceptionHelper.TrueThrow(true, "ϵͳ��û��������ݴ���\"" + strCommandName + "\"�ĳ���");
						break;
				}
			}
		}

		/// <summary>
		/// �߼�ɾ��xmlDoc��ָ�����������ݶ����߼�ɾ��������վ�У�
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ��ɾ��������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///		<logicDelete>
		///			<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\����" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" 
		/// DESCRIPTION=" " RANK_CODE="POS_ORGAN_U" NAME="������" />
		///			<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\�����" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="�����" OBJ_NAME="�����" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="������" />
		///		</logicDelete>
		/// </code>
		/// </remarks>
		public static void LogicDeleteObjects(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);		

			OguDataWriter.LogicDeleteObjects(xmlDoc);		
			
			WriteLogOfDeleteObjects(xmlDoc, OGULogDefine.LOG_LOGIC_DELETE_OBJECTS, "ɾ������ϵͳ����վ��");// д��־

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// ��xmlDoc��ָ�����������ݶ���ӻ���վ�лָ���ϵͳ������ʹ��
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///	<furbishDelete>
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\����" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_ORGAN_U" NAME="������" />
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\�����" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="�����" OBJ_NAME="�����" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="������" />
		///	</furbishDelete>
		/// </code>
		/// </remarks>
		public static void FurbishDeleteObjects(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.FurbishDeleteObjects(xmlDoc);
			
			WriteLogOfDeleteObjects(xmlDoc, OGULogDefine.LOG_FURBISH_DELETE_OBJECTS, "��ϵͳ����վ�лָ�");// д��־

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// ȷ���Ĵ����ݿ��а����ݶ���ɾ��
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///	<realDelete>
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\����" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_ORGAN_U" NAME="������" />
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\�����" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="�����" OBJ_NAME="�����" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="������" />
		///	</realDelete>
		/// </code>
		/// </remarks>
		public static void RealDeleteObjects(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.RealDeleteObjects(xmlDoc);

			WriteLogOfDeleteObjects(xmlDoc, OGULogDefine.LOG_REAL_DELETE_OBJECTS, "���״�ϵͳ��ɾ��");// д��־

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// ��ϵͳ�д���һ���µ����ݶ��󣨻�������Ա����Ա�飩
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <example>
		/// xmlDoc�е��������£�
		/// <code>
		///		<Insert>
		///			<ORGANIZATIONS>
		///				<SET>
		///					<OBJ_NAME>testOrg</OBJ_NAME>
		///					<DISPLAY_NAME>testOrg</DISPLAY_NAME>
		///					<RANK_CODE>POS_DEPART_D</RANK_CODE>
		///					<ORG_CLASS>0</ORG_CLASS>
		///					<ORG_TYPE>2</ORG_TYPE>
		///					<ALL_PATH_NAME>�й�����\01��������\01�칫��\testOrg</ALL_PATH_NAME>
		///					<CREATE_TIME>2004-05-18</CREATE_TIME>
		///					<DESCRIPTION>testOrg</DESCRIPTION>
		///					<PARENT_GUID>0c6135c6-1038-48ec-a79b-ba63dffac758</PARENT_GUID>
		///				</SET>
		///			</ORGANIZATIONS>
		///		</Insert>
		/// ���ߣ����ü�ְ��
		///		<Insert opType="AddSideline">
		///			<USERS>
		///				<SET>
		///					<SIDELINE>1</SIDELINE>
		///					<LAST_NAME>��</LAST_NAME>
		///					<FIRST_NAME>��</FIRST_NAME>
		///					<OBJ_NAME>����</OBJ_NAME>
		///					<DISPLAY_NAME>����</DISPLAY_NAME>
		///					<LOGON_NAME>daijie</LOGON_NAME>
		///					<ALL_PATH_NAME>�й�����\01��������\01�칫��\���쵼\����</ALL_PATH_NAME>
		///					<RANK_CODE>SUB_MINISTRY_U</RANK_CODE>
		///					<RANK_NAME>test</RANK_NAME>
		///					<ATTRIBUTES>0</ATTRIBUTES>
		///					<CREATE_TIME>2004-05-19</CREATE_TIME>
		///					<START_TIME>2004-05-19</START_TIME>
		///					<END_TIME>2004-05-19</END_TIME>
		///					<POSTURAL>4</POSTURAL>
		///					<DESCRIPTION>test</DESCRIPTION>
		///					<PARENT_GUID>dd615a2f-efe1-4cd3-bb6a-3d5946e13156</PARENT_GUID>
		///					<USER_GUID>5592c712-ebea-471e-aea0-2661a258699a</USER_GUID>
		///				</SET>
		///			</USERS>
		///		</Insert>
		/// </code>
		/// </example>
		public static void InsertObjects(XmlDocument xmlDoc)
		{
            CheckUserOperation(xmlDoc);

            OguDataWriter.InsertObjects(xmlDoc);

            WriteLogOfInsertObjects(xmlDoc);// д��־

            OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// �޸�ϵͳ�е�һ�����ݶ��󣨻�������Ա(���ְ)����Ա�飩
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///	<Update>
		///		<USERS>
		///			<SET>
		///				<RANK_NAME>TEST</RANK_NAME>
		///				<DESCRIPTION>DDD</DESCRIPTION>
		///			</SET>
		///			<WHERE>
		///				<USER_GUID operator="=">5902aba2-ef24-4acd-9c36-71a10e25ce9c</USER_GUID>
		///				<PARENT_GUID>829624bd-61f2-4ccd-aff8-5e7a22f5ff93</PARENT_GUID>
		///			</WHERE>
		///		</USERS>
		///	</Update>
		/// </code>
		/// </remarks>
		public static void UpdateObjects(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.UpdateObjects(xmlDoc);

			WriteLogOfUpdateObjects(xmlDoc);// д��־

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// ��ָ������ĵ�¼��������ΪϵͳĬ�ϵĿ���Լ�ʹ��Ĭ�ϵļ��ܷ�ʽ��
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///		<InitPassword>
		///			<GUID>959d109a-8a08-4062-86c5-0143e0f9028e</GUID>
		///			<PARENT_GUID>237698798-8a024062-86c5-0143e0f9028e</PARENT_GUID>
		///		</InitPassword>
		/// </code>
		/// </remarks>
		public static void InitPassword(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.InitPassword(xmlDoc);

			WriteLogInitPassword(xmlDoc);// д��־
		}

		/// <summary>
		/// �û��Լ��������Լ���ʹ�õ�ϵͳ��¼��������ݼ��ܵķ�ʽ
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///		<ResetPassword>
		///			<GUID>b33b3c0c-8f28-45ef-8373-8fcccac0246f</GUID>
		///			<OldPwd>000000</OldPwd>
		///			<OldPwdType>21545d16-a62f-4a7e-ac2f-beca958e0fdf</OldPwdType>
		///			<NewPwd>000000</NewPwd>
		///			<NewPwdType>21545d16-a62f-4a7e-ac2f-beca958e0fdf</NewPwdType>
		///		</ResetPassword>
		/// </code>
		/// </remarks>
		public static void ResetPassword(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);
			OguDataWriter.ResetPassword(xmlDoc);

			//����������¼��־
		}

		/// <summary>
		/// �ƶ�ָ�������ƶ�������
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///	<Move GUID="45a67fd1-9805-4a97-a70f-79efa6ed7b16" ORIGINAL_SORT="000000000000000022000002">
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ����\������" GLOBAL_SORT="000000000000000022000003000001" 
		/// ORIGINAL_SORT="000000000000000022000003000001" DISPLAY_NAME="������" OBJ_NAME="������" LOGON_NAME="wangdonghong" 
		/// PARENT_GUID="829624bd-61f2-4ccd-aff8-5e7a22f5ff93" GUID="5902aba2-ef24-4acd-9c36-71a10e25ce9c" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="������" />
		///	</Move>
		/// </code>
		/// </remarks>
		public static void MoveObjects(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.MoveObjects(xmlDoc);			

			WriteLogMoveObjects(xmlDoc);// д��־

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// ����ϵͳ��ָ�������ݶ�����������Ŷӣ�����ϵͳ�����ݵĴ���
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///	<Sort OrgGuid="829624bd-61f2-4ccd-aff8-5e7a22f5ff93">
		///		<USERS>c70ab1da-2b7f-41eb-898b-70e375c7de41</USERS>
		///		<USERS>f82124cb-5c21-4601-a126-bf2d1761ed5a</USERS>
		///		<USERS>5902aba2-ef24-4acd-9c36-71a10e25ce9c</USERS>
		///	</Sort>
		/// </code>
		/// </remarks>
		public static void SortObjects(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.SortObjects(xmlDoc);	
		
			WriteLogSortObjects(xmlDoc);// д��־

			OGUReader.RemoveAllCache();				
		}

		/// <summary>
		/// ��Ա���е���Ա����
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <returns>��Ա���е���Ա�����Ľ��</returns>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///	<GroupSort GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" extAttr="E_MAIL,DESCRIPTION,RANK_CODE,NAME">
		///		<USERS>
		///			<USER_PARENT_GUID>65eb8160-f0fa-4f1c-8984-2649788fe1d0</USER_PARENT_GUID>
		///			<USER_GUID>1ea2d199-54e6-4a78-9282-710e455b4973</USER_GUID>
		///		</USERS>
		///		<USERS>
		///			<USER_PARENT_GUID>65eb8160-f0fa-4f1c-8984-2649788fe1d0</USER_PARENT_GUID>
		///			<USER_GUID>5239c6a9-59bf-4965-aa51-f2712e6aa577</USER_GUID>
		///		</USERS>
		///		<USERS>
		///			<USER_PARENT_GUID>65eb8160-f0fa-4f1c-8984-2649788fe1d0</USER_PARENT_GUID>
		///			<USER_GUID>5592c712-ebea-471e-aea0-2661a258699a</USER_GUID>
		///		</USERS>
		///	</GroupSort>
		/// </code>
		/// </remarks>
		public static XmlDocument GroupSortObjects(XmlDocument xmlDoc)
		{
			XmlDocument result;

			CheckUserOperation(xmlDoc);

			result = OguDataWriter.GroupSortObjects(xmlDoc);

			WriteLogGroupSortObjects(xmlDoc);// д��־

			OGUReader.RemoveAllCache();		
			
			return result;
		}

		/// <summary>
		/// ���õ�ǰ��Ա�ĵ�ǰְλΪ��Ҫְλ������ְλΪ��ְְλ��
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///		<setMainDuty>
		///			<MainDuty>
		///				<USER_GUID>5e3aa542-29c3-40b5-b4cc-617045223c22</USER_GUID>
		///				<PARENT_GUID>5b23a9ad-645f-43c1-b969-cfc9fa0c16d7</PARENT_GUID>
		///			</MainDuty>
		///		</setMainDuty>
		/// </code>
		/// </remarks>
		public static void SetUserMainDuty(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.SetUserMainDuty(xmlDoc);

			WriteLogSetUserMainDuty(xmlDoc);// д��־

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// ɾ��ָ����Ա���е�ָ���û�
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///		<delUsersFromGroups GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef">
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\Ĳ����" GLOBAL_SORT="000000000000000000000000" 
		/// ORIGINAL_SORT="000000000000000000000000" DISPLAY_NAME="Ĳ����" OBJ_NAME="Ĳ����" LOGON_NAME="muxinsh" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_MINISTRY_U" NAME="��������" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\����" GLOBAL_SORT="000000000000000000000005" 
		/// ORIGINAL_SORT="000000000000000000000005" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="zhenpu" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="6d41eeb3-d229-4b58-8753-f1e2a42a5104" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_MINISTRY_U" NAME="��������" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\Ҷ��" GLOBAL_SORT="000000000000000000000006" 
		/// ORIGINAL_SORT="000000000000000000000006" DISPLAY_NAME="Ҷ��" OBJ_NAME="Ҷ��" LOGON_NAME="yejian" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="9b22f19b-0815-4d39-8315-a6b4cd09505f" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="���м���" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME="tyty" 
		/// STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\����" GLOBAL_SORT="000000000000000000000004" 
		/// ORIGINAL_SORT="000000000000000000000004" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="zhaorong" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="959d109a-8a08-4062-86c5-0143e0f9028e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_MINISTRY_U" NAME="��������" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\���ѫ" GLOBAL_SORT="000000000000000000000008" 
		/// ORIGINAL_SORT="000000000000000000000008" DISPLAY_NAME="���ѫ" OBJ_NAME="���ѫ" LOGON_NAME="yangguoxun" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="5e3aa542-29c3-40b5-b4cc-617045223c22" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="���м���" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\��ľ��" GLOBAL_SORT="000000000000000000000007" 
		/// ORIGINAL_SORT="000000000000000000000007" DISPLAY_NAME="��ľ��" OBJ_NAME="��ľ��" LOGON_NAME="duanmujun" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="b79d63c8-5bc4-44be-b099-b7cac2202a67" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="���м���" />
		///		</delUsersFromGroups>
		/// </code>
		/// </remarks>
		public static void DelUsersFromGroups(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.DelUsersFromGroups(xmlDoc);
		
			WriteLogDelUsersFromGroups(xmlDoc);// д��־

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// ��ָ�����󣨻����е��ˡ���Ա���е��ˡ��������ˣ����뵽ָ���ġ���Ա���С�
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <returns>��ָ�����󣨻����е��ˡ���Ա���е��ˡ��������ˣ����뵽ָ���ġ���Ա���С���Ľ��</returns>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///	<addObjectsToGroups USER_ACCESS_LEVEL="" GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" extAttr="E_MAIL,DESCRIPTION,RANK_CODE,NAME">
		///		<object OBJECTCLASS="ORGANIZATIONS" SIDELINE="" POSTURAL="" RANK_NAME="" STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\01�칫��\���쵼" GLOBAL_SORT="000000000000000001000000" ORIGINAL_SORT="000000000000000001000000" 
		/// DISPLAY_NAME="���쵼" OBJ_NAME="���쵼" LOGON_NAME="" PARENT_GUID="0c6135c6-1038-48ec-a79b-ba63dffac758" 
		/// GUID="dd615a2f-efe1-4cd3-bb6a-3d5946e13156" />
		///	</addObjectsToGroups>
		/// </code>
		/// </remarks>
		public static XmlDocument AddObjectsToGroups(XmlDocument xmlDoc)
		{
			XmlDocument result;

			CheckUserOperation(xmlDoc);

			result = OguDataWriter.AddObjectsToGroups(xmlDoc);

			WriteLogAddObjectsToGroups(xmlDoc); // д��־

			OGUReader.RemoveAllCache();

			return result;
		}

		/// <summary>
		/// �����쵼������֮��Ĺ�ϵ
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <returns>�����쵼������֮��Ĺ�ϵ</returns>
		/// <remarks> 
		/// xmlDoc�е��������£�
		/// <code>
		///		<addSecsToLeader GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf" USER_ACCESS_LEVEL="" extAttr="E_MAIL,DESCRIPTION,RANK_CODE,NAME" START_TIME="2004-05-19" END_TIME="2004-5-31">
		///			<object OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" ALL_PATH_NAME="�й�����\01��������\01�칫��\�𳤰칫��\����" GLOBAL_SORT="000000000000000001000001000000" ORIGINAL_SORT="000000000000000001000001000000" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="fankun" PARENT_GUID="8fce3073-5569-4bdd-847b-30d5be138e7f" GUID="c5e5065f-68e4-4764-a058-99c7e23ea208" />
		///		</addSecsToLeader>
		/// </code>
		/// </remarks>
		public static XmlDocument SetSecsToLeader(XmlDocument xmlDoc)
		{
			XmlDocument result;

			CheckUserOperation(xmlDoc);

			result = OguDataWriter.SetSecsToLeader(xmlDoc);

			WriteLogSetSecsToLeader(xmlDoc); // д��־

			return result;			
		}

		/// <summary>
		/// ɾ���쵼�����е����м��� ��ָ���Ķ���
		/// </summary>k
		/// <param name="xmlDoc">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݴ��������Ҫ��ʹ�õ����ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///		<delSecsOfLeader GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf">
		///			 <USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\01�칫��\���쵼\���" GLOBAL_SORT="000000000000000001000000000002" 
		/// ORIGINAL_SORT="000000000000000001000000000002" DISPLAY_NAME="���" OBJ_NAME="���" LOGON_NAME="yangchenguang" 
		/// PARENT_GUID="dd615a2f-efe1-4cd3-bb6a-3d5946e13156" GUID="46c71220-710d-4f69-bd2b-ae3b99267df6" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_OFFICE_U" NAME="���ּ�" />
		///		</delSecsOfLeader>
		/// </code>
		/// </remarks>
		public static void DelSecsOfLeader(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.DelSecsOfLeader(xmlDoc);

			WriteLogDelSecsOfLeader(xmlDoc); // д��־

			OGUReader.RemoveAllCache();		
		}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="strObjType"></param>
		///// <param name="strObjValue"></param>
		///// <param name="soc"></param>
		///// <param name="strParentGuid"></param>
		///// <param name="objsDs"></param>
		///// <returns></returns>
		//public static bool IsObjectIsIncludeInObjects(string strObjType, string strObjValue, SearchObjectColumn soc, string strParentGuid, DataSet objsDs)
		//{
		//    using (TransactionScope scope = TransactionScopeFactory.Create())
		//    {
		//        using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
		//        {
		//            DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);
		//            using (da.dBContextInfo)
		//            {
		//                da.dBContextInfo.OpenConnection();
		//                return IsObjectIsIncludeInObjects(strObjType, strObjValue, soc, strParentGuid, objsDs, da);
		//            }
		//        }
		//        scope.Complete();
		//    }
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strObjType"></param>
		/// <param name="strObjValue"></param>
		/// <param name="soc"></param>
		/// <param name="objsDs"></param>
		/// <param name="da"></param>
		/// <returns></returns>
		public static bool IsObjectIsIncludeInObjects(string strObjType,
			string strObjValue,
			SearchObjectColumn soc,
			string strParentGuid,
			DataSet objsDs)
		{
			DataSet oDs = OGUReader.GetObjectsDetail(strObjType, strObjValue, soc, strParentGuid, SearchObjectColumn.SEARCH_GUID);
			foreach (DataRow oRow in oDs.Tables[0].Rows)
			{
				string strObjAllPathName = OGUCommonDefine.DBValueToString(oRow["ALL_PATH_NAME"]);
				foreach (DataRow sRow in objsDs.Tables[0].Rows)
				{
					string[] strArry = OGUCommonDefine.DBValueToString(sRow["DESCRIPTION"]).Split(',', ';', ' ');
					foreach (string strValue in strArry)
					{
						if (strValue.Length <= strObjAllPathName.Length
							&& strValue.Length >= 0
							&& strObjAllPathName.Substring(0, strValue.Length) == strValue)
							return true;
					}
				}
			}

			return false;
		}
		#endregion

		#region �ڲ����ݴ���
		/// <summary>
		/// ��Ի����е���Ա��������ݴ���
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="userXml"></param>
		/// <param name="userXsd"></param>
		/// <param name="orgUserXml"></param>
		/// <param name="orgUsersXsd"></param>
		private static void XmlDocToUsersAndOrgUsers(XmlDocument xmlDoc,
			XmlDocument userXml,
			XmlDocument userXsd,
			XmlDocument orgUserXml,
			XmlDocument orgUsersXsd)
		{
			XmlNode oNodeSet = xmlDoc.DocumentElement.SelectSingleNode(".//SET");
			XmlNode uNodeSet = userXml.DocumentElement.SelectSingleNode(".//SET");
			XmlNode ouNodeSet = orgUserXml.DocumentElement.SelectSingleNode(".//SET");

			foreach (XmlNode oElemNode in oNodeSet.ChildNodes)
			{
				if (InnerCommon.GetXSDColumnNode(userXsd, oElemNode.LocalName) != null)
					XmlHelper.AppendNode(uNodeSet, oElemNode.LocalName, oElemNode.InnerText);

				if (InnerCommon.GetXSDColumnNode(orgUsersXsd, oElemNode.LocalName) != null)
					XmlHelper.AppendNode(ouNodeSet, oElemNode.LocalName, oElemNode.InnerText);
			}

			XmlNode wNode = xmlDoc.DocumentElement.SelectSingleNode(".//WHERE");
			if (wNode != null)
			{
				XmlNode uWNode = XmlHelper.AppendNode(userXml.DocumentElement.FirstChild, "WHERE");
				XmlHelper.AppendNode(uWNode, "GUID", wNode.SelectSingleNode("USER_GUID").InnerText);

				XmlNode ouWNode = XmlHelper.AppendNode(orgUserXml.DocumentElement.FirstChild, "WHERE");
				foreach (XmlNode wcNode in wNode.ChildNodes)
				{
					XmlHelper.AppendNode(ouWNode, wcNode.LocalName, wcNode.InnerText);
				}
			}

			if (uNodeSet.ChildNodes.Count > 0)
			{
				XmlNode mNode = XmlHelper.AppendNode(uNodeSet, "MODIFY_TIME", "GETDATE()");
				XmlHelper.AppendAttr(mNode, "type", "other");
			}

			if (ouNodeSet.ChildNodes.Count > 0)
			{
				XmlNode mNode = XmlHelper.AppendNode(ouNodeSet, "MODIFY_TIME", "GETDATE()");
				XmlHelper.AppendAttr(mNode, "type", "other");
			}
		}
		#endregion

		#region ����Ȩ�޵ļ��
		/// <summary>
		/// ϵͳ��������ݶ���Ȩ�޼�⣨�����һ���û�����������Ա�顢һ����ְ��Ա��
		/// �������ɸ�����������
		/// </summary>
		/// <param name="root">�����������ݵ����ݽڵ�</param>
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// �������Ȩ�޲���ͨ�����ڲ��ᱨ���쳣���±��β���ʧ��
		/// </remarks>
		/// <example>
		/// root�Ľڵ�����
		/// <code>
		///		<Insert>
		///			<ORGANIZATIONS>
		///				<SET>
		///					<OBJ_NAME>testOrg</OBJ_NAME>
		///					<DISPLAY_NAME>testOrg</DISPLAY_NAME>
		///					<RANK_CODE>POS_DEPART_D</RANK_CODE>
		///					<ORG_CLASS>0</ORG_CLASS>
		///					<ORG_TYPE>2</ORG_TYPE>
		///					<ALL_PATH_NAME>�й�����\01��������\01�칫��\testOrg</ALL_PATH_NAME>
		///					<CREATE_TIME>2004-05-18</CREATE_TIME>
		///					<DESCRIPTION>testOrg</DESCRIPTION>
		///					<PARENT_GUID>0c6135c6-1038-48ec-a79b-ba63dffac758</PARENT_GUID>
		///				</SET>
		///			</ORGANIZATIONS>
		///		</Insert>
		/// ���ߣ����ü�ְ��
		///		<Insert opType="AddSideline">
		///			<USERS>
		///				<SET>
		///					<SIDELINE>1</SIDELINE>
		///					<LAST_NAME>��</LAST_NAME>
		///					<FIRST_NAME>��</FIRST_NAME>
		///					<OBJ_NAME>����</OBJ_NAME>
		///					<DISPLAY_NAME>����</DISPLAY_NAME>
		///					<LOGON_NAME>daijie</LOGON_NAME>
		///					<ALL_PATH_NAME>�й�����\01��������\01�칫��\���쵼\����</ALL_PATH_NAME>
		///					<RANK_CODE>SUB_MINISTRY_U</RANK_CODE>
		///					<RANK_NAME>test</RANK_NAME>
		///					<ATTRIBUTES>0</ATTRIBUTES>
		///					<CREATE_TIME>2004-05-19</CREATE_TIME>
		///					<START_TIME>2004-05-19</START_TIME>
		///					<END_TIME>2004-05-19</END_TIME>
		///					<POSTURAL>4</POSTURAL>
		///					<DESCRIPTION>test</DESCRIPTION>
		///					<PARENT_GUID>dd615a2f-efe1-4cd3-bb6a-3d5946e13156</PARENT_GUID>
		///					<USER_GUID>5592c712-ebea-471e-aea0-2661a258699a</USER_GUID>
		///				</SET>
		///			</USERS>
		///		</Insert>
		/// </code>
		/// </example>
		private static void CheckPermissionInsert(XmlElement root)
		{
			DataSet gDs = null, oDs = null, uDs = null, sDs = null;

			foreach (XmlNode xNode in root.ChildNodes)
			{
				string strParentGuid = xNode.SelectSingleNode("SET/PARENT_GUID").InnerText;
				switch (xNode.LocalName)
				{
					case "ORGANIZATIONS":
						if (oDs == null)
							oDs = GetUserFunctionsScopes(AccreditResource.Func_CreateOrg, "�Բ�����û�д����»�����Ȩ�ޣ�");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strParentGuid, SearchObjectColumn.SEARCH_GUID, oDs),
							"�Բ�����û��Ȩ���ڸû����´����»�����");
						break;
					case "GROUPS":
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_CreateGroup, "�Բ�����û�д�������Ա���Ȩ�ޣ�");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strParentGuid, SearchObjectColumn.SEARCH_GUID, gDs),
							"�Բ�����û��Ȩ���ڸû����´�������Ա�飡");
						break;
					case "OU_USERS":
					case "USERS":
						if (root.GetAttribute("opType") != null && root.GetAttribute("opType") == "AddSideline")
						{
							if (sDs == null)
								sDs = GetUserFunctionsScopes(AccreditResource.Func_SetSideline, "�Բ�����û�������û���ְ��Ȩ�ޣ�");
							ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strParentGuid, SearchObjectColumn.SEARCH_GUID, sDs),
								"�Բ�����û��Ȩ���ڸû����������û���ְ��");
						}
						else
						{
							if (uDs == null)
								uDs = GetUserFunctionsScopes(AccreditResource.Func_CreateUser, "�Բ�����û�д������û���Ȩ�ޣ�");
							ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strParentGuid, SearchObjectColumn.SEARCH_GUID, uDs),
								"�Բ�����û��Ȩ���ڸû����´������û���");
						}
						break;
					default: ExceptionHelper.TrueThrow(true, "�Բ���û�ж�Ӧ�ڡ�" + xNode.LocalName + "������ش������");
						break;
				}

			}
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ���޸�ָ�����ݶ���
		/// ���޸��������������
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///	<Update>
		///		<USERS>
		///			<SET>
		///				<RANK_NAME>TEST</RANK_NAME>
		///				<DESCRIPTION>DDD</DESCRIPTION>
		///			</SET>
		///			<WHERE>
		///				<USER_GUID operator="=">5902aba2-ef24-4acd-9c36-71a10e25ce9c</USER_GUID>
		///				<PARENT_GUID>829624bd-61f2-4ccd-aff8-5e7a22f5ff93</PARENT_GUID>
		///			</WHERE>
		///		</USERS>
		///	</Update>
		/// </code>
		/// </remarks>
		private static void CheckPermissionUpdate(XmlElement root)
		{
			DataSet gDs = null, oDs = null, uDs = null;

			foreach (XmlNode xNode in root.ChildNodes)
			{
				switch (xNode.LocalName)
				{
					case "ORGANIZATIONS"://(�޸ķ�Χ��������)
						string strObjGuid = xNode.SelectSingleNode("WHERE/GUID").InnerText;
						if (oDs == null)
							oDs = GetUserFunctionsScopes(AccreditResource.Func_ModifyOrg, "�Բ�����û��Ȩ���޸Ļ������ԣ�");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, oDs),
							"�Բ�����û���޸ĵ�ǰ�������Ե�Ȩ�ޣ�");
						break;
					case "GROUPS"://(�޸ķ�Χ��������)
						strObjGuid = xNode.SelectSingleNode("WHERE/GUID").InnerText;
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_ModifyGroup, "�Բ�����û��Ȩ���޸���Ա�����ԣ�");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, gDs),
							"�Բ�����û���޸ĵ�ǰ��Ա������Ȩ�ޣ�");
						break;
					case "OU_USERS":
					case "USERS"://(�޸ķ�Χ��������)
						strObjGuid = xNode.SelectSingleNode("WHERE/USER_GUID").InnerText;
						string strParentGuid = xNode.SelectSingleNode("WHERE/PARENT_GUID").InnerText;
						if (uDs == null)
							uDs = GetUserFunctionsScopes(AccreditResource.Func_ModifyUser, "�Բ�����û��Ȩ���޸��û����ԣ�");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, strParentGuid, uDs),
							"�Բ�����û���޸ĵ�ǰ�û�����Ȩ�ޣ�");
						break;
					default: ExceptionHelper.TrueThrow(true, "û����ص����ݴ�����Ϣ��");
						break;
				}
			}
		}

		/// <summary>
		///  �жϵ�ǰ�û��Ƿ���Ȩ�ޡ��߼�ɾ����ָ�����ݶ���
		///  ��Ȩ���жϿ�����
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///		<logicDelete>
		///			<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\����" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_ORGAN_U" NAME="������" />
		///			<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\�����" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="�����" OBJ_NAME="�����" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="������" />
		///		</logicDelete>
		/// </code>
		/// </remarks>
		private static void CheckPermissionLogicDelete(XmlElement root)
		{
			DataSet gDs = null, oDs = null, uDs = null;

			foreach (XmlElement xNode in root.ChildNodes)
			{
				string strObjGuid = xNode.GetAttribute("GUID");
				switch (xNode.LocalName)
				{
					case "ORGANIZATIONS":
						if (oDs == null)
							oDs = GetUserFunctionsScopes(AccreditResource.Func_LogicDelOrg, "�Բ�����û��Ȩ�ްѻ����������վ��");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, oDs),
							"�Բ�����û�аѵ�ǰ�����������վ��Ȩ�ޣ�");
						break;
					case "GROUPS":
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_LogicDelGroup, "�Բ�����û��Ȩ�ް���Ա���������վ��");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, gDs),
							"�Բ�����û�аѵ�ǰ��Ա���������վ��Ȩ�ޣ�");
						break;
					case "USERS":
					case "OU_USERS":
						string strParentGuid = xNode.GetAttribute("PARENT_GUID");
						if (uDs == null)
							uDs = GetUserFunctionsScopes(AccreditResource.Func_LogicDelUser, "�Բ�����û��Ȩ�ް��û��������վ��");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								strParentGuid,
								uDs),
							"�Բ�����û�аѵ�ǰ�û��������վ��Ȩ�ޣ�");
						break;
				}
			}
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ�ޡ��ָ��߼�ɾ����ָ�����ݶ���
		///  ��Ȩ���жϿ�����
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///	<furbishDelete>
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\����" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_ORGAN_U" NAME="������" />
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\�����" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="�����" OBJ_NAME="�����" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="������" />
		///	</furbishDelete>
		/// </code>
		/// </remarks>
		private static void CheckPermissionFurbishDelete(XmlElement root)
		{
			DataSet gDs = null, oDs = null, uDs = null;

			foreach (XmlElement xNode in root.ChildNodes)
			{
				string strObjGuid = xNode.GetAttribute("GUID");
				switch (xNode.LocalName)
				{
					case "ORGANIZATIONS":
						if (oDs == null)
							oDs = GetUserFunctionsScopes(AccreditResource.Func_FurbishOrg, "�Բ�����û��Ȩ�ްѻ����ӻ���վ�лָ���");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								oDs),
							"�Բ�����û�аѵ�ǰ�����ӻ���վ�лָ���Ȩ�ޣ�");
						break;
					case "GROUPS":
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_FurbishGroup, "�Բ�����û��Ȩ�ް���Ա��ӻ���վ�лָ���");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								gDs),
							"�Բ�����û�аѵ�ǰ��Ա��ӻ���վ�лָ���Ȩ�ޣ�");
						break;
					case "USERS":
					case "OU_USERS":
						string strParentGuid = xNode.GetAttribute("PARENT_GUID");
						if (uDs == null)
							uDs = GetUserFunctionsScopes(AccreditResource.Func_FurbishUser, "�Բ�����û��Ȩ�ް��û��ӻ���վ�лָ���");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								strParentGuid,
								uDs),
							"�Բ�����û�аѵ�ǰ�û��ӻ���վ�лָ���Ȩ�ޣ�");
						break;
				}
			}
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ�ޡ�����ɾ����ָ�����ݶ���
		///  ��Ȩ���жϿ�����
		/// </summary>
		/// <param name="root"></param>
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///	<realDelete>
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\����" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" 
		/// DESCRIPTION=" " RANK_CODE="POS_ORGAN_U" NAME="������" />
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ���ۺ����\�����" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="�����" OBJ_NAME="�����" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="������" />
		///	</realDelete>
		/// </code>
		/// </remarks>
		private static void CheckPermissionRealDelete(XmlElement root)
		{
			DataSet gDs = null, oDs = null, uDs = null;

			foreach (XmlElement xNode in root.ChildNodes)
			{
				string strObjGuid = xNode.GetAttribute("GUID");
				switch (xNode.LocalName)
				{
					case "ORGANIZATIONS":
						if (oDs == null)
							oDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelOrg, "�Բ�����û��Ȩ�ްѻ�����ϵͳ��ɾ����");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, oDs),
							"�Բ�����û�аѵ�ǰ������ϵͳ��ɾ����Ȩ�ޣ�");
						break;
					case "GROUPS":
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelGroup, "�Բ�����û��Ȩ�ް���Ա���ϵͳ��ɾ����");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, gDs),
							"�Բ�����û�аѵ�ǰ��Ա���ϵͳ��ɾ����Ȩ�ޣ�");
						break;
					case "USERS":
					case "OU_USERS":
						string strParentGuid = xNode.GetAttribute("PARENT_GUID");
						if (uDs == null)
							uDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelUser, "�Բ�����û��Ȩ�ް��û���ϵͳ��ɾ����");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								strParentGuid,
								uDs),
							"�Բ�����û�аѵ�ǰ�û���ϵͳ��ɾ����Ȩ�ޣ�");
						break;
				}
			}
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ�ޡ��ƶ���ָ�������ݶ���
		///  ��Ȩ���жϿ������ɾ���Լ����»����еĴ���Ȩ�ޣ�
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///	<Move GUID="45a67fd1-9805-4a97-a70f-79efa6ed7b16" ORIGINAL_SORT="000000000000000022000002">
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\28����װ������\����װ����\������" GLOBAL_SORT="000000000000000022000003000001" 
		/// ORIGINAL_SORT="000000000000000022000003000001" DISPLAY_NAME="������" OBJ_NAME="������" LOGON_NAME="wangdonghong" 
		/// PARENT_GUID="829624bd-61f2-4ccd-aff8-5e7a22f5ff93" GUID="5902aba2-ef24-4acd-9c36-71a10e25ce9c" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="������" />
		///	</Move>
		/// </code>
		/// </remarks>
		private static void CheckPermissionMove(XmlElement root)
		{
			DataSet gDs = null, oDs = null, uDs = null;
			DataSet giDs = null, oiDs = null, uiDs = null;

			string strAimOrgGuid = root.GetAttribute("GUID");

			foreach (XmlElement xNode in root.ChildNodes)
			{
				string strObjGuid = xNode.GetAttribute("GUID");
				switch (xNode.LocalName)
				{
					case "ORGANIZATIONS":
						if (oDs == null)
							oDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelOrg, "�Բ�����û��Ȩ�ްѻ�����ϵͳ���ƶ���");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, oDs),
							"�Բ�����û�аѵ�ǰ������ϵͳ���ƶ���Ȩ�ޣ�");

						if (oiDs == null)
							oiDs = GetUserFunctionsScopes(AccreditResource.Func_CreateOrg, "�Բ�����û��Ȩ�ްѻ�����ϵͳ���ƶ���");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strAimOrgGuid, SearchObjectColumn.SEARCH_GUID, oiDs),
							"�Բ�����û�аѵ�ǰ������ϵͳ���ƶ���Ȩ�ޣ�");
						break;
					case "GROUPS":
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelGroup, "�Բ�����û��Ȩ�ް���Ա���ϵͳ���ƶ���");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, gDs),
							"�Բ�����û�аѵ�ǰ��Ա���ϵͳ���ƶ���Ȩ�ޣ�");

						if (giDs == null)
							giDs = GetUserFunctionsScopes(AccreditResource.Func_CreateGroup, "�Բ�����û��Ȩ�ް���Ա���ϵͳ���ƶ���");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strAimOrgGuid, SearchObjectColumn.SEARCH_GUID, giDs),
							"�Բ�����û�аѵ�ǰ��Ա���ϵͳ���ƶ���Ȩ�ޣ�");
						break;
					case "USERS":
					case "OU_USERS":
						string strParentGuid = xNode.GetAttribute("PARENT_GUID");
						if (uDs == null)
							uDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelUser, "�Բ�����û��Ȩ�ް��û���ϵͳ���ƶ���");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								strParentGuid,
								uDs),
							"�Բ�����û�аѵ�ǰ�û���ϵͳ���ƶ���Ȩ�ޣ�");

						if (uiDs == null)
							uiDs = GetUserFunctionsScopes(AccreditResource.Func_CreateUser, "�Բ�����û��Ȩ�ް��û���ϵͳ���ƶ���");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strAimOrgGuid, SearchObjectColumn.SEARCH_GUID, uiDs),
							"�Բ�����û�аѵ�ǰ�û���ϵͳ���ƶ���Ȩ�ޣ�");
						break;
				}
			}
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ�ޡ����򡱵�ǰָ�������ݶ���
		///  ��Ȩ���жϿ�����
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///	<Sort OrgGuid="829624bd-61f2-4ccd-aff8-5e7a22f5ff93">
		///		<USERS>c70ab1da-2b7f-41eb-898b-70e375c7de41</USERS>
		///		<USERS>f82124cb-5c21-4601-a126-bf2d1761ed5a</USERS>
		///		<USERS>5902aba2-ef24-4acd-9c36-71a10e25ce9c</USERS>
		///	</Sort>
		/// </code>
		/// </remarks>
		private static void CheckPermissionSort(XmlElement root)
		{
			string strOrgGuid = root.GetAttribute("OrgGuid");

			DataSet oDs = GetUserFunctionsScopes(AccreditResource.Func_SortInOrg, "�Բ�����û��Ȩ�޶Ի����еĶ����������");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strOrgGuid, SearchObjectColumn.SEARCH_GUID, oDs),
				"�Բ�����û�жԵ�ǰ�����ж�����������Ȩ�ޣ�");
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ�޶�ָ������Ա���г�Ա��������
		/// ��Ȩ���жϿ�����
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// xmlDoc�е��������£�
		/// <code>
		///	<GroupSort GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" extAttr="E_MAIL,DESCRIPTION,RANK_CODE,NAME">
		///		<USERS>
		///			<USER_PARENT_GUID>65eb8160-f0fa-4f1c-8984-2649788fe1d0</USER_PARENT_GUID>
		///			<USER_GUID>1ea2d199-54e6-4a78-9282-710e455b4973</USER_GUID>
		///		</USERS>
		///		<USERS>
		///			<USER_PARENT_GUID>65eb8160-f0fa-4f1c-8984-2649788fe1d0</USER_PARENT_GUID>
		///			<USER_GUID>5239c6a9-59bf-4965-aa51-f2712e6aa577</USER_GUID>
		///		</USERS>
		///		<USERS>
		///			<USER_PARENT_GUID>65eb8160-f0fa-4f1c-8984-2649788fe1d0</USER_PARENT_GUID>
		///			<USER_GUID>5592c712-ebea-471e-aea0-2661a258699a</USER_GUID>
		///		</USERS>
		///	</GroupSort>
		/// </code>
		/// </remarks>
		private static void CheckPermissionGroupSort(XmlElement root)
		{
			string strGroupGuid = root.GetAttribute("GUID");

			DataSet gDs = GetUserFunctionsScopes(AccreditResource.Func_SortInGroup, "�Բ�����û��Ȩ�޶���Ա���еĶ����������");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("GROUPS", strGroupGuid, SearchObjectColumn.SEARCH_GUID, gDs),
				"�Բ�����û�жԵ�ǰ��Ա���ж�����������Ȩ�ޣ�");
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ����ָ����Ա���С�����³�Ա��
		/// ��Ȩ���жϿ�����
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///	<addObjectsToGroups USER_ACCESS_LEVEL="" GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" extAttr="E_MAIL,DESCRIPTION,RANK_CODE,NAME">
		///		<object OBJECTCLASS="ORGANIZATIONS" SIDELINE="" POSTURAL="" RANK_NAME="" STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\01�칫��\���쵼" GLOBAL_SORT="000000000000000001000000" ORIGINAL_SORT="000000000000000001000000" 
		/// DISPLAY_NAME="���쵼" OBJ_NAME="���쵼" LOGON_NAME="" PARENT_GUID="0c6135c6-1038-48ec-a79b-ba63dffac758" 
		/// GUID="dd615a2f-efe1-4cd3-bb6a-3d5946e13156" />
		///	</addObjectsToGroups>
		/// </code>
		/// </remarks>
		private static void CheckPermissionAddObjectsToGroups(XmlElement root)
		{
			string strGroupGuid = root.GetAttribute("GUID");

			DataSet gDs = GetUserFunctionsScopes(AccreditResource.Func_GroupAddUser, "�Բ�����û��Ȩ��Ϊ��Ա�������³�Ա��");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("GROUPS", strGroupGuid, SearchObjectColumn.SEARCH_GUID, gDs),
				"�Բ�����û�жԵ�ǰ��Ա�������³�Ա��Ȩ�ޣ�");
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ����ָ����Ա���С�ɾ����Ա��
		/// ��Ȩ���жϿ�����
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///		<delUsersFromGroups GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef">
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" 
		/// RANK_NAME=" " STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\Ĳ����" GLOBAL_SORT="000000000000000000000000" 
		/// ORIGINAL_SORT="000000000000000000000000" DISPLAY_NAME="Ĳ����" OBJ_NAME="Ĳ����" LOGON_NAME="muxinsh" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_MINISTRY_U" NAME="��������" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" 
		/// RANK_NAME=" " STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\����" GLOBAL_SORT="000000000000000000000005" 
		/// ORIGINAL_SORT="000000000000000000000005" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="zhenpu" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="6d41eeb3-d229-4b58-8753-f1e2a42a5104" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_MINISTRY_U" NAME="��������" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\Ҷ��" GLOBAL_SORT="000000000000000000000006" 
		/// ORIGINAL_SORT="000000000000000000000006" DISPLAY_NAME="Ҷ��" OBJ_NAME="Ҷ��" LOGON_NAME="yejian" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="9b22f19b-0815-4d39-8315-a6b4cd09505f" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="���м���" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME="tyty" 
		/// STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\����" GLOBAL_SORT="000000000000000000000004" 
		/// ORIGINAL_SORT="000000000000000000000004" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="zhaorong" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="959d109a-8a08-4062-86c5-0143e0f9028e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_MINISTRY_U" NAME="��������" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\���ѫ" GLOBAL_SORT="000000000000000000000008" 
		/// ORIGINAL_SORT="000000000000000000000008" DISPLAY_NAME="���ѫ" OBJ_NAME="���ѫ" LOGON_NAME="yangguoxun" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="5e3aa542-29c3-40b5-b4cc-617045223c22" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="���м���" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" "
		/// STATUS="1" ALL_PATH_NAME="�й�����\01��������\00���쵼\��ľ��" GLOBAL_SORT="000000000000000000000007"
		/// ORIGINAL_SORT="000000000000000000000007" DISPLAY_NAME="��ľ��" OBJ_NAME="��ľ��" LOGON_NAME="duanmujun" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="b79d63c8-5bc4-44be-b099-b7cac2202a67" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="���м���" />
		///		</delUsersFromGroups>
		/// </code>
		/// </remarks>
		private static void CheckPermissionDelUsersFromGroups(XmlElement root)
		{
			string strGroupGuid = root.GetAttribute("GUID");

			DataSet gDs = GetUserFunctionsScopes(AccreditResource.Func_GroupDelUser, "�Բ�����û��Ȩ��Ϊ��Ա��ɾ����Ա��");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("GROUPS", strGroupGuid, SearchObjectColumn.SEARCH_GUID, gDs),
				"�Բ�����û�жԵ�ǰ��Ա��ɾ����Ա��Ȩ�ޣ�");
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ�ް��û��ĵ�ǰ��ְ����Ϊ��ְ
		/// ���жϿ�����,���û������ά�������Լ�������Ҫְ���ά�����ܣ�
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param> 
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///		<setMainDuty>
		///			<MainDuty>
		///				<USER_GUID>5e3aa542-29c3-40b5-b4cc-617045223c22</USER_GUID>
		///				<PARENT_GUID>5b23a9ad-645f-43c1-b969-cfc9fa0c16d7</PARENT_GUID>
		///			</MainDuty>
		///		</setMainDuty>
		/// </code>
		/// </remarks>
		private static void CheckPermissionSetUserMainDuty(XmlElement root)
		{
			DataSet uDs = GetUserFunctionsScopes(AccreditResource.Func_ModifyUser, "�Բ�����û��Ȩ��Ϊ�û�������Ҫְ��");

			foreach (XmlElement xNode in root.ChildNodes)
			{
				string strObjGuid = xNode.SelectSingleNode("USER_GUID").InnerText;
				string strParentGuid = xNode.SelectSingleNode("PARENT_GUID").InnerText;
				ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("USERS", strObjGuid, SearchObjectColumn.SEARCH_GUID, strParentGuid, uDs),
					"�Բ�����û�жԵ�ǰ�û�������Ҫְ���Ȩ�ޣ�");

				DataSet ds = OGUReader.GetObjectsDetail("USERS",
					strObjGuid,
					SearchObjectColumn.SEARCH_GUID,
					string.Empty,
					SearchObjectColumn.SEARCH_NULL);
				foreach (DataRow row in ds.Tables[0].Select(" SIDELINE = 0 "))
				{
					strParentGuid = OGUCommonDefine.DBValueToString(row["PARENT_GUID"]);
					ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("USERS", strObjGuid, SearchObjectColumn.SEARCH_GUID, strParentGuid, uDs),
						"�Բ�����û�жԵ�ǰ�û�������Ҫְ���Ȩ�ޣ�");
				}
			}
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ�޸�ָ���쵼��������
		/// �����쵼������Ȩ�ޣ�
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param> 
		/// <param name="da">���ݿ��������</param>
		/// <remarks> 
		/// root�е��������£�
		/// <code>
		///		<addSecsToLeader GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf" USER_ACCESS_LEVEL="" 
		/// extAttr="E_MAIL,DESCRIPTION,RANK_CODE,NAME" START_TIME="2004-05-19" END_TIME="2004-5-31">
		///			<object OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\01�칫��\�𳤰칫��\����" GLOBAL_SORT="000000000000000001000001000000" 
		/// ORIGINAL_SORT="000000000000000001000001000000" DISPLAY_NAME="����" OBJ_NAME="����" LOGON_NAME="fankun" 
		/// PARENT_GUID="8fce3073-5569-4bdd-847b-30d5be138e7f" GUID="c5e5065f-68e4-4764-a058-99c7e23ea208" />
		///		</addSecsToLeader>
		/// </code>
		/// </remarks>
		private static void CheckPermissionAddSecsToLeader(XmlElement root)
		{
			string strLeaderGuid = root.GetAttribute("GUID");

			DataSet uDs = GetUserFunctionsScopes(AccreditResource.Func_SecretaryAdd, "�Բ�����û��Ȩ��Ϊ�û��������飡");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("USERS", strLeaderGuid, SearchObjectColumn.SEARCH_GUID, uDs),
				"�Բ�����û�жԵ�ǰ�û����������Ȩ�ޣ�");
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ��ɾ��ָ���쵼������
		/// �����쵼������Ȩ�ޣ�
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param> 
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///		<delSecsOfLeader GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf">
		///			 <USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="�й�����\01��������\01�칫��\���쵼\���" GLOBAL_SORT="000000000000000001000000000002" 
		/// ORIGINAL_SORT="000000000000000001000000000002" DISPLAY_NAME="���" OBJ_NAME="���" LOGON_NAME="yangchenguang" 
		/// PARENT_GUID="dd615a2f-efe1-4cd3-bb6a-3d5946e13156" GUID="46c71220-710d-4f69-bd2b-ae3b99267df6" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_OFFICE_U" NAME="���ּ�" />
		///		</delSecsOfLeader>
		/// </code>
		/// </remarks>
		private static void CheckPermissionDelSecsOfLeader(XmlElement root)
		{
			string strLeaderGuid = root.GetAttribute("GUID");

			DataSet uDs = GetUserFunctionsScopes(AccreditResource.Func_SecretaryDel, "�Բ�����û��Ȩ��Ϊ�û�ɾ�����飡");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("USERS", strLeaderGuid, SearchObjectColumn.SEARCH_GUID, uDs),
				"�Բ�����û�жԵ�ǰ�û�ɾ�������Ȩ�ޣ�");
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ�޸�ָ����Ա�������ÿ���
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param> 
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///		<ResetPassword>
		///			<GUID>b33b3c0c-8f28-45ef-8373-8fcccac0246f</GUID>
		///			<OldPwd>000000</OldPwd>
		///			<OldPwdType>21545d16-a62f-4a7e-ac2f-beca958e0fdf</OldPwdType>
		///			<NewPwd>000000</NewPwd>
		///			<NewPwdType>21545d16-a62f-4a7e-ac2f-beca958e0fdf</NewPwdType>
		///		</ResetPassword>
		/// </code>
		/// </remarks>
		private static void CheckPermissionResetPassword(XmlElement root)
		{
			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strUserGuid = root.SelectSingleNode("GUID").InnerText;

			ExceptionHelper.FalseThrow(strUserGuid == lou.UserGuid, "�Բ���ֻ���û����˲����޸Ŀ��");
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ȩ�ް�ָ���û��Ŀ�������Ϊ��ʼ������
		/// </summary>
		/// <param name="root">��������Ҫ�󱻲���������</param>
		/// <param name="da">���ݿ��������</param>
		/// <remarks>
		/// root�е��������£�
		/// <code>
		///		<InitPassword>
		///			<GUID>959d109a-8a08-4062-86c5-0143e0f9028e</GUID>
		///			<PARENT_GUID>237698798-8a024062-86c5-0143e0f9028e</PARENT_GUID>
		///		</InitPassword>
		/// </code>
		/// </remarks>
		private static void CheckPermissionInitPassword(XmlElement root)
		{
			DataSet uDs = GetUserFunctionsScopes(AccreditResource.Func_InitUserPwd, "�Բ�����û��Ȩ��Ϊ�û���ʼ�����");

			string strObjGuid = root.SelectSingleNode("GUID").InnerText;
			string strParentGuid = root.SelectSingleNode("PARENT_GUID").InnerText;
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("USERS", strObjGuid, SearchObjectColumn.SEARCH_GUID, strParentGuid, uDs),
				"�Բ�����û�жԵ�ǰ�û���ʼ�������Ȩ�ޣ�");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strObjType"></param>
		/// <param name="strObjValue"></param>
		/// <param name="soc"></param>
		/// <param name="objsDs"></param>
		/// <param name="da"></param>
		/// <returns></returns>
		private static bool IsObjectIsIncludeInObjects(string strObjType, string strObjValue, SearchObjectColumn soc, DataSet objsDs)
		{
			return IsObjectIsIncludeInObjects(strObjType, strObjValue, soc, string.Empty, objsDs);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strFuncCodeName"></param>
		/// <param name="strErrMsg"></param>
		/// <returns></returns>
		private static DataSet GetUserFunctionsScopes(string strFuncCodeName, string strErrMsg)
		{
			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;

			bool bPermission = SecurityCheck.DoesUserHasPermissions(lou.UserLogOnName,
				AccreditResource.AppCodeName,
				strFuncCodeName,
				UserValueType.LogonName,
				DelegationMaskType.All);
			ExceptionHelper.FalseThrow(bPermission, strErrMsg);

			return SecurityCheck.GetUserFunctionsScopes(lou.UserLogOnName,
				AccreditResource.AppCodeName,
				strFuncCodeName,
				UserValueType.LogonName,
				DelegationMaskType.All,
				ScopeMaskType.All);
		}
		#endregion

		#region ��־��¼
		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="strOPType"></param>
		/// <param name="strOPMsg"></param>
		/// <param name="da"></param>
		private static void WriteLogOfDeleteObjects(XmlDocument xmlDoc, string strOPType, string strOPMsg)
		{
			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;

			string strObj = string.Empty;

			foreach (XmlElement elem in xmlDoc.DocumentElement.ChildNodes)
			{
				if (strObj.Length > 0)
					strObj += "��";
				string strParentGuid = string.Empty;
				switch (elem.LocalName)
				{
					case "ORGANIZATIONS": strObj += "[������";
						break;
					case "GROUPS": strObj += "[��Ա�飺";
						break;
					case "USERS": strObj += "[�û���";
						strParentGuid = elem.GetAttribute("PARENT_GUID");
						break;
				}

				strObj += elem.GetAttribute("ALL_PATH_NAME") + "]";
			}

			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]��(" + strObj + ")" + strOPMsg + "��";

			WriteLogMsg(strOPType, strExplain, xmlDoc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		private static void WriteLogOfInsertObjects(XmlDocument xmlDoc)
		{
			XmlElement root = xmlDoc.DocumentElement;

			string strObj = string.Empty;
			foreach (XmlElement elem in root.ChildNodes)
			{
				if (strObj.Length > 0)
					strObj += "��";

				switch (elem.LocalName)
				{
					case "ORGANIZATIONS": strObj += "[������";
						break;
					case "GROUPS": strObj += "[��Ա�飺";
						break;
					case "USERS": strObj += "[�û���";
						break;
				}
				strObj += elem.SelectSingleNode(".//ALL_PATH_NAME").InnerText + "]";
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]��ϵͳ��";
			if (root.GetAttribute("opType") != null && root.GetAttribute("opType") == "AddSideline")
			{
				strExplain += "���ü�ְ(" + strObj + ")��";
				WriteLogMsg(OGULogDefine.LOG_SET_SIDELINE, strExplain, xmlDoc);
			}
			else
			{
				strExplain += "��������(" + strObj + ")��";
				WriteLogMsg(OGULogDefine.LOG_CREATE_OBJECTS, strExplain, xmlDoc);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="da"></param>
		private static void WriteLogOfUpdateObjects(XmlDocument xmlDoc)
		{
			string strObj = string.Empty;
			foreach (XmlElement elem in xmlDoc.DocumentElement.ChildNodes)
			{
				if (strObj.Length > 0)
					strObj += "��";
				XmlNode wNode = elem.SelectSingleNode("WHERE");
				string strObjGuid = string.Empty;
				string strParentGuid = string.Empty;
				switch (elem.LocalName)
				{
					case "ORGANIZATIONS": strObj += "[������";
						strObjGuid = wNode.SelectSingleNode("GUID").InnerText;
						break;
					case "GROUPS": strObj += "[��Ա�飺";
						strObjGuid = wNode.SelectSingleNode("GUID").InnerText;
						break;
					case "USERS": strObj += "[�û���";
						strObjGuid = wNode.SelectSingleNode("USER_GUID").InnerText;
						strParentGuid = wNode.SelectSingleNode("PARENT_GUID").InnerText;
						break;
				}

				DataSet ds = OGUReader.GetObjectsDetail(elem.LocalName,
					strObjGuid,
					SearchObjectColumn.SEARCH_GUID,
					strParentGuid,
					SearchObjectColumn.SEARCH_GUID);

				strObj += OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]";
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]�Ѷ���(" + strObj + ")�����������޸ģ�";

			WriteLogMsg(OGULogDefine.LOG_UPDATE_OBJECTS, strExplain, xmlDoc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="da"></param>
		private static void WriteLogInitPassword(XmlDocument xmlDoc)
		{
			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;

			XmlElement root = xmlDoc.DocumentElement;
			DataSet ds = OGUReader.GetObjectsDetail("USERS",
				root.SelectSingleNode("GUID").InnerText,
				SearchObjectColumn.SEARCH_GUID,
				root.SelectSingleNode("PARENT_GUID").InnerText,
				SearchObjectColumn.SEARCH_GUID);

			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]�Ѷ���(�û���"
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + ")�������ʼ����";

			WriteLogMsg(OGULogDefine.LOG_INIT_USERS_PWD, strExplain, xmlDoc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="da"></param>
		private static void WriteLogMoveObjects(XmlDocument xmlDoc)
		{
			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;

			XmlElement root = xmlDoc.DocumentElement;
			DataSet oDs = OGUReader.GetObjectsDetail("ORGANIZATIONS",
				root.GetAttribute("GUID"),
				SearchObjectColumn.SEARCH_GUID,
				string.Empty,
				SearchObjectColumn.SEARCH_NULL);

			string strObjs = string.Empty;
			foreach (XmlElement elem in root.ChildNodes)
			{
				if (strObjs.Length > 0)
					strObjs += "��";

				string strParentGuid = string.Empty;
				string strObjGuid = elem.GetAttribute("GUID");
				switch (elem.LocalName)
				{
					case "ORGANIZATIONS": strObjs += "[������";
						break;
					case "GROUPS": strObjs += "[��Ա�飺";
						break;
					case "OU_USERS":
					case "USERS": strObjs += "[�û���";
						break;
				}
				DataSet ds = OGUReader.GetObjectsDetail(elem.LocalName,
					strObjGuid,
					SearchObjectColumn.SEARCH_GUID,
					strParentGuid,
					SearchObjectColumn.SEARCH_GUID);
				strObjs += OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]";
			}

			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]�Ѷ���(" + strObjs + ")�ƶ���[������"
				+ OGUCommonDefine.DBValueToString(oDs.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]�У�";

			WriteLogMsg(OGULogDefine.LOG_MOVE_OBJECTS, strExplain, xmlDoc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="da"></param>
		private static void WriteLogSortObjects(XmlDocument xmlDoc)
		{
			XmlElement root = xmlDoc.DocumentElement;
			DataSet ds = OGUReader.GetObjectsDetail("ORGANIZATIONS",
				root.GetAttribute("OrgGuid"),
				SearchObjectColumn.SEARCH_GUID,
				string.Empty,
				SearchObjectColumn.SEARCH_GUID);

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]��[������"
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]�еĶ���������������";

			WriteLogMsg(OGULogDefine.LOG_SORT_IN_ORGANIZATIONS, strExplain, xmlDoc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="da"></param>
		private static void WriteLogGroupSortObjects(XmlDocument xmlDoc)
		{
			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			XmlElement root = xmlDoc.DocumentElement;

			DataSet ds = OGUReader.GetObjectsDetail("GROUPS",
				root.GetAttribute("GUID"),
				SearchObjectColumn.SEARCH_GUID,
				string.Empty,
				SearchObjectColumn.SEARCH_NULL);
			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]��[��Ա�飺"
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]�еĶ���������������";

			WriteLogMsg(OGULogDefine.LOG_SORT_IN_GROUP, strExplain, xmlDoc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="da"></param>
		private static void WriteLogSetUserMainDuty(XmlDocument xmlDoc)
		{
			string strObjs = string.Empty;
			XmlElement root = xmlDoc.DocumentElement;
			foreach (XmlElement elem in root.ChildNodes)
			{
				DataSet ds = OGUReader.GetObjectsDetail("USERS",
					elem.SelectSingleNode("USER_GUID").InnerText,
					SearchObjectColumn.SEARCH_GUID,
					elem.SelectSingleNode("PARENT_GUID").InnerText,
					SearchObjectColumn.SEARCH_GUID);

				if (strObjs.Length > 0)
					strObjs += "��";

				strObjs += "[�û���" + OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]";
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]��(" + strObjs + ")����Ϊ��Ҫְ��";

			WriteLogMsg(OGULogDefine.LOG_SET_MAINDUTY, strExplain, xmlDoc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="da"></param>
		private static void WriteLogDelUsersFromGroups(XmlDocument xmlDoc)
		{
			XmlElement root = xmlDoc.DocumentElement;

			DataSet gds = OGUReader.GetObjectsDetail("GROUPS",
				root.GetAttribute("GUID"),
				SearchObjectColumn.SEARCH_GUID,
				string.Empty,
				SearchObjectColumn.SEARCH_NULL);

			string strObjs = string.Empty;
			foreach (XmlElement elem in root.ChildNodes)
			{
				if (strObjs.Length > 0)
					strObjs += "��";

				strObjs += "[�û���" + elem.GetAttribute("ALL_PATH_NAME") + "]";
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]��(" + strObjs + ")��[��Ա�飺"
				+ OGUCommonDefine.DBValueToString(gds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]��ɾ����";

			WriteLogMsg(OGULogDefine.LOG_GROUP_DEL_USERS, strExplain, xmlDoc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="da"></param>
		private static void WriteLogAddObjectsToGroups(XmlDocument xmlDoc)
		{
			XmlElement root = xmlDoc.DocumentElement;

			DataSet ds = OGUReader.GetObjectsDetail("GROUPS",
				root.GetAttribute("GUID"),
				SearchObjectColumn.SEARCH_GUID,
				string.Empty,
				SearchObjectColumn.SEARCH_NULL);

			string strObjs = string.Empty;
			foreach (XmlElement elem in root.ChildNodes)
			{
				if (strObjs.Length > 0)
					strObjs += "��";

				switch (elem.GetAttribute("OBJECTCLASS"))
				{
					case "ORGANIZATIONS": strObjs += "[������" + elem.GetAttribute("ALL_PATH_NAME") + "]��������Ա";
						break;
					case "GROUPS": strObjs += "[��Ա�飺" + elem.GetAttribute("ALL_PATH_NAME") + "]��������Ա";
						break;
					case "USERS":
					case "OU_USERS": strObjs += "[�û���" + elem.GetAttribute("ALL_PATH_NAME") + "]";
						break;
				}
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]��[��Ա�飺"
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]�����³�Ա(" + strObjs + ")��";

			WriteLogMsg(OGULogDefine.LOG_GROUP_ADD_USERS, strExplain, xmlDoc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="da"></param>
		private static void WriteLogSetSecsToLeader(XmlDocument xmlDoc)
		{
			XmlElement root = xmlDoc.DocumentElement;

			DataSet ds = OGUReader.GetObjectsDetail("USERS",
				root.GetAttribute("GUID"),
				SearchObjectColumn.SEARCH_GUID,
				string.Empty,
				SearchObjectColumn.SEARCH_NULL);

			string strObjs = string.Empty;
			foreach (XmlElement elem in root.ChildNodes)
			{
				if (strObjs.Length > 0)
					strObjs += "��";

				switch (elem.GetAttribute("OBJECTCLASS"))
				{
					case "ORGANIZATIONS": strObjs += "[������" + elem.GetAttribute("ALL_PATH_NAME") + "]��������Ա";
						break;
					case "GROUPS": strObjs += "[��Ա�飺" + elem.GetAttribute("ALL_PATH_NAME") + "]��������Ա";
						break;
					case "USERS":
					case "OU_USERS": strObjs += "[�û���" + elem.GetAttribute("ALL_PATH_NAME") + "]";
						break;
				}
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]��[�û���"
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]��������(" + strObjs + ")��";

			WriteLogMsg(OGULogDefine.LOG_SECRETARY_ADD, strExplain, xmlDoc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="da"></param>
		private static void WriteLogDelSecsOfLeader(XmlDocument xmlDoc)
		{
			XmlElement root = xmlDoc.DocumentElement;

			DataSet ds = OGUReader.GetObjectsDetail("USERS",
				root.GetAttribute("GUID"),
				SearchObjectColumn.SEARCH_GUID,
				string.Empty,
				SearchObjectColumn.SEARCH_NULL);

			string strObjs = string.Empty;
			foreach (XmlElement elem in root.ChildNodes)
			{
				if (strObjs.Length > 0)
					strObjs += "��";

				strObjs += "[�û���" + elem.GetAttribute("ALL_PATH_NAME") + "]";
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[�û���" + lou.OuUsers[0].AllPathName + "]��[�û���"
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]ɾ������(" + strObjs + ")��";

			WriteLogMsg(OGULogDefine.LOG_SECRETARY_DEL, strExplain, xmlDoc);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strOPType"></param>
		/// <param name="strExplain"></param>
		/// <param name="xmlDoc"></param>
		private static void WriteLogMsg(string strOPType, string strExplain, XmlDocument xmlDoc)
		{
			UserDataWrite.InsertUserLog(OGULogDefine.LOG_OGU_APP_NAME, strOPType, strExplain, xmlDoc.OuterXml);
		}

		#endregion
	}
}