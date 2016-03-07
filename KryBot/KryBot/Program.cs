using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using KryBot.lang;
using Microsoft.Win32;

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
                Registry.CurrentUser.CreateSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION");
                RegistryKey key =
                    Registry.CurrentUser.OpenSubKey(
                        @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
                string programName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
                key?.SetValue(programName, (decimal) 11000, RegistryValueKind.DWord);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
        }

        public static void FormMain_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            MessageBox.Show($"[{t.Exception.TargetSite}] {{{t.Exception.Message}}}", strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }
    }
}