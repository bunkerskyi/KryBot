using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using KryBot.Core;

namespace KryBot.Gui.WinFormsGui.Forms
{
    public partial class FormLog : Form
    {
        private readonly LogMessage _messages = LogMessage.Instance;
        private readonly Settings _settings;
        private readonly int _y;
        private bool _win7;
        private int _x;
<<<<<<< HEAD:src/Gui/WinFormsGui/Forms/LogForm.cs
=======
        private bool _win7;
>>>>>>> refs/remotes/origin/master:src/KryBot/formLog.cs

        public FormLog(int x, int y, Settings settings)
        {
            _x = x;
            _y = y;
            _settings = settings;
            InitializeComponent();
            Localization();
            _messages.HandleMessage += OnHandleMessage;
        }

        private void formLog_Load(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.ToString().Split('.')[1] == "1")
            {
                _win7 = true;
            }

            if (_win7)
            {
                _x = _x + 15;
            }

            Location = new Point(_x, _y);
            Height = _settings.LogHeight;
            Width = _settings.LogWidth;
            var owner = Owner as FormMain;
            if (owner != null) AppendText(richTextBox, owner.LogBuffer.Content, owner.LogBuffer.Color);
        }

        public void FormHide()
        {
            Hide();
        }

        public void FormUnHide()
        {
            Show();
        }

        public void FormChangeLocation()
        {
            var owner = Owner as FormMain;
            if (_win7)
            {
                if (owner != null) Location = new Point(owner.Location.X + owner.Width, owner.Location.Y);
            }
            else
            {
                if (owner != null) Location = new Point(owner.Location.X + owner.Width - 15, owner.Location.Y);
            }
        }

<<<<<<< HEAD:src/Gui/WinFormsGui/Forms/LogForm.cs
=======
        public void LogChanged()
        {
            var owner = Owner as FormMain;
            if (owner != null) AppendText(richTextBox1, owner.LogBuffer.Content, owner.LogBuffer.Color);
        }

        private void Design()
        {
            Text = @"Лог";
            Icon = Icon.FromHandle(Resources.log.GetHicon());
            Height = Properties.Settings.Default.LogHeight;
            Width = Properties.Settings.Default.LogWidth;
        }

>>>>>>> refs/remotes/origin/master:src/KryBot/formLog.cs
        private static void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox.SelectionStart = richTextBox.Text.Length;
            richTextBox.ScrollToCaret();
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void FormLog_ResizeEnd(object sender, EventArgs e)
        {
            _settings.LogHeight = Height;
            _settings.LogWidth = Width;
        }

        private void OnHandleMessage(object sender, EventArgs args)
        {
            var messageEvent = args as MessageEventArgs;
            if (messageEvent != null)
            {
                AppendText(richTextBox, messageEvent.Message, messageEvent.Color);
            }
        }
    }
}