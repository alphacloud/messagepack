namespace _build
{
    using System;


    public class RepositoryInfo {
        public bool IsPullRequest { get; protected set; }
        public bool IsMain { get; protected set; }
        public bool IsDevelopBranch { get; protected set; }
        // Release or hotfix branch
        public bool IsReleaseBranch { get; protected set; }
        public bool IsTagged { get; protected set; }

        public static RepositoryInfo Get(BuildBase buildSystem, ProjectSettings settings) {
            return new RepositoryInfo {
                IsPullRequest = buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest,
                IsDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", buildSystem.AppVeyor.Environment.Repository.Branch),
                IsReleaseBranch = buildSystem.AppVeyor.Environment.Repository.Branch.IndexOf("releases/", StringComparison.OrdinalIgnoreCase) >= 0
                    || buildSystem.AppVeyor.Environment.Repository.Branch.IndexOf("hotfixes/", StringComparison.OrdinalIgnoreCase) >= 0,
                IsTagged = buildSystem.AppVeyor.Environment.Repository.Tag.IsTag,
                IsMain = StringComparer.OrdinalIgnoreCase.Equals($"{settings.RepoOwner}/{settings.RepoName}", buildSystem.AppVeyor.Environment.Repository.Name),
            };
        }
    }
}
