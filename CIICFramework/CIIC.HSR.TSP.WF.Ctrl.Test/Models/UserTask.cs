#region ���߰汾
// -------------------------------------------------
// Assembly	��	HB.DataObjects
// FileName	��	UserTask.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070723		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CIIC.HSR.TSP.WF.BizObject{
	/// <summary>
	/// ������ʵ����
	/// </summary>
	public class UserTask 
	{

		public string TaskID{ get; set; } 
       
		/// <summary>
		/// Ӧ������
		/// </summary>
		public string ApplicationName{ get; set; } 

		/// <summary>
		/// ģ������
		/// </summary>
		public string ProgramName { get; set; } 


		/// <summary>
		/// ����
		/// </summary>
		public string TaskTitle { get; set; } 


		/// <summary>
		/// ��ԴID
		/// </summary>
		public string ResourceID { get; set; } 

		/// <summary>
		/// ������ID
		/// </summary>
		public string SourceID { get; set; } 


		/// <summary>
		/// ����������
		/// </summary>
		public string SourceName { get; set; } 

		public string SendToUserID { get; set; } 


		public string SendToUserName { get; set; } 

		public string Body { get; set; } 

		/// <summary>
		/// ��Ϣ����
		/// </summary>
		public int?  Level { get; set; } 

		/// <summary>
		/// ����ID
		/// </summary>
		public string ProcessID{ get; set; } 


		/// <summary>
		/// �������ڵ�ID
		/// </summary>
		public string ActivityID { get; set; } 


		/// <summary>
		/// �������������
		/// </summary>
		public string Url { get; set; } 


		/// <summary>
		/// �����̶�
		/// </summary>
		public int?  Emergency{ get; set; } 


		/// <summary>
		/// �ڵ����ƣ���������
		/// </summary>
		public string Purpose { get; set; } 


		/// <summary>
		/// ��������״̬
		/// </summary>
		public string Status { get; set; } 


		/// <summary>
		/// ����ʼʱ��
		/// </summary>
		/// <remarks>һ��Ϊ�ļ������ʱ�䣬�Ե�ǰʱ��ΪĬ��ֵ�����ʵ������Ƿ��׵�</remarks>
		public DateTime TaskStartTime { get; set; } 


		/// <summary>
		/// ����ʱ��
		/// </summary>
		public DateTime ExpireTime { get; set; } 


		/// <summary>
		/// ��Ϣ����ʱ��
		/// </summary>
		public DateTime DeliverTime { get; set; } 


		public DateTime ReadTime { get; set; } 

		/// <summary>
		/// �û�����
		/// </summary>
        public TaskCategory Category { get; set; } 


		/// <summary>
		/// ��Ϣ�ö���־
		/// </summary>
		public int TopFlag { get; set; } 


		/// <summary>
		/// ��ݲ��ŵ�ʱ��
		/// </summary>
		public string DraftDepartmentName { get; set; }

		/// <summary>
		/// �Ѱ�����İ���ʱ��
		/// </summary>

		public DateTime CompletedTime { get; set; }

		public string DraftUserID { get; set; }

		public string DraftUserName { get; set; }

	}
}
