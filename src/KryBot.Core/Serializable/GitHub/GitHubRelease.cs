using System.Collections.Generic;

namespace KryBot.Core.Serializable.GitHub
{
	public class GitHubRelease
	{
		public string tag_name { get; set; }
		public bool prerelease { get; set; }
		public List<GitHunReleaseAssets> assets { get; set; }
		public string body { get; set; }
	}
}