/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System.Diagnostics;
using System.Windows.Forms;

namespace KryBot.Gui.WinFormsGui.Forms
{
    public partial class FormDonate : Form
    {
        public FormDonate()
        {
            InitializeComponent();
            Localization();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(((LinkLabel) sender).Text);
        }
    }
}