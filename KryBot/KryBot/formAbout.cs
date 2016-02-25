using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using KryBot.Properties;

namespace KryBot
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            Design();
        }

        private void Design()
        {
            Icon = Icon.FromHandle(Resources.information.GetHicon());
            Text = @"О программе";

            labelVersion.Text = @"KryBot - " + Application.ProductVersion;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://vk.com/krybot");
        }
    }
}