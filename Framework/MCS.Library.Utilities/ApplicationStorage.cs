using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace MCS.Library.Utilities
{
	public static class ApplicationStorage
	{
		public static void SaveObject(string fileName, object data)
		{
			string filePath = Path.Combine(Application.UserAppDataPath, fileName);

			try
			{
				BinaryFormatter formatter = new BinaryFormatter();

				using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
				{
					formatter.Serialize(stream, data);
				}
			}
			catch (System.Exception)
			{
			}
		}

		public static object LoadObject(string fileName)
		{
			object result = null;
			string filePath = Path.Combine(Application.UserAppDataPath, fileName);

			try
			{
				BinaryFormatter formatter = new BinaryFormatter();

				using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					result = formatter.Deserialize(stream);
				}
			}
			catch (System.Exception)
			{
			}

			return result;
		}
	}
}
