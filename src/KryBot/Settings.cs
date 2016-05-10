using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace KryBot
{
    public class Settings
    {
        public bool LogActive { get; set; }
        public string Lang { get; set; }
        public bool Sort { get; set; }
        public bool SortToMore { get; set; }
        public bool SortToLess { get; set; }
        public bool Timer { get; set; }
        public bool Autorun { get; set; }
        public bool ShowWonTip { get; set; }
        public bool ShowFarmTip { get; set; }
        public bool FullLog { get; set; }
        public bool WishlistSort { get; set; }
        public int TimerInterval { get; set; }
        public int TimerLoops { get; set; }

        public void Load()
        {
            Settings settings;
            try
            {
                using (var reader = new StreamReader("settings.xml"))
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    settings = (Settings) serializer.Deserialize(reader);
                }
            }
            catch (Exception)
            {
                settings = new Settings();
            }

            Properties.Settings.Default.Autorun = settings.Autorun;
            Properties.Settings.Default.Sort = settings.Sort;
            Properties.Settings.Default.SortToLess = settings.SortToLess;
            Properties.Settings.Default.SortToMore = settings.SortToMore;
            Properties.Settings.Default.Timer = settings.Timer;
            Properties.Settings.Default.TimerInterval = settings.TimerInterval;
            Properties.Settings.Default.TimerLoops = settings.TimerLoops;
            Properties.Settings.Default.LogActive = settings.LogActive;
            Properties.Settings.Default.FullLog = settings.FullLog;
            Properties.Settings.Default.WishlistNotSort = settings.WishlistSort;
            Properties.Settings.Default.Lang = settings.Lang;
            Properties.Settings.Default.ShowFarmTip = settings.ShowFarmTip;
            Properties.Settings.Default.ShowWonTip = settings.ShowWonTip;
            Properties.Settings.Default.Save();
        }

        public void Save()
        {
            var settings = new Settings
            {
                Lang = Properties.Settings.Default.Lang,
                LogActive = Properties.Settings.Default.LogActive,
                FullLog = Properties.Settings.Default.FullLog,
                Timer = Properties.Settings.Default.Timer,
                TimerInterval = Properties.Settings.Default.TimerInterval,
                TimerLoops = Properties.Settings.Default.TimerLoops,
                SortToLess = Properties.Settings.Default.SortToLess,
                SortToMore = Properties.Settings.Default.SortToMore,
                Sort = Properties.Settings.Default.Sort,
                ShowFarmTip = Properties.Settings.Default.ShowFarmTip,
                ShowWonTip = Properties.Settings.Default.ShowWonTip,
                Autorun = Properties.Settings.Default.Autorun,
                WishlistSort = Properties.Settings.Default.WishlistNotSort
            };

            try
            {
                using (var fs = new FileStream("settings.xml", FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(fs, settings);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Ошибка", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}