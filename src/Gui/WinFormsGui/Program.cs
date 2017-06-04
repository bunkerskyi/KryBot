using System;
using System.Threading;
using System.Windows.Forms;
using KryBot.CommonResources.Localization;
using KryBot.Gui.WinFormsGui.Forms;
using SharpRaven;
using SharpRaven.Data;

namespace KryBot.Gui.WinFormsGui
{
    public static class Program
    {
        private static RavenClient _ravenClient;

        /// <summary>
        ///     Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.ThreadException += FormMain_UIThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            {
                _ravenClient = new RavenClient("https://cbe9a28e8d5149e19e744ba0726e1c5b:b84d96c35f8f45cb910ea8f8ed2b5b6b@sentry.io/175808");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
        }

        private static void FormMain_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            MessageBox.Show($@"[{t.Exception.TargetSite}] {{{t.Exception.Message}}}", strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            _ravenClient.Capture(new SentryEvent(t.Exception));
            Environment.Exit(0);
        }
    }
}