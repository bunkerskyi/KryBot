using System.Collections.Generic;

namespace KryBot_Updater
{
    class Classes
    {
        public class GitHubRelease
        {
            public string tag_name { get; set; }
            public bool prerelease { get; set; }
            public List<GitHunReleaseAssets> assets { get; set; }
            public string body { get; set; }
        }

        public class GitHunReleaseAssets
        {
            public string name { get; set; }
            public int size { get; set; }
            public string browser_download_url { get; set; }
        }
    }
}
