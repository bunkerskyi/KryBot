using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace KryBot
{
    public class Blacklist
    {
        public Blacklist()
        {
            Items = new List<BlacklistItem>();    
        }

        public List<BlacklistItem> Items { get; set; }

        public class BlacklistItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public bool Save()
        {
            try
            {
                using (var fs = new FileStream("blacklist.xml", FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof(Blacklist));
                    serializer.Serialize(fs, this);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Ошибка", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
