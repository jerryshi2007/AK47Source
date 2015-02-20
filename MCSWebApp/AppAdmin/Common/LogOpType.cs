using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace MCS.Applications.AppAdmin.Common
{
	/// <summary>
	/// LOGϵͳ����Ҫ����Ĳ�������
	/// </summary>
	public enum LogOpType
	{
		INSERT_ROLE,
		UPDATE_ROLE,
		DELETE_ROLE,
		INSERT_FUNCTION,
		UPDATE_FUNCTION,
		DELETE_FUNCTION,
		MODIFY_ROLE_TO_FUNC,
		INSERT_DELEGATION,
		DELETE_DELEGATION,
		UPDATE_DELEGATION,
		ADD_APPLICATION_FUNC,//����������Ӧ��ϵͳ
		DELETE_APPLICATION_FUNC,//������ɾ��Ӧ��ϵͳ
		MODIFY_APPLICATION_FUNC,//�޸�Ӧ��ϵͳ������
		ADD_SCOPE_FUNC,//���������ӷ���Χ
		DELETE_SCOPE_FUNC,//������ɾ������Χ
		MODIFY_SCOPE_FUNC,//�������޸ķ���Χ-����Ȩ����֮��Ĺ�ϵ
		ADD_OBJECT_FUNC,//����������ɫ���ӱ���Ȩ����
		DELETE_OBJECT_FUNC,//������ɾ����ɫ�еı���Ȩ����
	}
}
