using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KryBot.CommonResources.Localization;
using KryBot.Core;
using KryBot.Gui.WinFormsGui.Properties;

namespace KryBot.Gui.WinFormsGui.Forms
{
    public partial class FormBlackList : Form
    {
        private readonly Bot _bot;

        public FormBlackList(Bot bot)
        {
            _bot = bot;
            InitializeComponent();
            Localization();
            listView.ListViewItemSorter = new ListViewItemComparer();
        }

        private void formBlackList_Load(object sender, EventArgs e)
        {
            Design();

            if (_bot.Blacklist != null)
            {
                foreach (var item in _bot.Blacklist.Items)
                {
                    listView.Items.Add(item.Id).SubItems.Add(item.Name);
                }
            }
            toolStripStatusLabel.Text = $"{strings.Count}: {listView.Items.Count}";
        }

        private void Design()
        {
            listView.Columns.Add("ID");
            listView.Columns.Add("Name", Width - listView.Columns[0].Width);
        }

        private void SaveBlackList()
        {
            if (listView.Items.Count > 0)
            {
                _bot.Blacklist.Items.Clear();
                foreach (ListViewItem lvitem in listView.Items)
                {
                    _bot.Blacklist.Items.Add(new Blacklist.BlacklistItem
                    {
                        Id = lvitem.SubItems[0].Text,
                        Name = lvitem.SubItems[1].Text
                    });
                }
                _bot.Save();
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
                toolStripStatusLabel.Text = $"{strings.Count}: {listView.Items.Count}";
            }
        }

        private async void профильSteamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_bot.Steam.Enabled && _bot.Steam.ProfileLink != "")
            {
                toolStripStatusLabel.Image = Resources.load;
                var list = await _bot.Steam.GetUserGames();

                if (list.Games.Game.Count > 0)
                {
                    foreach (ListViewItem item in listView.Items)
                    {
                        for (var i = 0; i < list.Games.Game.Count; i++)
                        {
                            if (item.Text == list.Games.Game[i].AppId)
                            {
                                list.Games.Game.Remove(list.Games.Game[i]);
                                i--;
                            }
                        }
                    }

                    foreach (var game in list.Games.Game)
                    {
                        listView.Items.Add(game.AppId).SubItems.Add(game.Name);
                    }
                }

                toolStripStatusLabel.Image = null;
                toolStripStatusLabel.Text = $"{strings.Count}: {listView.Items.Count}";
            }
            else
            {
                MessageBox.Show(strings.Blacklist_NeedAuth, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormBlackList_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveBlackList();
        }

        private async void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new TextBoxForm(strings.BlacklistForm_EnterId, true);
            form.ShowDialog();
            if (Properties.Settings.Default._idCache != "0")
            {
                listView.Items.Add(Properties.Settings.Default._idCache)
                    .SubItems.Add(await LoadName(Properties.Settings.Default._idCache));
                Properties.Settings.Default._idCache = "0";
            }
            toolStripStatusLabel.Text = $"{strings.Count}: {listView.Items.Count}";
        }

        private async Task<string> LoadName(string id)
        {
            toolStripStatusLabel.Image = Resources.load;
            var name = await _bot.Steam.GetGameName(id) ?? "";
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

        private void FormBlackList_Resize(object sender, EventArgs e)
        {
            listView.Columns[1].Width = listView.Width - listView.Columns[0].Width;
        }

        private async void steamGiftsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(_bot.SteamGifts.Enabled)
            {
                toolStripStatusLabel.Image = Resources.load;
                var list = await _bot.SteamGifts.GetBlaclList();

                if(list != null && list.Count > 0)
                {
                    foreach(ListViewItem item in listView.Items)
                    {
                        for(var i = 0; i < list.Count; i++)
                        {
                            if(item.Text == list.ElementAt(i).Key.ToString())
                            {
                                list.Remove(list.ElementAt(i).Key);
                                i--;
                            }
                        }
                    }

                    foreach(var game in list.Keys)
                    {
                        listView.Items.Add(game.ToString()).SubItems.Add(list[game]);
                    }
                }

                toolStripStatusLabel.Image = null;
                toolStripStatusLabel.Text = $"{strings.Count}: {listView.Items.Count}";
            }
            else
            {
                MessageBox.Show(strings.Blacklist_NeedAuth, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}