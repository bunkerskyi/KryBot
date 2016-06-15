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