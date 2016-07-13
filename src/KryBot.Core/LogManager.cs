using System.Diagnostics;
using System.IO;
using System.Reflection;

using NLog;

namespace KryBot.Core
{
	public static class LogManager
	{
		static LogManager()
		{
			SetModuleName();
		}

		public static ILogger GetCurrentClassLogger()
		{
			
			var method = new StackTrace().GetFrame(1).GetMethod();
			var className = method.ReflectedType != null ? method.ReflectedType.FullName : "VOID";
			var nlogger = NLog.LogManager.GetLogger(className);
			return nlogger;
		}

		public static ILogger GetLogger(string name)
		{
			return NLog.LogManager.GetLogger(name);
		}

		/// <summary>
		/// Set variable assemblyName (Declared in NLog.config).
		/// </summary>
		private static void SetModuleName()
		{
			var currentAssembly = Assembly.GetEntryAssembly();
			if (currentAssembly != null)
			{
				var location = currentAssembly.Location;
				var moduleName = Path.GetFileNameWithoutExtension(location);
				NLog.LogManager.Configuration.Variables["assemblyName"] = moduleName;
			}
		}
	}
}
