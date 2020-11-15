namespace _build
{
    using System;
    using Settings;


    public class ProjectSettings {
        public string RepoOwner { get; set; }
        public string RepoName { get; set; }
        public string SolutionName { get; set; }

        public CodeCoverageSettings CodeCoverage { get; }

        public ProjectSettings(string repoOwner, string repoName, string solutionName)
        {
            if (string.IsNullOrEmpty(repoOwner))
                throw new ArgumentNullException(nameof(repoOwner), "Value cannot be null or empty.");
            if (string.IsNullOrEmpty(repoName))
                throw new ArgumentNullException(nameof(repoName), "Value cannot be null or empty.");
            if (string.IsNullOrEmpty(solutionName))
                throw new ArgumentNullException(nameof(solutionName), "Value cannot be null or empty.");

            RepoOwner = repoOwner;
            RepoName = repoName;
            SolutionName = solutionName;

            CodeCoverage = new CodeCoverageSettings {
                IncludeFilter = $"+[solutionName*]*"
            };
        }
    }
}