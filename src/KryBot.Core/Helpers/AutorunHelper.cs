using System;
using System.Security;
using System.Windows.Forms;
using KryBot.CommonResources.lang;
using Microsoft.Win32;

namespace KryBot.Core.Helpers
{
	public static class AutorunHelper
	{
		public static bool SetAutorun(string path)
		{
			try
			{
				var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
				key?.SetValue("KryBot", path);
				return true;
			}
			catch(SecurityException)
			{
				MessageBox.Show(
					@"Для выполнения этого действия приложение должно быть запущено с правами администратора",
					@"Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			catch(UnauthorizedAccessException)
			{
				MessageBox.Show(
					@"При записи в реестр произошла aошибка. Запись в реестр разрешена администратором?",
					strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}

		public static bool DeleteAutorun()
		{
			try
			{
				var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
				key?.DeleteValue("KryBot");
				return true;
			}
			catch(SecurityException)
			{
				MessageBox.Show(
					@"Для выполнения этого действия приложение должно быть запущено с правами администратора",
					@"Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			catch(UnauthorizedAccessException)
			{
				MessageBox.Show(
					@"При записи в реестр произошла ошибка. Запись в реестр разрешена администратором?",
					strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			catch(ArgumentException)
			{
				return false;
			}
		}
	}
}
