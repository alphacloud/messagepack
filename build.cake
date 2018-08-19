// ADDINS
#addin "Cake.FileHelpers"
#addin "Cake.Incubator"
#addin "Cake.Issues"
#addin nuget:?package=Cake.AppVeyor

// TOOLS
#tool "GitReleaseManager"
// #tool "GitVersion.CommandLine"
#tool "nuget:?package=GitVersion.CommandLine&prerelease"

// ARGUMENTS
var target = Argument("target", "Default");
if (string.IsNullOrWhiteSpace(target))
{
    target = "Default";
}

var buildConfig = Argument("buildConfig", "Release");
if (string.IsNullOrEmpty(buildConfig)) {
    buildConfig = "Release";
}

// Build configuration

var repoOwner = "alphacloud";
var repoName = "messagepack";

var local = BuildSystem.IsLocalBuild;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var isRepository = StringComparer.OrdinalIgnoreCase.Equals($"{repoOwner}/{repoName}", AppVeyor.Environment.Repository.Name);

var isDebugBuild = string.Equals(buildConfig, "Debug", StringComparison.OrdinalIgnoreCase);
var isReleaseBuild = string.Equals(buildConfig, "Release", StringComparison.OrdinalIgnoreCase);

var isDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", AppVeyor.Environment.Repository.Branch);
var isReleaseBranch = AppVeyor.Environment.Repository.Branch.IndexOf("releases/", StringComparison.OrdinalIgnoreCase) >= 0
    || AppVeyor.Environment.Repository.Branch.IndexOf("hotfixes/", StringComparison.OrdinalIgnoreCase) >= 0;

var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;
var appVeyorJobId = AppVeyor.Environment.JobId;

// Solution settings
// Nuget packages to build
var nugetPackages = new [] {
    "Alphacloud.MessagePack.AspNetCore.Formatters"
};

// Calculate version and commit hash
GitVersion semVersion = GitVersion();
var nugetVersion = semVersion.NuGetVersion;
var buildVersion = semVersion.FullBuildMetaData;
var informationalVersion = semVersion.InformationalVersion;
var nextMajorRelease = $"{semVersion.Major+1}.0.0";
var commitHash = semVersion.Sha;
var milestone = semVersion.MajorMinorPatch;

// Artifacts
var artifactsDir = "./artifacts";
var packagesDir = artifactsDir + "/packages";
var srcDir = "./src";
var testsRootDir = srcDir + "/Tests";
var solutionFile = srcDir + "/Alphacloud.MessagePack.sln";
var samplesDir = "./samples";

Credentials githubCredentials = null;

public class Credentials {
    public string UserName { get; set; }
    public string Password { get; set; }

    public Credentials(string userName, string password) {
        UserName = userName;
        Password = password;
    }
}


// SETUP / TEARDOWN

Setup((context) =>
{
    Information("Building version {0} (tagged: {1}, local: {2}, release branch: {3})...", nugetVersion, isTagged, local, isReleaseBranch);
    CreateDirectory(artifactsDir);
    CleanDirectory(artifactsDir);
    githubCredentials = new Credentials(
      context.EnvironmentVariable("GITHUB_USER"),
      context.EnvironmentVariable("GITHUB_PASSWORD")
    );
});

Teardown((context) =>
{
    // Executed AFTER the last task.
});

Task("SetVersion")
    .Does(() =>
    {
        CreateAssemblyInfo("./src/common/AssemblyVersion.cs", new AssemblyInfoSettings{
            FileVersion = semVersion.MajorMinorPatch,
            InformationalVersion = semVersion.InformationalVersion,
            Version = semVersion.MajorMinorPatch
        });
    });


Task("UpdateAppVeyorBuildNumber")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
    {
        AppVeyor.UpdateBuildVersion(buildVersion);

    }).ReportError(exception =>
    {
        // When a build starts, the initial identifier is an auto-incremented value supplied by AppVeyor.
        // As part of the build script, this version in AppVeyor is changed to be the version obtained from
        // GitVersion. This identifier is purely cosmetic and is used by the core team to correlate a build
        // with the pull-request. In some circumstances, such as restarting a failed/cancelled build the
        // identifier in AppVeyor will be already updated and default behaviour is to throw an
        // exception/cancel the build when in fact it is safe to swallow.
        // See https://github.com/reactiveui/ReactiveUI/issues/1262

        Warning("Build with version {0} already exists.", buildVersion);
    });


Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore(srcDir);
    });


Task("Build")
    .IsDependentOn("SetVersion")
    .IsDependentOn("UpdateAppVeyorBuildNumber")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetCoreBuild(srcDir, new DotNetCoreBuildSettings {
            NoRestore = true,
            Configuration = buildConfig,
        });
    });


Task("CreateNugetPackages")
    .Does(() => {
        Action<string> buildPackage = (string projectName) => {

            DotNetCorePack($"{srcDir}/lib/{projectName}/{projectName}.csproj", new DotNetCorePackSettings {
                Configuration = buildConfig,
                OutputDirectory = packagesDir,
                NoBuild = true,
                ArgumentCustomization = args => args.Append($"-p:Version={nugetVersion}")
            });
        };

        foreach(var projectName in nugetPackages) {
            buildPackage(projectName);
        };
    });

Task("CreateRelease")
    .WithCriteria(() => isRepository && isReleaseBranch && !isPullRequest)
    .Does(() => {
        GitReleaseManagerCreate(githubCredentials.UserName, githubCredentials.Password, repoOwner, repoName,
            new GitReleaseManagerCreateSettings {
              Milestone = milestone,
              TargetCommitish = "master"
        });
    });

Task("CloseMilestone")
    .WithCriteria(() => isRepository && isTagged && !isPullRequest)
    .Does(() => {
        GitReleaseManagerClose(githubCredentials.UserName, githubCredentials.Password, repoOwner, repoName, milestone);
    });

Task("Default")
    .IsDependentOn("UpdateAppVeyorBuildNumber")
    .IsDependentOn("Build")
    .IsDependentOn("CreateNugetPackages")
    .IsDependentOn("CreateRelease")
    .IsDependentOn("CloseMilestone")
    .Does(
        () => {}
    );


// EXECUTION
RunTarget(target);
