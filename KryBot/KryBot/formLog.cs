using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using KryBot.Properties;

namespace KryBot
{
    public partial class FormLog : Form
    {
        private readonly int _x;
        private readonly int _y;
        public bool Win7;

        public FormLog(int x, int y)
        {
            _x = x;
            _y = y;
            InitializeComponent();
        }

        private void formLog_Load(object sender, EventArgs e)
        {
            Location = new Point(_x, _y);
            var owner = Owner as FormMain;
            if (owner != null) AppendText(richTextBox1, owner.LogBuffer.Content, owner.LogBuffer.Color);
            Design();

            if (Environment.OSVersion.Version.ToString().Split('.')[1] == "1")
            {
                Win7 = true;
            }
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
            if (Win7)
            {
                if (owner != null) Location = new Point(owner.Location.X + owner.Width, owner.Location.Y);
            }
            else
            {
                if (owner != null) Location = new Point(owner.Location.X + owner.Width - 15, owner.Location.Y);
            }
        }

        public void LogChanged()
        {
            var owner = Owner as FormMain;
            if (owner != null) AppendText(richTextBox1, owner.LogBuffer.Content, owner.LogBuffer.Color);
        }

        private void Design()
        {
            Text = @"Лог";
            Icon = Icon.FromHandle(Resources.log.GetHicon());
        }

        public static void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
    }
}