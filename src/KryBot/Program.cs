using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using KryBot.lang;
using Microsoft.Win32;
using Exceptionless;

namespace KryBot
{
    internal static class Program
    {
        /// <summary>
        ///     Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.ThreadException += FormMain_UIThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            {
                ExceptionlessClient.Default.Register();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
        }

        public static void FormMain_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            t.Exception.ToExceptionless().Submit();
            MessageBox.Show($"[{t.Exception.TargetSite}] {{{t.Exception.Message}}}", strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }
    }
}