#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.OGUPermission
// FileName	��	EnumDefine.cs
// Remark	��	���л�����Ա��ѯʱ���ṩ��ID������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission
{
    /// <summary>
    /// ���л�����Ա��ѯʱ���ṩ��ID������
    /// </summary>
    public enum SearchOUIDType
    {
		/// <summary>
		/// ������
		/// </summary>
		None = 0,

        /// <summary>
        /// ����ID�����в�ѯ(Guid)
        /// </summary>
        Guid = 1,

        /// <summary>
        /// ���ն����ȫ·�������в�ѯ
        /// </summary>
        FullPath = 5,

        /// <summary>
        /// �����û��ĵ�¼���������в�ѯ
        /// </summary>
        LogOnName = 6,
    }

    /// <summary>
    /// ��ѯ�Ӷ�������
    /// </summary>
    public enum SearchLevel
    {
        /// <summary>
        /// ��ѯ�����Ӷ���
        /// </summary>
        SubTree = 0,

        /// <summary>
        /// ����ѯһ��
        /// </summary>
        OneLevel = 1
    }

	/// <summary>
	/// �г��������������
	/// </summary>
	[Flags]
	public enum ListObjectMask
	{
		/// <summary>
		/// ��
		/// </summary>
		None = 0,

		/// <summary>
		/// ��ѯ����ʹ�õ����ݶ���
		/// </summary>
		Common = 1,

		/// <summary>
		/// ֱ��ɾ���Ķ���
		/// </summary>
		DirectDeleted = 2,

		/// <summary>
		/// ���ŵ��������߼�ɾ������
		/// </summary>
		DeletedByOrganization = 4,

		/// <summary>
		/// ����Ա���������߼�ɾ������
		/// </summary>
		DeletedByUser = 8,

		/// <summary>
		/// ȫ������
		/// </summary>
		All = 15
	}

    /// <summary>
    /// ����������������
    /// </summary>
    public enum OrderByPropertyType
    {
		/// <summary>
		/// ������
		/// </summary>
		None = 0,

        /// <summary>
        /// ����GlobalSortID��������
        /// </summary>
        GlobalSortID = 1,

        /// <summary>
        /// ����FullPath����
        /// </summary>
        FullPath = 2,

        /// <summary>
        /// ������������
        /// </summary>
        Name = 3
    }

    /// <summary>
    /// ��������������
    /// </summary>
    public enum SortDirectionType
    {
		/// <summary>
		/// ������
		/// </summary>
		None = 0,

        /// <summary>
        /// ����
        /// </summary>
        Ascending = 1,

        /// <summary>
        /// ����
        /// </summary>
        Descending = 2
    }

    /// <summary>
    /// ���������
    /// </summary>
    [Flags]
    public enum SchemaType
    {
        /// <summary>
        /// δָ��
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// ��֯����
        /// </summary>
        Organizations = 1,

        /// <summary>
        /// �û�
        /// </summary>
        Users = 2,

        /// <summary>
        /// ��
        /// </summary>
        Groups = 4,

        /// <summary>
        /// ��ְ
        /// </summary>
        Sideline = 8,

        /// <summary>
        /// ��ɫ�еĻ���
        /// </summary>
        OrganizationsInRole = 65,

        /// <summary>
        /// ��������
        /// </summary>
        All = 65535
    }

    /// <summary>
    /// ��Ա�Ͳ��ŵļ���
    /// </summary>
    public enum UserRankType
    {
        /// <summary>
        /// ���м���
        /// </summary>
        [EnumItemDescription("(δָ��)", SortId = 99)]
        Unspecified = 0,

        /// <summary>
        /// ���м���
        /// </summary>
        [EnumItemDescription("���⼶��", SortId = 98)]
        MinGanJiBie = 1,

        /// <summary>
        /// ����
        /// </summary>
        [EnumItemDescription("����", SortId = 95)]
        GongRen = 8,

        /// <summary>
        /// һ����Ա
        /// </summary>
        [EnumItemDescription("һ����Ա", SortId = 90)]
        YiBanRenYuan = 10,

        /// <summary>
        /// ���Ƽ�
        /// </summary>
        [EnumItemDescription("���Ƽ�", SortId = 80)]
        FuKeji = 20,

        /// <summary>
        /// ���Ƽ�
        /// </summary>
        [EnumItemDescription("���Ƽ�", SortId = 70)]
        ZhengKeJi = 30,

        /// <summary>
        /// ������
        /// </summary>
        [EnumItemDescription("������", SortId = 60)]
        FuChuJi = 40,

        /// <summary>
        /// ������
        /// </summary>
        /// 
        [EnumItemDescription("������", SortId = 50)]
        ZhengChuJi = 50,

        /// <summary>
        /// ���ּ�
        /// </summary>
        [EnumItemDescription("���ּ�", SortId = 40)]
        FuJuJi = 60,

        /// <summary>
        /// ���ּ�
        /// </summary>
        [EnumItemDescription("���ּ�", SortId = 30)]
        ZhengJuJi = 70,

        /// <summary>
        /// ������
        /// </summary>
        [EnumItemDescription("������", SortId = 20)]
        FuBuJi = 80,

        /// <summary>
        /// ������
        /// </summary>
        [EnumItemDescription("������", SortId = 10)]
        ZhengBuJi = 90
    }

    /// <summary>
    /// ���ŵļ���
    /// </summary>
    public enum DepartmentRankType
    {
		/// <summary>
		/// ������
		/// </summary>
		[EnumItemDescription("(δָ��)", SortId = 99)]
		None = 0,

        /// <summary>
        /// ���м���
        /// </summary>
		 [EnumItemDescription("���м���", SortId = 95)]
        MinGanJiBie = 1,

        /// <summary>
        /// һ�㲿��
        /// </summary>
		 [EnumItemDescription("һ�㲿��", SortId = 90)]
        YiBanBuMen = 10,

        /// <summary>
        /// ���Ƽ�
        /// </summary>
		 [EnumItemDescription("���Ƽ�", SortId = 80)]
        FuKeji = 20,

        /// <summary>
        /// ���Ƽ�
        /// </summary>
		 [EnumItemDescription("���Ƽ�", SortId = 70)]
        ZhengKeJi = 30,

        /// <summary>
        /// ������
        /// </summary>
		 [EnumItemDescription("������", SortId = 60)]
        FuChuJi = 40,

        /// <summary>
        /// ������
        /// </summary>
		 [EnumItemDescription("������", SortId = 50)]
        ZhengChuJi = 50,

        /// <summary>
        /// ���ּ�
        /// </summary>
		 [EnumItemDescription("���ּ�", SortId = 40)]
        FuJuJi = 60,

        /// <summary>
        /// ���ּ�
        /// </summary>
		 [EnumItemDescription("���ּ�", SortId = 30)]
        ZhengJuJi = 70,

        /// <summary>
        /// ������
        /// </summary>
		 [EnumItemDescription("������", SortId = 20)]
        FuBuJi = 80,

        /// <summary>
        /// ������
        /// </summary>
		[EnumItemDescription("������", SortId = 10)]
        ZhengBuJi = 90
    }

    /// <summary>
    /// ���ŵķ��࣬��:32�������ء�64��פ����...
    /// </summary>
    [Flags]
    public enum DepartmentClassType
    {
        /// <summary>
        /// δָ��
        /// </summary>
		[EnumItemDescription("δָ��")]
        Unspecified = 0,

        /// <summary>
        /// ��������
        /// </summary>
		[EnumItemDescription("������֯")]
        LiShuHaiGuan = 32,

        /// <summary>
        /// ��פ����
        /// </summary>
		[EnumItemDescription("��פ��֯")]
        PaiZhuJiGou = 64,

        /// <summary>
        /// �������
        /// </summary>
		[EnumItemDescription("������֯")]
        NeiSheJiGou = 128,

        /// <summary>
        /// ��������
        /// </summary>
		[EnumItemDescription("������֯")]
        QiTaJiGou = 256,
    }

    /// <summary>
    /// ���ŵ�һЩ�������ԣ�1���������2һ�㲿�š�4�칫�ң�������8�ۺϴ���
    /// </summary>
    [Flags]
    public enum DepartmentTypeDefine
    {
        /// <summary>
        /// δָ��
        /// </summary>
        [EnumItemDescription("(δָ��)")]
        Unspecified = 0,

        /// <summary>
        /// �������
        /// </summary>
        [EnumItemDescription("�������")]
        XuNiJiGou = 1,

        /// <summary>
        /// һ�㲿��
        /// </summary>
        [EnumItemDescription("һ�㲿��")]
        YiBanBuMen = 2,

        /// <summary>
        /// �칫�ң�����
        /// </summary>
        [EnumItemDescription("�칫��(��)")]
        BanGongShi = 4,

        /// <summary>
        /// �ۺϴ�
        /// </summary>
        [EnumItemDescription("�ۺϴ�")]
        ZongHeChu = 8,

        /// <summary>
        /// ��˽��
        /// </summary>
        [EnumItemDescription("��˽��")]
        JiSiJu = 16,
    }

    /// <summary>
    /// �û���һЩ�������ԣ������Ա1����ܸɲ�2�������ɲ�4������ɲ�8��
    /// </summary>
    [Flags]
    public enum UserAttributesType
    {
        /// <summary>
        /// δָ��
        /// </summary>
        [EnumItemDescription("(δָ��)")]
        Unspecified = 0,

        /// <summary>
        /// �����Ա
        /// </summary>
        [EnumItemDescription("�����Ա")]
        DangZuChengYuan = 1,

        /// <summary>
        /// ��ܸɲ�
        /// </summary>
        [EnumItemDescription("��ܸɲ�")]
        ShuGuanGanBu = 2,

        /// <summary>
        /// �����ɲ�
        /// </summary>
        [EnumItemDescription("�����ɲ�")]
        JiaoLiuGanBu = 4,

        /// <summary>
        /// ����ɲ�
        /// </summary>
        [EnumItemDescription("����ɲ�")]
        JieDiaoGanBu = 8
    }

    /// <summary>
    /// �������ظ��û�����ʱ��������ְ�����Ǽ�ְ
    /// </summary>
    public enum DistinctReserveType
    {
		/// <summary>
		/// ������
		/// </summary>
		None = 0, 

        /// <summary>
        /// ������ְ
        /// </summary>
        KeepMasterOccupation = 1,

        /// <summary>
        /// ������ְ
        /// </summary>
        KeepSidelineOccupation = 2
    }
}
