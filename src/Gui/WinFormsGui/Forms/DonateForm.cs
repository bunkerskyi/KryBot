using System.Diagnostics;
using System.Windows.Forms;

namespace KryBot.Gui.WinFormsGui.Forms
{
	public partial class FormDonate : Form
	{
		public FormDonate()
		{
			InitializeComponent();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(((LinkLabel)sender).Text);
		}
	}
}