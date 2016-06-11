using System.Xml.Serialization;

namespace KryBot.Core.Json.Steam
{
	[XmlRoot(ElementName = "gamesList")]
	public class ProfileGamesList
	{
		[XmlElement(ElementName = "games")]
		public ProfileGames Games { get; set; }
	}
}