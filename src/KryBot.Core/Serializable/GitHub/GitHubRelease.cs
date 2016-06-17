using System.Collections.Generic;

namespace KryBot.Core.Serializable.GitHub
{
	public class GitHubRelease
	{
		public string tag_name { get;}
		public List<GitHunReleaseAssets> assets { get;}
	}
}