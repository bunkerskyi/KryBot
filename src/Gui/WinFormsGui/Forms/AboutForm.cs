using System.Diagnostics;
using System.Windows.Forms;
using KryBot.CommonResources.Localization;

namespace KryBot.Gui.WinFormsGui.Forms
{
    public partial class FormAbout : Form
	{
		public FormAbout()
		{
			InitializeComponent();
            Localization();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(((LinkLabel) sender).Text);
		}

		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(((LinkLabel) sender).Text);
		}

		private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(((LinkLabel) sender).Text);
		}
	}
}