using System;
using System.Drawing;
using System.Windows.Forms;
using KryBot.Gui.WinFormsGui.Properties;

namespace KryBot.Gui.WinFormsGui.Forms
{
    public partial class FormStatistic : Form
    {
        public FormStatistic()
        {
            InitializeComponent();
        }

        private void formStatistic_Load(object sender, EventArgs e)
        {
            Design();
        }

        private void Design()
        {
            Icon = Icon.FromHandle(Resources.statistic1.GetHicon());
            Text = @"Статистика";

            lblSessionJoins.Text += Properties.Settings.Default.JoinsPerSession;
            lblSessionLoops.Text += Properties.Settings.Default.JoinsLoops;

            lblTotalJoins.Text += Properties.Settings.Default.JoinsTotal;
            lblTotalLoops.Text += Properties.Settings.Default.JoinsLoopsTotal;
        }
    }
}