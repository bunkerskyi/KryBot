﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using KryBot.Properties;

namespace KryBot
{
    public partial class FormBlackList : Form
    {
        private Classes.Bot bot;
        public FormBlackList(Classes.Bot bot)
        {
            this.bot = bot;
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
                    toolStripStatusLabel.Text = $"Количество: {strings.Length}";
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
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
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

        private async void профильSteamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bot.SteamEnabled && bot.SteamProfileLink != "")
            {
                toolStripStatusLabel.Image = Resources.load;
                var list = await Parse.SteamGetUserGames(bot.SteamProfileLink);

                if (list.Count > 0)
                {
                    foreach (string item in listBox.Items)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (item == list[i])
                            {
                                list.Remove(list[i]);
                                i--;
                            }    
                        }
                    }

                    foreach (var id in list)
                    {
                        listBox.Items.Add(id);
                    }
                }
                toolStripStatusLabel.Image = null;
                toolStripStatusLabel.Text = $"Количество: {listBox.Items.Count}";
            }
        }
    }
}
