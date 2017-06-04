/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System;
using System.Windows.Forms;

namespace KryBot.Gui.WinFormsGui.Forms
{
    public partial class TextBoxForm : Form
    {
        private readonly string _formTitle;
        private readonly bool _isInteger;

        public TextBoxForm(string formTitle, bool isInteger)
        {
            _isInteger = isInteger;
            _formTitle = formTitle;
            InitializeComponent();
            Localization();
        }

        private void FormTextBox_Load(object sender, EventArgs e)
        {
            Text = _formTitle;
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