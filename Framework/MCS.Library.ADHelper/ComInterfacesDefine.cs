using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MCS.Library
{
	[ComImport, Guid("9068270B-0939-11D1-8BE1-00C04FD8D503"), TypeLibType((short)0x1040)]
	public interface IADsLargeInteger
	{
		[DispId(2)]
		int HighPart { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] set; }

		[DispId(3)]
		int LowPart { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] set; }
	}

	[ComImport, Guid("9068270B-0939-11D1-8BE1-00C04FD8D503"), CoClass(typeof(LargeIntegerClass))]
	public interface LargeInteger : IADsLargeInteger
	{
	}

	[ComImport, ClassInterface((short)0), Guid("927971F5-0939-11D1-8BE1-00C04FD8D503"), TypeLibType((short)2)]
	public class LargeIntegerClass : IADsLargeInteger, LargeInteger
	{
		// Properties
		[DispId(2)]
		public virtual extern int HighPart { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] set; }

		[DispId(3)]
		public virtual extern int LowPart { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] set; }

	}

	[Flags]
	public enum ADS_USER_FLAG : int
	{
		ADS_UF_NONE = 0,
		/*
		 * 原来的定义
        ADS_UF_ACCOUNTDISABLE = 2,
        ADS_UF_DONT_EXPIRE_PASSWD = 0x10000,
        ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x80,
        ADS_UF_HOMEDIR_REQUIRED = 8,
        ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0x800,
        ADS_UF_LOCKOUT = 0x10,
        ADS_UF_MNS_LOGON_ACCOUNT = 0x20000,
        ADS_UF_NORMAL_ACCOUNT = 0x200,
        ADS_UF_NOT_DELEGATED = 0x100000,
        ADS_UF_PASSWD_CANT_CHANGE = 0x40,
        ADS_UF_PASSWD_NOTREQD = 0x20,
        ADS_UF_SCRIPT = 1,
        ADS_UF_SERVER_TRUST_ACCOUNT = 0x2000,
        ADS_UF_SMARTCARD_REQUIRED = 0x40000,
        ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0x100,
        ADS_UF_TRUSTED_FOR_DELEGATION = 0x80000,
        ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x1000
		 */
		ADS_UF_SCRIPT = 0x00000001,
		ADS_UF_ACCOUNTDISABLE = 0x00000002,
		ADS_UF_HOMEDIR_REQUIRED = 0x00000008,
		ADS_UF_LOCKOUT = 0x00000010,
		ADS_UF_PASSWD_NOTREQD = 0x00000020,
		ADS_UF_PASSWD_CANT_CHANGE = 0x00000040,
		ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x00000080,
		ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0x00000100,
		ADS_UF_NORMAL_ACCOUNT = 0x00000200,
		ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0x00000800,
		ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x00001000,
		ADS_UF_SERVER_TRUST_ACCOUNT = 0x00002000,
		ADS_UF_DONT_EXPIRE_PASSWD = 0x00010000,
		ADS_UF_MNS_LOGON_ACCOUNT = 0x00020000,
		ADS_UF_SMARTCARD_REQUIRED = 0x00040000,
		ADS_UF_TRUSTED_FOR_DELEGATION = 0x00080000,
		ADS_UF_NOT_DELEGATED = 0x00100000,
		ADS_UF_USE_DES_KEY_ONLY = 0x00200000,
		ADS_UF_DONT_REQUIRE_PREAUTH = 0x00400000,
		ADS_UF_PASSWORD_EXPIRED = 0x00800000,
		ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x01000000,
	}

	public class UserAccountPolicy
	{
		bool userPwdLastSet = false;
		bool userPasswordNeverExpires = false;
		bool userAccountDisabled = false;
		DateTime userAccountExpirationDate = DateTime.MinValue;

		public bool UserPwdLastSet
		{
			get
			{
				return this.userPwdLastSet;
			}
			set
			{
				this.userPwdLastSet = value;
			}
		}

		public bool UserPasswordNeverExpires
		{
			get
			{
				return this.userPasswordNeverExpires;
			}
			set
			{
				this.userPasswordNeverExpires = value;
			}
		}

		public bool UserAccountDisabled
		{
			get
			{
				return this.userAccountDisabled;
			}
			set
			{
				this.userAccountDisabled = value;
			}
		}

		public DateTime UserAccountExpirationDate
		{
			get
			{
				return this.userAccountExpirationDate;
			}
			set
			{
				this.userAccountExpirationDate = value;
			}
		}
	}
}
