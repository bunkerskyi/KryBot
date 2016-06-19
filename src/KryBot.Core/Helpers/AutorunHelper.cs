using System;
using System.Security;
using System.Windows.Forms;
using KryBot.CommonResources.Localization;
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
			catch (SecurityException)
			{
				MessageBox.Show(
					strings.Autorun_RegisterEditError,
					strings.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				MessageBox.Show(
					strings.Autorun_RegisterEditError,
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
			catch (SecurityException)
			{
				MessageBox.Show(
					strings.Autorun_MustHaveAdminPerm,
					strings.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			catch (UnauthorizedAccessException)
			{
				MessageBox.Show(
					strings.Autorun_RegisterEditError,
					strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}
	}
}