using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace KryBot.Core.Helpers
{
	public static class FileHelper
	{
		/// <summary>
		/// Trying to write (serialize) object to file.
		/// </summary>
		/// <returns> 
		/// If writing is successful, returns true, otherwise false. 
		/// </returns>
		public static bool Save<T>(T instance, string path) 
		{ 
			try
			{
				using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					var serializer = new XmlSerializer(typeof (T));
					serializer.Serialize(fileStream, instance);
				}
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		/// <summary>
		/// Trying to read (deserialize) the file into an object.
		/// </summary>
		/// <returns> 
		/// If reading is successful, returns true, otherwise false. 
		/// </returns>
		public static bool Load<T>(ref T instance, string path) 
		{ 
			try
			{
				using (var reader = new StreamReader(path))
				{
					var serializer = new XmlSerializer(typeof (T));
					instance = (T) serializer.Deserialize(reader);
				}
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		/// <summary>
		/// Trying to read (deserialize) the file into an object.
		/// </summary>
		/// <returns>
		/// If reading is successful, returns the completed object, otherwise create a new one.
		/// </returns>
		public static T SafelyLoad<T>(string path) where T : new()
		{
			try
			{
				using (var reader = new StreamReader(path))
				{
					var serializer = new XmlSerializer(typeof(T));
					return (T)serializer.Deserialize(reader);
				}
			}
			catch (Exception)
			{
				return new T();
			}
		}
	}
}
