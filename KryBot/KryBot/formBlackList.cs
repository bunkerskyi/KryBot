using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using KryBot.Properties;

namespace KryBot
{
    public partial class FormBlackList : Form
    {
        public FormBlackList()
        {
            InitializeComponent();
        }

        private void formBlackList_Load(object sender, EventArgs e)
        {
            Design();
            LoadBlackList();
        }

        private void Design()
        {
            Text = @"Черный список";
            Icon = Icon.FromHandle(Resources.blocked.GetHicon());
        }

        private void LoadBlackList()
        {
            if (File.Exists("blacklist.txt"))
            {
                try
                {
                    var strings = File.ReadAllLines("blacklist.txt");
                    foreach (var str in strings)
                    {
                        listBox.Items.Add(str);
                    }
                }
                catch (IOException ex)
                {
                    MessageBox.Show(@"Ошибка", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox.Items.Add(tbId.Text);
            tbId.Text = "";
            tbId.Focus();
        }

        private void tbId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
                e.Handled = true;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveBlackList())
            {
                Close();
            }    
        }

        private bool SaveBlackList()
        {
            try
            {
                StreamWriter saveFile = new StreamWriter("blacklist.txt");
                foreach (var item in listBox.Items)
                {
                    saveFile.WriteLine(item.ToString());
                }
                saveFile.Close();
                return true;
            }
            catch (IOException ex)
            {
                MessageBox.Show(@"Ошибка", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void удалитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex != -1)
            {
                listBox.Items.Remove(listBox.Items[listBox.SelectedIndex]);
            }
        }
    }
}
