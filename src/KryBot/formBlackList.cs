using System;
using System.Collections;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using KryBot.lang;
using KryBot.Properties;

namespace KryBot
{
    public partial class FormBlackList : Form
    {
        private readonly Bot _bot;

        public FormBlackList(Bot bot)
        {
            _bot = bot;
            InitializeComponent();
            listView.ListViewItemSorter = new ListViewItemComparer();
        }

        private void formBlackList_Load(object sender, EventArgs e)
        {
            Design();
            var blacklist = Tools.LoadBlackList();

            if (blacklist.Items != null)
            {
                foreach (var item in blacklist.Items)
                {
                    listView.Items.Add(item.Id).SubItems.Add(item.Name);
                }
            }
            toolStripStatusLabel.Text = $"Количество: {listView.Items.Count}";
        }

        private void Design()
        {
            Text = @"Черный список";
            Icon = Icon.FromHandle(Resources.blocked.GetHicon());

            listView.Columns.Add("ID");
            listView.Columns.Add("Name", Width - listView.Columns[0].Width);
        }

        private void SaveBlackList()
        {
            if (listView.Items.Count > 0)
            {
                var blacklist = new Blacklist();

                foreach (ListViewItem lvitem in listView.Items)
                {
                    var item = new Blacklist.BlacklistItem
                    {
                        Id = lvitem.SubItems[0].Text,
                        Name = lvitem.SubItems[1].Text
                    };
                    blacklist.Items.Add(item);
                }

                blacklist.Save();
            }
        }

        private void удалитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                for (var i = 0; i < listView.SelectedItems.Count; i++)
                {
                    listView.Items.Remove(listView.Items[listView.SelectedItems[i].Index]);
                    i--;
                }
                toolStripStatusLabel.Text = $"Количество: {listView.Items.Count}";
            }
        }

        private async void профильSteamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_bot.Steam.Enabled && _bot.Steam.ProfileLink != "")
            {
                toolStripStatusLabel.Image = Resources.load;
                var list = await Parse.SteamGetUserGames(_bot.Steam.ProfileLink);

                if (list.Games.Game.Count > 0)
                {
                    foreach (ListViewItem item in listView.Items)
                    {
                        for (var i = 0; i < list.Games.Game.Count; i++)
                        {
                            if (item.Text == list.Games.Game[i].AppID)
                            {
                                list.Games.Game.Remove(list.Games.Game[i]);
                                i--;
                            }
                        }
                    }

                    foreach (var game in list.Games.Game)
                    {
                        listView.Items.Add(game.AppID).SubItems.Add(game.Name);
                    }
                }

                toolStripStatusLabel.Image = null;
                toolStripStatusLabel.Text = $"Количество: {listView.Items.Count}";
            }
            else
            {
                MessageBox.Show(@"Нужна авторизация в Steam", strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormBlackList_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveBlackList();
        }

        private async void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormTextBox("Enter id", true);
            form.ShowDialog();
            if (Properties.Settings.Default._idCache != "0")
            {
                listView.Items.Add(Properties.Settings.Default._idCache)
                    .SubItems.Add(await LoadName(Properties.Settings.Default._idCache));
                Properties.Settings.Default._idCache = "0";
            }
            toolStripStatusLabel.Text = $"Количество: {listView.Items.Count}";
        }

        private async Task<string> LoadName(string id)
        {
            toolStripStatusLabel.Image = Resources.load;
            var name = await Parse.SteamGetGameName(id) ?? "";
            toolStripStatusLabel.Image = null;
            return name;
        }

        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var comparer = (ListViewItemComparer) listView.ListViewItemSorter;
            comparer.Order = e.Column == comparer.Column ? comparer.Swap(comparer.Order) : SortOrder.Ascending;
            comparer.Column = e.Column;
            listView.Sort();
        }

        private class ListViewItemComparer : IComparer
        {
            public int Column;
            public SortOrder Order;

            public int Compare(object x, object y)
            {
                if (Column == 0)
                {
                    var int1 = int.Parse(((ListViewItem) x).SubItems[Column].Text);
                    var int2 = int.Parse(((ListViewItem) y).SubItems[Column].Text);

                    return Order == SortOrder.Ascending ? int1.CompareTo(int2) : int2.CompareTo(int1);
                }

                if (Column == 1)
                {
                    var str1 = ((ListViewItem) x).SubItems[Column].Text;
                    var str2 = ((ListViewItem) y).SubItems[Column].Text;

                    return Order == SortOrder.Ascending
                        ? string.Compare(str1, str2, StringComparison.Ordinal)
                        : string.Compare(str2, str1, StringComparison.Ordinal);
                }

                return 0;
            }

            public SortOrder Swap(SortOrder a)
            {
                if (a == SortOrder.Ascending)
                {
                    return SortOrder.Descending;
                }

                if (a == SortOrder.Descending)
                {
                    return SortOrder.Ascending;
                }

                return SortOrder.Ascending;
            }
        }
    }
}