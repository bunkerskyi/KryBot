using System;
using System.Drawing;
using System.Windows.Forms;
using KryBot.Properties;

namespace KryBot
{
    public partial class FormTextBox : Form
    {
        private readonly string _formTitle;
        private readonly bool _isInteger;

        public FormTextBox(string formTitle, bool isInteger)
        {
            _isInteger = isInteger;
            _formTitle = formTitle;
            InitializeComponent();
        }

        private void FormTextBox_Load(object sender, EventArgs e)
        {
            Text = _formTitle;
            Icon = Icon.FromHandle(Resources.blocked.GetHicon());
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_isInteger)
            {
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default._idCache = textBox.Text;
            Close();
        }
    }
}