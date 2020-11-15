namespace _build
{
    using System;
    using Nuke.Common;
    using Nuke.Common.Tools.GitVersion;

    // params

    // default paths and files


    public class BuildInfo
    {
        //public string Target { get; protected set; }
        //public string Config { get; protected set; }

        public Configuration Configuration { get; protected set;}

        public bool IsLocal { get; protected set; }
        public string AppVeyorJobId { get; protected set; }

        public BuildVersion Version { get; protected set; }

        public RepositoryInfo Repository { get; protected set; }

        public string GitHubToken { get; protected set; }

        public Paths Paths { get; protected set; }

        public ProjectSettings Settings { get; protected set; }

        public static BuildInfo Get(BuildBase buildBase, ProjectSettings settings)
        {
            // Calculate version and commit hash
            var version = new BuildVersion(
                buildBase.GitVersion.NuGetVersion,
                buildBase.GitVersion.FullBuildMetaData,
                buildBase.GitVersion.InformationalVersion,
                $"{buildBase.GitVersion.Major + 1}.0.0",
                buildBase.GitVersion.Sha,
                buildBase.GitVersion.MajorMinorPatch
            );

            var gitHubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

            return new BuildInfo
            {
                Configuration = buildBase.Configuration,
                IsLocal = NukeBuild.IsLocalBuild,
                AppVeyorJobId = Environment.GetEnvironmentVariable("APPVEYOR_JOB_ID"),
                Version = version,
                Repository = RepositoryInfo.Get(buildBase, settings),
                GitHubToken = gitHubToken,
                Settings = settings,
                Paths = new Paths(buildBase),
            };
        }
    }
