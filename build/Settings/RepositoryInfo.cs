namespace _build
{
    using System;
    using Nuke.Common.CI.AppVeyor;


    public class RepositoryInfo {
        public bool IsPullRequest { get; protected set; }
        public bool IsMain { get; protected set; }
        public bool IsDevelopBranch { get; protected set; }
        // Release or hotfix branch
        public bool IsReleaseBranch { get; protected set; }
        public bool IsTagged { get; protected set; }

        public static RepositoryInfo Get(BuildBase buildSystem, ProjectSettings settings) {

            

            return new RepositoryInfo {
                IsPullRequest = AppVeyor.Instance.PullRequestNumber > 0,
                IsDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", AppVeyor.Instance.RepositoryBranch),
                IsReleaseBranch = AppVeyor.Instance.RepositoryBranch.IndexOf("releases/", StringComparison.OrdinalIgnoreCase) >= 0
                    || AppVeyor.Instance.RepositoryBranch.IndexOf("hotfixes/", StringComparison.OrdinalIgnoreCase) >= 0,
                IsTagged = AppVeyor.Instance.RepositoryTag,
                IsMain = StringComparer.OrdinalIgnoreCase.Equals($"{settings.RepoOwner}/{settings.RepoName}", AppVeyor.Instance.RepositoryName),
            };
        }
    }
}
