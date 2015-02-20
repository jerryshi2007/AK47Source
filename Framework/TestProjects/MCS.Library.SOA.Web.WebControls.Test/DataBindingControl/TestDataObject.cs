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

		[EnumItemDescription("男")]
		Male = 2,

		[EnumItemDescription("女")]
		Female = 3
	}

	public enum SizeDefine
	{
		[EnumItemDescription("-")]
		None,

		[EnumItemDescription("小")]
		Small,
		[EnumItemDescription("中")]
		Medium,
		[EnumItemDescription("大")]
		Big
	}

	[Flags]
	public enum BeverageDefine
	{
		[EnumItemDescription("牛奶")]
		Milk = 1,

		[EnumItemDescription("咖啡")]
		Coffee = 2,

		[EnumItemDescription("果汁")]
		Juice = 4,

		[EnumItemDescription("可乐")]
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

		[EnumDefaultValueValidator(MessageTemplate = "别忘了选择Size")]
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

		[ObjectNullValidator(MessageTemplate = "Creator不能为空")]
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

		[IntegerRangeValidator(1, 50000, MessageTemplate = "年龄1到50000之间")]
		public int UserAge
		{
			get { return userAge; }
			set { userAge = value; }
		}

		[StringEmptyValidator(MessageTemplate = "用户名字不能为空！")]
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

		[DateTimeEmptyValidator(MessageTemplate = "PassawayDay不能为空")]
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

		[StringEmptyValidator(MessageTemplate = "用户名字2不能为空！")]
		public string UserName2
		{
			get;
			set;
		}
	}
}
