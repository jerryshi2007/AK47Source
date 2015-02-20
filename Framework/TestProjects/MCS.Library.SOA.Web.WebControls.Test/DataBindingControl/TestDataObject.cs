using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using MCS.OA.DataObjects;
using MCS.Library.OGUPermission;
using MCS.Library.Core;
using MCS.Library.Validation;

namespace MCS.OA.Web.WebControls.Test.DataBindingControl
{
	public enum GenderDefine
	{
		[EnumItemDescription("-")]
		None = 1,

		[EnumItemDescription("��")]
		Male = 2,

		[EnumItemDescription("Ů")]
		Female = 3
	}

	public enum SizeDefine
	{
		[EnumItemDescription("-")]
		None,

		[EnumItemDescription("С")]
		Small,
		[EnumItemDescription("��")]
		Medium,
		[EnumItemDescription("��")]
		Big
	}

	[Flags]
	public enum BeverageDefine
	{
		[EnumItemDescription("ţ��")]
		Milk = 1,

		[EnumItemDescription("����")]
		Coffee = 2,

		[EnumItemDescription("��֭")]
		Juice = 4,

		[EnumItemDescription("����")]
		Coke = 16
	}

	[DefaultProperty("UserName")]
	public class TestDataObject
	{
		private string userName;
		private int userAge;
		private DateTime birthday = DateTime.MinValue;
		private DateTime passawayDay = DateTime.MinValue;
		private IUser creator = null;
		private string temper = "b";
		private GenderDefine gender;
		private SizeDefine size = SizeDefine.None;
		private double income = 111100.136;
		private BeverageDefine beverages;

		public BeverageDefine Beverages
		{
			get { return this.beverages; }
			set { this.beverages = value; }
		}

		[EnumDefaultValueValidator(MessageTemplate = "������ѡ��Size")]
		public SizeDefine Size
		{
			get { return this.size; }
			set { this.size = value; }
		}

		public GenderDefine Gender
		{
			get { return this.gender; }
			set { this.gender = value; }
		}

		public string Temper
		{
			get { return this.temper; }
			set { this.temper = value; }
		}

		[ObjectNullValidator(MessageTemplate = "Creator����Ϊ��")]
		public IUser Creator
		{
			get
			{
				return this.creator;
			}
			set
			{
				if (value is OguUser == false)
					this.creator = (IUser)OguUser.CreateWrapperObject(value);
				else
					this.creator = value;
			}
		}

		private MaterialList materials = null;

		[IntegerRangeValidator(1, 50000, MessageTemplate = "����1��50000֮��")]
		public int UserAge
		{
			get { return userAge; }
			set { userAge = value; }
		}

		[StringEmptyValidator(MessageTemplate = "�û����ֲ���Ϊ�գ�")]
		public string UserName
		{
			get { return userName; }
			set { userName = value; }
		}

		public DateTime Birthday
		{
			get { return this.birthday; }
			set { this.birthday = value; }
		}

		[DateTimeEmptyValidator(MessageTemplate = "PassawayDay����Ϊ��")]
		public DateTime PassawayDay
		{
			get { return this.passawayDay; }
			set { this.passawayDay = value; }
		}

		[DoubleRangeValidator(0, 999.99, MessageTemplate = "Income must be from 0.0 To 999.99")]
		public double Income
		{
			get { return this.income; }
			set { this.income = value; }
		}

		public MaterialList Materials
		{
			get
			{
				if (this.materials == null)
					this.materials = new MaterialList();

				return this.materials;
			}
		}

		private OguDataCollection<IUser> users = new OguDataCollection<IUser>();

		public OguDataCollection<IUser> Users
		{
			get
			{
				return this.users;
			}
		}

		[StringEmptyValidator(MessageTemplate = "�û�����2����Ϊ�գ�")]
		public string UserName2
		{
			get;
			set;
		}
	}
}
