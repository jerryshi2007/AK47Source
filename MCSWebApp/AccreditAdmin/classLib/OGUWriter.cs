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
	/// 机构人员管理系统中的所有维护操作实现
	/// </summary>
	internal class OGUWriter
	{
		/// <summary>
		/// 
		/// </summary>
		public OGUWriter()
		{
		}

		#region 操作相关实现
		/// <summary>
		/// 系统中要求实现的权限检测（检测当前用户是否有足够的权限对xmlDoc中的数据进行xmlDoc中指定的操作）
		/// </summary>
		/// <param name="xmlDoc">包含了所有要求进行的所有操作数据</param>
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
					default: ExceptionHelper.TrueThrow(true, "系统中没有相关数据处理\"" + strCommandName + "\"的程序！");
						break;
				}
			}
		}

		/// <summary>
		/// 逻辑删除xmlDoc中指定的所有数据对象（逻辑删除到回收站中）
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被删除的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
		/// <code>
		///		<logicDelete>
		///			<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\夏青" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="夏青" OBJ_NAME="夏青" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" 
		/// DESCRIPTION=" " RANK_CODE="POS_ORGAN_U" NAME="正处级" />
		///			<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\李广生" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="李广生" OBJ_NAME="李广生" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="副处级" />
		///		</logicDelete>
		/// </code>
		/// </remarks>
		public static void LogicDeleteObjects(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);		

			OguDataWriter.LogicDeleteObjects(xmlDoc);		
			
			WriteLogOfDeleteObjects(xmlDoc, OGULogDefine.LOG_LOGIC_DELETE_OBJECTS, "删除到了系统回收站中");// 写日志

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// 把xmlDoc中指定的所有数据对象从回收站中恢复到系统中正常使用
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
		/// <code>
		///	<furbishDelete>
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\夏青" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="夏青" OBJ_NAME="夏青" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_ORGAN_U" NAME="正处级" />
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\李广生" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="李广生" OBJ_NAME="李广生" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="副处级" />
		///	</furbishDelete>
		/// </code>
		/// </remarks>
		public static void FurbishDeleteObjects(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.FurbishDeleteObjects(xmlDoc);
			
			WriteLogOfDeleteObjects(xmlDoc, OGULogDefine.LOG_FURBISH_DELETE_OBJECTS, "从系统回收站中恢复");// 写日志

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// 确定的从数据库中把数据对象删除
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
		/// <code>
		///	<realDelete>
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\夏青" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="夏青" OBJ_NAME="夏青" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_ORGAN_U" NAME="正处级" />
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\李广生" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="李广生" OBJ_NAME="李广生" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="副处级" />
		///	</realDelete>
		/// </code>
		/// </remarks>
		public static void RealDeleteObjects(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.RealDeleteObjects(xmlDoc);

			WriteLogOfDeleteObjects(xmlDoc, OGULogDefine.LOG_REAL_DELETE_OBJECTS, "彻底从系统中删除");// 写日志

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// 在系统中创建一项新的数据对象（机构、人员、人员组）
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <example>
		/// xmlDoc中的数据如下：
		/// <code>
		///		<Insert>
		///			<ORGANIZATIONS>
		///				<SET>
		///					<OBJ_NAME>testOrg</OBJ_NAME>
		///					<DISPLAY_NAME>testOrg</DISPLAY_NAME>
		///					<RANK_CODE>POS_DEPART_D</RANK_CODE>
		///					<ORG_CLASS>0</ORG_CLASS>
		///					<ORG_TYPE>2</ORG_TYPE>
		///					<ALL_PATH_NAME>中国海关\01海关总署\01办公厅\testOrg</ALL_PATH_NAME>
		///					<CREATE_TIME>2004-05-18</CREATE_TIME>
		///					<DESCRIPTION>testOrg</DESCRIPTION>
		///					<PARENT_GUID>0c6135c6-1038-48ec-a79b-ba63dffac758</PARENT_GUID>
		///				</SET>
		///			</ORGANIZATIONS>
		///		</Insert>
		/// 或者（设置兼职）
		///		<Insert opType="AddSideline">
		///			<USERS>
		///				<SET>
		///					<SIDELINE>1</SIDELINE>
		///					<LAST_NAME>戴</LAST_NAME>
		///					<FIRST_NAME>杰</FIRST_NAME>
		///					<OBJ_NAME>戴杰</OBJ_NAME>
		///					<DISPLAY_NAME>戴杰</DISPLAY_NAME>
		///					<LOGON_NAME>daijie</LOGON_NAME>
		///					<ALL_PATH_NAME>中国海关\01海关总署\01办公厅\厅领导\戴杰</ALL_PATH_NAME>
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

            WriteLogOfInsertObjects(xmlDoc);// 写日志

            OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// 修改系统中的一项数据对象（机构、人员(或兼职)、人员组）
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
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

			WriteLogOfUpdateObjects(xmlDoc);// 写日志

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// 把指定对象的登录口令设置为系统默认的口令（以及使用默认的加密方式）
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
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

			WriteLogInitPassword(xmlDoc);// 写日志
		}

		/// <summary>
		/// 用户自己来设置自己所使用的系统登录密码和数据加密的方式
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
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

			//本操作不记录日志
		}

		/// <summary>
		/// 移动指定对象到制定机构中
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
		/// <code>
		///	<Move GUID="45a67fd1-9805-4a97-a70f-79efa6ed7b16" ORIGINAL_SORT="000000000000000022000002">
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\技术装备部\王东红" GLOBAL_SORT="000000000000000022000003000001" 
		/// ORIGINAL_SORT="000000000000000022000003000001" DISPLAY_NAME="王东红" OBJ_NAME="王东红" LOGON_NAME="wangdonghong" 
		/// PARENT_GUID="829624bd-61f2-4ccd-aff8-5e7a22f5ff93" GUID="5902aba2-ef24-4acd-9c36-71a10e25ce9c" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="副处级" />
		///	</Move>
		/// </code>
		/// </remarks>
		public static void MoveObjects(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.MoveObjects(xmlDoc);			

			WriteLogMoveObjects(xmlDoc);// 写日志

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// 对于系统中指定的数据对象进行数据排队（保存系统中数据的次序）
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
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
		
			WriteLogSortObjects(xmlDoc);// 写日志

			OGUReader.RemoveAllCache();				
		}

		/// <summary>
		/// 人员组中的人员排序
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <returns>人员组中的人员排序后的结果</returns>
		/// <remarks>
		/// xmlDoc中的数据如下：
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

			WriteLogGroupSortObjects(xmlDoc);// 写日志

			OGUReader.RemoveAllCache();		
			
			return result;
		}

		/// <summary>
		/// 设置当前人员的当前职位为主要职位（其他职位为兼职职位）
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
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

			WriteLogSetUserMainDuty(xmlDoc);// 写日志

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// 删除指定人员组中的指定用户
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
		/// <code>
		///		<delUsersFromGroups GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef">
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\牟新生" GLOBAL_SORT="000000000000000000000000" 
		/// ORIGINAL_SORT="000000000000000000000000" DISPLAY_NAME="牟新生" OBJ_NAME="牟新生" LOGON_NAME="muxinsh" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_MINISTRY_U" NAME="正部长级" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\甄朴" GLOBAL_SORT="000000000000000000000005" 
		/// ORIGINAL_SORT="000000000000000000000005" DISPLAY_NAME="甄朴" OBJ_NAME="甄朴" LOGON_NAME="zhenpu" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="6d41eeb3-d229-4b58-8753-f1e2a42a5104" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_MINISTRY_U" NAME="副部长级" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\叶剑" GLOBAL_SORT="000000000000000000000006" 
		/// ORIGINAL_SORT="000000000000000000000006" DISPLAY_NAME="叶剑" OBJ_NAME="叶剑" LOGON_NAME="yejian" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="9b22f19b-0815-4d39-8315-a6b4cd09505f" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="敏感级别" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME="tyty" 
		/// STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\赵荣" GLOBAL_SORT="000000000000000000000004" 
		/// ORIGINAL_SORT="000000000000000000000004" DISPLAY_NAME="赵荣" OBJ_NAME="赵荣" LOGON_NAME="zhaorong" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="959d109a-8a08-4062-86c5-0143e0f9028e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_MINISTRY_U" NAME="副部长级" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\杨国勋" GLOBAL_SORT="000000000000000000000008" 
		/// ORIGINAL_SORT="000000000000000000000008" DISPLAY_NAME="杨国勋" OBJ_NAME="杨国勋" LOGON_NAME="yangguoxun" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="5e3aa542-29c3-40b5-b4cc-617045223c22" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="敏感级别" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\端木君" GLOBAL_SORT="000000000000000000000007" 
		/// ORIGINAL_SORT="000000000000000000000007" DISPLAY_NAME="端木君" OBJ_NAME="端木君" LOGON_NAME="duanmujun" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="b79d63c8-5bc4-44be-b099-b7cac2202a67" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="敏感级别" />
		///		</delUsersFromGroups>
		/// </code>
		/// </remarks>
		public static void DelUsersFromGroups(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.DelUsersFromGroups(xmlDoc);
		
			WriteLogDelUsersFromGroups(xmlDoc);// 写日志

			OGUReader.RemoveAllCache();
		}

		/// <summary>
		/// 把指定对象（机构中的人、人员组中的人、单个的人）加入到指定的“人员组中”
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <returns>把指定对象（机构中的人、人员组中的人、单个的人）加入到指定的“人员组中”后的结果</returns>
		/// <remarks>
		/// xmlDoc中的数据如下：
		/// <code>
		///	<addObjectsToGroups USER_ACCESS_LEVEL="" GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" extAttr="E_MAIL,DESCRIPTION,RANK_CODE,NAME">
		///		<object OBJECTCLASS="ORGANIZATIONS" SIDELINE="" POSTURAL="" RANK_NAME="" STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\01办公厅\厅领导" GLOBAL_SORT="000000000000000001000000" ORIGINAL_SORT="000000000000000001000000" 
		/// DISPLAY_NAME="厅领导" OBJ_NAME="厅领导" LOGON_NAME="" PARENT_GUID="0c6135c6-1038-48ec-a79b-ba63dffac758" 
		/// GUID="dd615a2f-efe1-4cd3-bb6a-3d5946e13156" />
		///	</addObjectsToGroups>
		/// </code>
		/// </remarks>
		public static XmlDocument AddObjectsToGroups(XmlDocument xmlDoc)
		{
			XmlDocument result;

			CheckUserOperation(xmlDoc);

			result = OguDataWriter.AddObjectsToGroups(xmlDoc);

			WriteLogAddObjectsToGroups(xmlDoc); // 写日志

			OGUReader.RemoveAllCache();

			return result;
		}

		/// <summary>
		/// 设置领导与秘书之间的关系
		/// </summary>
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <returns>设置领导与秘书之间的关系</returns>
		/// <remarks> 
		/// xmlDoc中的数据如下：
		/// <code>
		///		<addSecsToLeader GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf" USER_ACCESS_LEVEL="" extAttr="E_MAIL,DESCRIPTION,RANK_CODE,NAME" START_TIME="2004-05-19" END_TIME="2004-5-31">
		///			<object OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\01办公厅\署长办公室\樊堃" GLOBAL_SORT="000000000000000001000001000000" ORIGINAL_SORT="000000000000000001000001000000" DISPLAY_NAME="樊堃" OBJ_NAME="樊堃" LOGON_NAME="fankun" PARENT_GUID="8fce3073-5569-4bdd-847b-30d5be138e7f" GUID="c5e5065f-68e4-4764-a058-99c7e23ea208" />
		///		</addSecsToLeader>
		/// </code>
		/// </remarks>
		public static XmlDocument SetSecsToLeader(XmlDocument xmlDoc)
		{
			XmlDocument result;

			CheckUserOperation(xmlDoc);

			result = OguDataWriter.SetSecsToLeader(xmlDoc);

			WriteLogSetSecsToLeader(xmlDoc); // 写日志

			return result;			
		}

		/// <summary>
		/// 删除领导秘书中的其中几个 （指定的对象）
		/// </summary>k
		/// <param name="xmlDoc">包含所有要求被操作的数据</param>
		/// <param name="da">数据处理过程中要求使用的数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
		/// <code>
		///		<delSecsOfLeader GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf">
		///			 <USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\01办公厅\厅领导\杨晨光" GLOBAL_SORT="000000000000000001000000000002" 
		/// ORIGINAL_SORT="000000000000000001000000000002" DISPLAY_NAME="杨晨光" OBJ_NAME="杨晨光" LOGON_NAME="yangchenguang" 
		/// PARENT_GUID="dd615a2f-efe1-4cd3-bb6a-3d5946e13156" GUID="46c71220-710d-4f69-bd2b-ae3b99267df6" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_OFFICE_U" NAME="副局级" />
		///		</delSecsOfLeader>
		/// </code>
		/// </remarks>
		public static void DelSecsOfLeader(XmlDocument xmlDoc)
		{
			CheckUserOperation(xmlDoc);

			OguDataWriter.DelSecsOfLeader(xmlDoc);

			WriteLogDelSecsOfLeader(xmlDoc); // 写日志

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

		#region 内部数据处理
		/// <summary>
		/// 针对机构中的人员的相关数据处理
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

		#region 操作权限的检测
		/// <summary>
		/// 系统添加新数据对象权限检测（新添加一个用户、机构、人员组、一个兼职人员）
		/// （创建由父机构决定）
		/// </summary>
		/// <param name="root">包含所有数据的数据节点</param>
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// 如果发现权限不能通过，内部会报出异常导致本次操作失败
		/// </remarks>
		/// <example>
		/// root的节点内容
		/// <code>
		///		<Insert>
		///			<ORGANIZATIONS>
		///				<SET>
		///					<OBJ_NAME>testOrg</OBJ_NAME>
		///					<DISPLAY_NAME>testOrg</DISPLAY_NAME>
		///					<RANK_CODE>POS_DEPART_D</RANK_CODE>
		///					<ORG_CLASS>0</ORG_CLASS>
		///					<ORG_TYPE>2</ORG_TYPE>
		///					<ALL_PATH_NAME>中国海关\01海关总署\01办公厅\testOrg</ALL_PATH_NAME>
		///					<CREATE_TIME>2004-05-18</CREATE_TIME>
		///					<DESCRIPTION>testOrg</DESCRIPTION>
		///					<PARENT_GUID>0c6135c6-1038-48ec-a79b-ba63dffac758</PARENT_GUID>
		///				</SET>
		///			</ORGANIZATIONS>
		///		</Insert>
		/// 或者（设置兼职）
		///		<Insert opType="AddSideline">
		///			<USERS>
		///				<SET>
		///					<SIDELINE>1</SIDELINE>
		///					<LAST_NAME>戴</LAST_NAME>
		///					<FIRST_NAME>杰</FIRST_NAME>
		///					<OBJ_NAME>戴杰</OBJ_NAME>
		///					<DISPLAY_NAME>戴杰</DISPLAY_NAME>
		///					<LOGON_NAME>daijie</LOGON_NAME>
		///					<ALL_PATH_NAME>中国海关\01海关总署\01办公厅\厅领导\戴杰</ALL_PATH_NAME>
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
							oDs = GetUserFunctionsScopes(AccreditResource.Func_CreateOrg, "对不起，您没有创建新机构的权限！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strParentGuid, SearchObjectColumn.SEARCH_GUID, oDs),
							"对不起，您没有权限在该机构下创建新机构！");
						break;
					case "GROUPS":
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_CreateGroup, "对不起，您没有创建新人员组的权限！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strParentGuid, SearchObjectColumn.SEARCH_GUID, gDs),
							"对不起，您没有权限在该机构下创建新人员组！");
						break;
					case "OU_USERS":
					case "USERS":
						if (root.GetAttribute("opType") != null && root.GetAttribute("opType") == "AddSideline")
						{
							if (sDs == null)
								sDs = GetUserFunctionsScopes(AccreditResource.Func_SetSideline, "对不起，您没有设置用户兼职的权限！");
							ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strParentGuid, SearchObjectColumn.SEARCH_GUID, sDs),
								"对不起，您没有权限在该机构下设置用户兼职！");
						}
						else
						{
							if (uDs == null)
								uDs = GetUserFunctionsScopes(AccreditResource.Func_CreateUser, "对不起，您没有创建新用户的权限！");
							ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strParentGuid, SearchObjectColumn.SEARCH_GUID, uDs),
								"对不起，您没有权限在该机构下创建新用户！");
						}
						break;
					default: ExceptionHelper.TrueThrow(true, "对不起，没有对应于“" + xNode.LocalName + "”的相关处理程序！");
						break;
				}

			}
		}

		/// <summary>
		/// 判断当前用户是否有权限修改指定数据对象
		/// （修改由其自身决定）
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param>
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// root中的数据如下：
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
					case "ORGANIZATIONS"://(修改范围是其自身)
						string strObjGuid = xNode.SelectSingleNode("WHERE/GUID").InnerText;
						if (oDs == null)
							oDs = GetUserFunctionsScopes(AccreditResource.Func_ModifyOrg, "对不起，您没有权限修改机构属性！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, oDs),
							"对不起，您没有修改当前机构属性的权限！");
						break;
					case "GROUPS"://(修改范围是其自身)
						strObjGuid = xNode.SelectSingleNode("WHERE/GUID").InnerText;
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_ModifyGroup, "对不起，您没有权限修改人员组属性！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, gDs),
							"对不起，您没有修改当前人员组属性权限！");
						break;
					case "OU_USERS":
					case "USERS"://(修改范围是其自身)
						strObjGuid = xNode.SelectSingleNode("WHERE/USER_GUID").InnerText;
						string strParentGuid = xNode.SelectSingleNode("WHERE/PARENT_GUID").InnerText;
						if (uDs == null)
							uDs = GetUserFunctionsScopes(AccreditResource.Func_ModifyUser, "对不起，您没有权限修改用户属性！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, strParentGuid, uDs),
							"对不起，您没有修改当前用户属性权限！");
						break;
					default: ExceptionHelper.TrueThrow(true, "没有相关的数据处理信息！");
						break;
				}
			}
		}

		/// <summary>
		///  判断当前用户是否有权限“逻辑删除”指定数据对象
		///  （权限判断看自身）
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param>
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// root中的数据如下：
		/// <code>
		///		<logicDelete>
		///			<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\夏青" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="夏青" OBJ_NAME="夏青" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_ORGAN_U" NAME="正处级" />
		///			<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\李广生" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="李广生" OBJ_NAME="李广生" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="副处级" />
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
							oDs = GetUserFunctionsScopes(AccreditResource.Func_LogicDelOrg, "对不起，您没有权限把机构送入回收站！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, oDs),
							"对不起，您没有把当前机构送入回收站的权限！");
						break;
					case "GROUPS":
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_LogicDelGroup, "对不起，您没有权限把人员组送入回收站！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, gDs),
							"对不起，您没有把当前人员组送入回收站的权限！");
						break;
					case "USERS":
					case "OU_USERS":
						string strParentGuid = xNode.GetAttribute("PARENT_GUID");
						if (uDs == null)
							uDs = GetUserFunctionsScopes(AccreditResource.Func_LogicDelUser, "对不起，您没有权限把用户送入回收站！");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								strParentGuid,
								uDs),
							"对不起，您没有把当前用户送入回收站的权限！");
						break;
				}
			}
		}

		/// <summary>
		/// 判断当前用户是否有权限“恢复逻辑删除”指定数据对象
		///  （权限判断看自身）
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param>
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// root中的数据如下：
		/// <code>
		///	<furbishDelete>
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\夏青" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="夏青" OBJ_NAME="夏青" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_ORGAN_U" NAME="正处级" />
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\李广生" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="李广生" OBJ_NAME="李广生" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="副处级" />
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
							oDs = GetUserFunctionsScopes(AccreditResource.Func_FurbishOrg, "对不起，您没有权限把机构从回收站中恢复！");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								oDs),
							"对不起，您没有把当前机构从回收站中恢复的权限！");
						break;
					case "GROUPS":
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_FurbishGroup, "对不起，您没有权限把人员组从回收站中恢复！");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								gDs),
							"对不起，您没有把当前人员组从回收站中恢复的权限！");
						break;
					case "USERS":
					case "OU_USERS":
						string strParentGuid = xNode.GetAttribute("PARENT_GUID");
						if (uDs == null)
							uDs = GetUserFunctionsScopes(AccreditResource.Func_FurbishUser, "对不起，您没有权限把用户从回收站中恢复！");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								strParentGuid,
								uDs),
							"对不起，您没有把当前用户从回收站中恢复的权限！");
						break;
				}
			}
		}

		/// <summary>
		/// 判断当前用户是否有权限“彻底删除”指定数据对象
		///  （权限判断看自身）
		/// </summary>
		/// <param name="root"></param>
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// root中的数据如下：
		/// <code>
		///	<realDelete>
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\夏青" GLOBAL_SORT="000000000000000022000004000000" 
		/// ORIGINAL_SORT="000000000000000022000004000000" DISPLAY_NAME="夏青" OBJ_NAME="夏青" LOGON_NAME="xiaqing" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="07386754-080c-4f51-834d-dd293ba8e41e" E_MAIL="" 
		/// DESCRIPTION=" " RANK_CODE="POS_ORGAN_U" NAME="正处级" />
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="3" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\大型装备售后服务部\李广生" GLOBAL_SORT="000000000000000022000004000001" 
		/// ORIGINAL_SORT="000000000000000022000004000001" DISPLAY_NAME="李广生" OBJ_NAME="李广生" LOGON_NAME="liguangsheng" 
		/// PARENT_GUID="044f1535-285c-405e-a062-1f44e7d07d34" GUID="a3de281f-1d8f-4c95-9666-6c2c5329c3c4" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="副处级" />
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
							oDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelOrg, "对不起，您没有权限把机构从系统中删除！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, oDs),
							"对不起，您没有把当前机构从系统中删除的权限！");
						break;
					case "GROUPS":
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelGroup, "对不起，您没有权限把人员组从系统中删除！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, gDs),
							"对不起，您没有把当前人员组从系统中删除的权限！");
						break;
					case "USERS":
					case "OU_USERS":
						string strParentGuid = xNode.GetAttribute("PARENT_GUID");
						if (uDs == null)
							uDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelUser, "对不起，您没有权限把用户从系统中删除！");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								strParentGuid,
								uDs),
							"对不起，您没有把当前用户从系统中删除的权限！");
						break;
				}
			}
		}

		/// <summary>
		/// 判断当前用户是否有权限“移动”指定的数据对象
		///  （权限判断看自身的删除以及在新机构中的创建权限）
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param>
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// root中的数据如下：
		/// <code>
		///	<Move GUID="45a67fd1-9805-4a97-a70f-79efa6ed7b16" ORIGINAL_SORT="000000000000000022000002">
		///		<USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\28物资装备中心\技术装备部\王东红" GLOBAL_SORT="000000000000000022000003000001" 
		/// ORIGINAL_SORT="000000000000000022000003000001" DISPLAY_NAME="王东红" OBJ_NAME="王东红" LOGON_NAME="wangdonghong" 
		/// PARENT_GUID="829624bd-61f2-4ccd-aff8-5e7a22f5ff93" GUID="5902aba2-ef24-4acd-9c36-71a10e25ce9c" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_ORGAN_U" NAME="副处级" />
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
							oDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelOrg, "对不起，您没有权限把机构从系统中移动！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, oDs),
							"对不起，您没有把当前机构从系统中移动的权限！");

						if (oiDs == null)
							oiDs = GetUserFunctionsScopes(AccreditResource.Func_CreateOrg, "对不起，您没有权限把机构从系统中移动！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strAimOrgGuid, SearchObjectColumn.SEARCH_GUID, oiDs),
							"对不起，您没有把当前机构从系统中移动的权限！");
						break;
					case "GROUPS":
						if (gDs == null)
							gDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelGroup, "对不起，您没有权限把人员组从系统中移动！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects(xNode.LocalName, strObjGuid, SearchObjectColumn.SEARCH_GUID, gDs),
							"对不起，您没有把当前人员组从系统中移动的权限！");

						if (giDs == null)
							giDs = GetUserFunctionsScopes(AccreditResource.Func_CreateGroup, "对不起，您没有权限把人员组从系统中移动！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strAimOrgGuid, SearchObjectColumn.SEARCH_GUID, giDs),
							"对不起，您没有把当前人员组从系统中移动的权限！");
						break;
					case "USERS":
					case "OU_USERS":
						string strParentGuid = xNode.GetAttribute("PARENT_GUID");
						if (uDs == null)
							uDs = GetUserFunctionsScopes(AccreditResource.Func_RealDelUser, "对不起，您没有权限把用户从系统中移动！");
						ExceptionHelper.FalseThrow(
							IsObjectIsIncludeInObjects(xNode.LocalName,
								strObjGuid,
								SearchObjectColumn.SEARCH_GUID,
								strParentGuid,
								uDs),
							"对不起，您没有把当前用户从系统中移动的权限！");

						if (uiDs == null)
							uiDs = GetUserFunctionsScopes(AccreditResource.Func_CreateUser, "对不起，您没有权限把用户从系统中移动！");
						ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strAimOrgGuid, SearchObjectColumn.SEARCH_GUID, uiDs),
							"对不起，您没有把当前用户从系统中移动的权限！");
						break;
				}
			}
		}

		/// <summary>
		/// 判断当前用户是否有权限“排序”当前指定的数据对象
		///  （权限判断看自身）
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param>
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// root中的数据如下：
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

			DataSet oDs = GetUserFunctionsScopes(AccreditResource.Func_SortInOrg, "对不起，您没有权限对机构中的对象进行排序！");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("ORGANIZATIONS", strOrgGuid, SearchObjectColumn.SEARCH_GUID, oDs),
				"对不起，您没有对当前机构中对象进行排序的权限！");
		}

		/// <summary>
		/// 判断当前用户是否有权限对指定的人员组中成员进行排序
		/// （权限判断看自身）
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param>
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// xmlDoc中的数据如下：
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

			DataSet gDs = GetUserFunctionsScopes(AccreditResource.Func_SortInGroup, "对不起，您没有权限对人员组中的对象进行排序！");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("GROUPS", strGroupGuid, SearchObjectColumn.SEARCH_GUID, gDs),
				"对不起，您没有对当前人员组中对象进行排序的权限！");
		}

		/// <summary>
		/// 判断当前用户是否有权限在指定人员组中“添加新成员”
		/// （权限判断看自身）
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param>
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// root中的数据如下：
		/// <code>
		///	<addObjectsToGroups USER_ACCESS_LEVEL="" GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" extAttr="E_MAIL,DESCRIPTION,RANK_CODE,NAME">
		///		<object OBJECTCLASS="ORGANIZATIONS" SIDELINE="" POSTURAL="" RANK_NAME="" STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\01办公厅\厅领导" GLOBAL_SORT="000000000000000001000000" ORIGINAL_SORT="000000000000000001000000" 
		/// DISPLAY_NAME="厅领导" OBJ_NAME="厅领导" LOGON_NAME="" PARENT_GUID="0c6135c6-1038-48ec-a79b-ba63dffac758" 
		/// GUID="dd615a2f-efe1-4cd3-bb6a-3d5946e13156" />
		///	</addObjectsToGroups>
		/// </code>
		/// </remarks>
		private static void CheckPermissionAddObjectsToGroups(XmlElement root)
		{
			string strGroupGuid = root.GetAttribute("GUID");

			DataSet gDs = GetUserFunctionsScopes(AccreditResource.Func_GroupAddUser, "对不起，您没有权限为人员组增加新成员！");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("GROUPS", strGroupGuid, SearchObjectColumn.SEARCH_GUID, gDs),
				"对不起，您没有对当前人员组增加新成员的权限！");
		}

		/// <summary>
		/// 判断当前用户是否有权限在指定人员组中“删除成员”
		/// （权限判断看自身）
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param>
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// root中的数据如下：
		/// <code>
		///		<delUsersFromGroups GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef">
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" 
		/// RANK_NAME=" " STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\牟新生" GLOBAL_SORT="000000000000000000000000" 
		/// ORIGINAL_SORT="000000000000000000000000" DISPLAY_NAME="牟新生" OBJ_NAME="牟新生" LOGON_NAME="muxinsh" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="POS_MINISTRY_U" NAME="正部长级" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" 
		/// RANK_NAME=" " STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\甄朴" GLOBAL_SORT="000000000000000000000005" 
		/// ORIGINAL_SORT="000000000000000000000005" DISPLAY_NAME="甄朴" OBJ_NAME="甄朴" LOGON_NAME="zhenpu" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="6d41eeb3-d229-4b58-8753-f1e2a42a5104" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_MINISTRY_U" NAME="副部长级" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\叶剑" GLOBAL_SORT="000000000000000000000006" 
		/// ORIGINAL_SORT="000000000000000000000006" DISPLAY_NAME="叶剑" OBJ_NAME="叶剑" LOGON_NAME="yejian" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="9b22f19b-0815-4d39-8315-a6b4cd09505f" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="敏感级别" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME="tyty" 
		/// STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\赵荣" GLOBAL_SORT="000000000000000000000004" 
		/// ORIGINAL_SORT="000000000000000000000004" DISPLAY_NAME="赵荣" OBJ_NAME="赵荣" LOGON_NAME="zhaorong" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="959d109a-8a08-4062-86c5-0143e0f9028e" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_MINISTRY_U" NAME="副部长级" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" " 
		/// STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\杨国勋" GLOBAL_SORT="000000000000000000000008" 
		/// ORIGINAL_SORT="000000000000000000000008" DISPLAY_NAME="杨国勋" OBJ_NAME="杨国勋" LOGON_NAME="yangguoxun" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="5e3aa542-29c3-40b5-b4cc-617045223c22" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="敏感级别" />
		///			<USERS OBJECTCLASS="USERS" GROUP_GUID="cd978926-9de1-49b3-bb1b-0b9142ee1fef" SIDELINE="0" POSTURAL="4" RANK_NAME=" "
		/// STATUS="1" ALL_PATH_NAME="中国海关\01海关总署\00署领导\端木君" GLOBAL_SORT="000000000000000000000007"
		/// ORIGINAL_SORT="000000000000000000000007" DISPLAY_NAME="端木君" OBJ_NAME="端木君" LOGON_NAME="duanmujun" 
		/// PARENT_GUID="65eb8160-f0fa-4f1c-8984-2649788fe1d0" GUID="b79d63c8-5bc4-44be-b099-b7cac2202a67" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUSCEPTIVITY_U" NAME="敏感级别" />
		///		</delUsersFromGroups>
		/// </code>
		/// </remarks>
		private static void CheckPermissionDelUsersFromGroups(XmlElement root)
		{
			string strGroupGuid = root.GetAttribute("GUID");

			DataSet gDs = GetUserFunctionsScopes(AccreditResource.Func_GroupDelUser, "对不起，您没有权限为人员组删除成员！");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("GROUPS", strGroupGuid, SearchObjectColumn.SEARCH_GUID, gDs),
				"对不起，您没有对当前人员组删除成员的权限！");
		}

		/// <summary>
		/// 判断当前用户是否有权限把用户的当前兼职设置为主职
		/// （判断看自身,对用户自身的维护功能以及对其主要职务的维护功能）
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param> 
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// root中的数据如下：
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
			DataSet uDs = GetUserFunctionsScopes(AccreditResource.Func_ModifyUser, "对不起，您没有权限为用户设置主要职务！");

			foreach (XmlElement xNode in root.ChildNodes)
			{
				string strObjGuid = xNode.SelectSingleNode("USER_GUID").InnerText;
				string strParentGuid = xNode.SelectSingleNode("PARENT_GUID").InnerText;
				ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("USERS", strObjGuid, SearchObjectColumn.SEARCH_GUID, strParentGuid, uDs),
					"对不起，您没有对当前用户设置主要职务的权限！");

				DataSet ds = OGUReader.GetObjectsDetail("USERS",
					strObjGuid,
					SearchObjectColumn.SEARCH_GUID,
					string.Empty,
					SearchObjectColumn.SEARCH_NULL);
				foreach (DataRow row in ds.Tables[0].Select(" SIDELINE = 0 "))
				{
					strParentGuid = OGUCommonDefine.DBValueToString(row["PARENT_GUID"]);
					ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("USERS", strObjGuid, SearchObjectColumn.SEARCH_GUID, strParentGuid, uDs),
						"对不起，您没有对当前用户设置主要职务的权限！");
				}
			}
		}

		/// <summary>
		/// 判断当前用户是否有权限给指定领导设置秘书
		/// （对领导的设置权限）
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param> 
		/// <param name="da">数据库操作对象</param>
		/// <remarks> 
		/// root中的数据如下：
		/// <code>
		///		<addSecsToLeader GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf" USER_ACCESS_LEVEL="" 
		/// extAttr="E_MAIL,DESCRIPTION,RANK_CODE,NAME" START_TIME="2004-05-19" END_TIME="2004-5-31">
		///			<object OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\01办公厅\署长办公室\樊堃" GLOBAL_SORT="000000000000000001000001000000" 
		/// ORIGINAL_SORT="000000000000000001000001000000" DISPLAY_NAME="樊堃" OBJ_NAME="樊堃" LOGON_NAME="fankun" 
		/// PARENT_GUID="8fce3073-5569-4bdd-847b-30d5be138e7f" GUID="c5e5065f-68e4-4764-a058-99c7e23ea208" />
		///		</addSecsToLeader>
		/// </code>
		/// </remarks>
		private static void CheckPermissionAddSecsToLeader(XmlElement root)
		{
			string strLeaderGuid = root.GetAttribute("GUID");

			DataSet uDs = GetUserFunctionsScopes(AccreditResource.Func_SecretaryAdd, "对不起，您没有权限为用户设置秘书！");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("USERS", strLeaderGuid, SearchObjectColumn.SEARCH_GUID, uDs),
				"对不起，您没有对当前用户设置秘书的权限！");
		}

		/// <summary>
		/// 判断当前用户是否有权限删除指定领导的秘书
		/// （对领导的设置权限）
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param> 
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// root中的数据如下：
		/// <code>
		///		<delSecsOfLeader GUID="04f49e97-60f2-4eae-a993-1aa43bb48daf">
		///			 <USERS OBJECTCLASS="USERS" SIDELINE="0" POSTURAL="4" RANK_NAME=" " STATUS="1" 
		/// ALL_PATH_NAME="中国海关\01海关总署\01办公厅\厅领导\杨晨光" GLOBAL_SORT="000000000000000001000000000002" 
		/// ORIGINAL_SORT="000000000000000001000000000002" DISPLAY_NAME="杨晨光" OBJ_NAME="杨晨光" LOGON_NAME="yangchenguang" 
		/// PARENT_GUID="dd615a2f-efe1-4cd3-bb6a-3d5946e13156" GUID="46c71220-710d-4f69-bd2b-ae3b99267df6" E_MAIL="" DESCRIPTION=" " 
		/// RANK_CODE="SUB_OFFICE_U" NAME="副局级" />
		///		</delSecsOfLeader>
		/// </code>
		/// </remarks>
		private static void CheckPermissionDelSecsOfLeader(XmlElement root)
		{
			string strLeaderGuid = root.GetAttribute("GUID");

			DataSet uDs = GetUserFunctionsScopes(AccreditResource.Func_SecretaryDel, "对不起，您没有权限为用户删除秘书！");
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("USERS", strLeaderGuid, SearchObjectColumn.SEARCH_GUID, uDs),
				"对不起，您没有对当前用户删除秘书的权限！");
		}

		/// <summary>
		/// 判断当前用户是否有权限给指定人员重新设置口令
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param> 
		/// <remarks>
		/// root中的数据如下：
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

			ExceptionHelper.FalseThrow(strUserGuid == lou.UserGuid, "对不起，只有用户本人才能修改口令！");
		}

		/// <summary>
		/// 判断当前用户是否有权限把指定用户的口令设置为初始化口令
		/// </summary>
		/// <param name="root">包含所有要求被操作的数据</param>
		/// <param name="da">数据库操作对象</param>
		/// <remarks>
		/// root中的数据如下：
		/// <code>
		///		<InitPassword>
		///			<GUID>959d109a-8a08-4062-86c5-0143e0f9028e</GUID>
		///			<PARENT_GUID>237698798-8a024062-86c5-0143e0f9028e</PARENT_GUID>
		///		</InitPassword>
		/// </code>
		/// </remarks>
		private static void CheckPermissionInitPassword(XmlElement root)
		{
			DataSet uDs = GetUserFunctionsScopes(AccreditResource.Func_InitUserPwd, "对不起，您没有权限为用户初始化口令！");

			string strObjGuid = root.SelectSingleNode("GUID").InnerText;
			string strParentGuid = root.SelectSingleNode("PARENT_GUID").InnerText;
			ExceptionHelper.FalseThrow(IsObjectIsIncludeInObjects("USERS", strObjGuid, SearchObjectColumn.SEARCH_GUID, strParentGuid, uDs),
				"对不起，您没有对当前用户初始化口令的权限！");
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

		#region 日志记录
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
					strObj += "；";
				string strParentGuid = string.Empty;
				switch (elem.LocalName)
				{
					case "ORGANIZATIONS": strObj += "[机构：";
						break;
					case "GROUPS": strObj += "[人员组：";
						break;
					case "USERS": strObj += "[用户：";
						strParentGuid = elem.GetAttribute("PARENT_GUID");
						break;
				}

				strObj += elem.GetAttribute("ALL_PATH_NAME") + "]";
			}

			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]把(" + strObj + ")" + strOPMsg + "！";

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
					strObj += "；";

				switch (elem.LocalName)
				{
					case "ORGANIZATIONS": strObj += "[机构：";
						break;
					case "GROUPS": strObj += "[人员组：";
						break;
					case "USERS": strObj += "[用户：";
						break;
				}
				strObj += elem.SelectSingleNode(".//ALL_PATH_NAME").InnerText + "]";
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]在系统中";
			if (root.GetAttribute("opType") != null && root.GetAttribute("opType") == "AddSideline")
			{
				strExplain += "设置兼职(" + strObj + ")！";
				WriteLogMsg(OGULogDefine.LOG_SET_SIDELINE, strExplain, xmlDoc);
			}
			else
			{
				strExplain += "创建对象(" + strObj + ")！";
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
					strObj += "；";
				XmlNode wNode = elem.SelectSingleNode("WHERE");
				string strObjGuid = string.Empty;
				string strParentGuid = string.Empty;
				switch (elem.LocalName)
				{
					case "ORGANIZATIONS": strObj += "[机构：";
						strObjGuid = wNode.SelectSingleNode("GUID").InnerText;
						break;
					case "GROUPS": strObj += "[人员组：";
						strObjGuid = wNode.SelectSingleNode("GUID").InnerText;
						break;
					case "USERS": strObj += "[用户：";
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
			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]把对象(" + strObj + ")的属性作了修改！";

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

			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]把对象(用户："
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + ")的密码初始化！";

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
					strObjs += "；";

				string strParentGuid = string.Empty;
				string strObjGuid = elem.GetAttribute("GUID");
				switch (elem.LocalName)
				{
					case "ORGANIZATIONS": strObjs += "[机构：";
						break;
					case "GROUPS": strObjs += "[人员组：";
						break;
					case "OU_USERS":
					case "USERS": strObjs += "[用户：";
						break;
				}
				DataSet ds = OGUReader.GetObjectsDetail(elem.LocalName,
					strObjGuid,
					SearchObjectColumn.SEARCH_GUID,
					strParentGuid,
					SearchObjectColumn.SEARCH_GUID);
				strObjs += OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]";
			}

			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]把对象(" + strObjs + ")移动到[机构："
				+ OGUCommonDefine.DBValueToString(oDs.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]中！";

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
			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]对[机构："
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]中的对象作了重新排序！";

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
			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]对[人员组："
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]中的对象作了重新排序！";

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
					strObjs += "；";

				strObjs += "[用户：" + OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]";
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]将(" + strObjs + ")设置为主要职务";

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
					strObjs += "；";

				strObjs += "[用户：" + elem.GetAttribute("ALL_PATH_NAME") + "]";
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]将(" + strObjs + ")从[人员组："
				+ OGUCommonDefine.DBValueToString(gds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]中删除！";

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
					strObjs += "；";

				switch (elem.GetAttribute("OBJECTCLASS"))
				{
					case "ORGANIZATIONS": strObjs += "[机构：" + elem.GetAttribute("ALL_PATH_NAME") + "]中所有人员";
						break;
					case "GROUPS": strObjs += "[人员组：" + elem.GetAttribute("ALL_PATH_NAME") + "]中所有人员";
						break;
					case "USERS":
					case "OU_USERS": strObjs += "[用户：" + elem.GetAttribute("ALL_PATH_NAME") + "]";
						break;
				}
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]给[人员组："
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]增加新成员(" + strObjs + ")！";

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
					strObjs += "；";

				switch (elem.GetAttribute("OBJECTCLASS"))
				{
					case "ORGANIZATIONS": strObjs += "[机构：" + elem.GetAttribute("ALL_PATH_NAME") + "]中所有人员";
						break;
					case "GROUPS": strObjs += "[人员组：" + elem.GetAttribute("ALL_PATH_NAME") + "]中所有人员";
						break;
					case "USERS":
					case "OU_USERS": strObjs += "[用户：" + elem.GetAttribute("ALL_PATH_NAME") + "]";
						break;
				}
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]给[用户："
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]设置秘书(" + strObjs + ")！";

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
					strObjs += "；";

				strObjs += "[用户：" + elem.GetAttribute("ALL_PATH_NAME") + "]";
			}

			ILogOnUserInfo lou = GlobalInfo.UserLogOnInfo;
			string strExplain = "[用户：" + lou.OuUsers[0].AllPathName + "]给[用户："
				+ OGUCommonDefine.DBValueToString(ds.Tables[0].Rows[0]["ALL_PATH_NAME"]) + "]删除秘书(" + strObjs + ")！";

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