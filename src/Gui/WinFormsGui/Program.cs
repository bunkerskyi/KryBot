using System;
using System.Threading;
using System.Windows.Forms;
using Exceptionless;
using KryBot.CommonResources.Localization;
using KryBot.Gui.WinFormsGui.Forms;

namespace KryBot.Gui.WinFormsGui
{
    public static class Program
    {
        /// <summary>
        ///     Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        public static void Main()
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

        private static void FormMain_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            MessageBox.Show($"[{t.Exception.TargetSite}] {{{t.Exception.Message}}}", strings.Error, MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            t.Exception.ToExceptionless().Submit();
            Environment.Exit(0);
        }
    }
}