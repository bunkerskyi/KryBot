/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System.Collections.Generic;
using System.Xml.Serialization;

namespace KryBot.Core.Serializable.Steam
{
    [XmlRoot(ElementName = "games")]
    public class ProfileGames
    {
        [XmlElement(ElementName = "game")]
        public List<ProfileGame> Game { get; set; }
    }
}