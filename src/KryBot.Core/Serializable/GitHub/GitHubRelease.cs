using System.Collections.Generic;

namespace KryBot.Core.Serializable.GitHub
{
	public class GitHubRelease
	{
		public string tag_name { get; set; }
		public List<GitHunReleaseAssets> assets { get; set; }
	}
}