using System.Collections.Generic;
using System.Xml.Serialization;

namespace KryBot.Core.Json.Steam
{
	[XmlRoot(ElementName = "games")]
	public class ProfileGames
	{
		[XmlElement(ElementName = "game")]
		public List<ProfileGame> Game { get; set; }
	}
}