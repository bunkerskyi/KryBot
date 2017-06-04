/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System.Xml.Serialization;

namespace KryBot.Core.Serializable.Steam
{
    [XmlRoot(ElementName = "profile")]
    public class Profile
    {
        [XmlElement(ElementName = "steamID64")]
        public string SteamId64 { get; set; }
    }
}