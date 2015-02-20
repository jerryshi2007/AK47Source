using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission.Commands
{
	/// <summary>
	/// 
	/// </summary>
	public static class OutputHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public static void OutputObjectInfo(IOguObject obj)
		{
			Console.WriteLine("ID={0}", obj.ID);
			Console.WriteLine("SchemaType={0}", obj.ObjectType);
			Console.WriteLine("Name={0}", obj.Name);
			Console.WriteLine("DisplayName={0}", obj.DisplayName);
			Console.WriteLine("FullPath={0}", obj.FullPath);
			Console.WriteLine();
		}

		/// <summary>
		/// 输出对象的详细信息
		/// </summary>
		/// <param name="obj"></param>
		public static void OutputObjectDetailInfo(IOguObject obj)
		{
			Console.WriteLine("ID={0}", obj.ID);
			Console.WriteLine("SchemaType={0}", obj.ObjectType);
			Console.WriteLine("Name={0}", obj.Name);
			Console.WriteLine("DisplayName={0}", obj.DisplayName);
			Console.WriteLine("FullPath={0}", obj.FullPath);
			Console.WriteLine("Properties:");

			foreach (DictionaryEntry entry in obj.Properties)
			{
				string key = "null";
				string value = "null";

				if (entry.Key != null)
					key = entry.Key.ToString();

				if (entry.Value != null)
					value = entry.Value.ToString();

				Console.WriteLine("Key={0}, Value={1}", key, value);
			}

			Console.WriteLine();
		}

		/// <summary>
		/// 输出用户的详细信息，包括每一个属性的信息
		/// </summary>
		/// <param name="users"></param>
		public static void OutputUserDetailInfo(IEnumerable<IUser> users)
		{
			foreach (IUser user in users)
			{
				OutputObjectDetailInfo(user);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="objs"></param>
		public static void OutputObjectInfo(IEnumerable<IOguObject> objs)
		{
			foreach (IOguObject obj in objs)
			{
				OutputObjectInfo(obj);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="users"></param>
		public static void OutputUserInfo(IEnumerable<IUser> users)
		{
			foreach (IUser user in users)
			{
				OutputObjectInfo(user);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		public static void OutputUserInfo(IUser user)
		{
			OutputObjectInfo(user);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public static void OutputPermissionInfo(IPermissionObject obj)
		{
			Console.WriteLine("ID={0}", obj.ID);
			Console.WriteLine("Name={0}", obj.Name);
			Console.WriteLine("CodeName={0}", obj.CodeName);
			Console.WriteLine();
		}
	}
}
