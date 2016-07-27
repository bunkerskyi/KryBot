using System;
using System.Security;
using System.Windows.Forms;
using KryBot.CommonResources.Localization;
using Microsoft.Win32;

using NLog;

namespace KryBot.Core.Helpers
{
	public static class AutorunHelper
	{
		private static ILogger _logger = LogManager.GetCurrentClassLogger();

		public static bool SetAutorun(string path)
		{
			try
			{
				var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
				key?.SetValue("KryBot", path);
				return true;
			}
			catch (SecurityException e)
			{
				MessageBox.Show(
					strings.Autorun_RegisterEditError,
					strings.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				_logger.Warn(e);
				return false;
			}
			catch (UnauthorizedAccessException e)
			{
				MessageBox.Show(
					strings.Autorun_RegisterEditError,
					strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				_logger.Error(e);
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
			catch (SecurityException e)
			{
				MessageBox.Show(
					strings.Autorun_MustHaveAdminPerm,
					strings.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				_logger.Warn(e);
				return false;
			}
			catch (UnauthorizedAccessException e)
			{
				MessageBox.Show(
					strings.Autorun_RegisterEditError,
					strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				_logger.Error(e);
				return false;
			}
			catch (ArgumentException e)
			{
				_logger.Error(e);
				return false;
			}
		}
	}
}