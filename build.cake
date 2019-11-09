// ADDINS
#addin nuget:?package=Cake.Coveralls&version=0.10.0
#addin nuget:?package=Cake.FileHelpers&version=3.2.1
#addin nuget:?package=Cake.Incubator&version=5.1.0
#addin nuget:?package=Cake.Issues&version=0.7.1
#addin nuget:?package=Cake.AppVeyor&version=4.0.0

// TOOLS
#tool nuget:?package=GitReleaseManager&version=0.8.0
#tool nuget:?package=GitVersion.CommandLine&version=5.0.1
#tool nuget:?package=coveralls.io&version=1.4.2
#tool nuget:?package=OpenCover&version=4.7.922
#tool nuget:?package=ReportGenerator&version=4.2.17

// ARGUMENTS

#load "defaults.cake"


var target = Argument("target", "Default");

// Build configuration

var repoOwner = "alphacloud";
var repoName = "messagepack";

// Solution settings

// paths
// Artifacts
var artifactsDir = "./artifacts";
var artifactsDirAbsolutePath = MakeAbsolute(Directory(artifactsDir));
var testCoverageOutputFile = artifactsDir + "/OpenCover.xml";
var codeCoverageReportDir = artifactsDir + "/CodeCoverageReport";
var coverageExcludeByAttribute = "*.ExcludeFromCodeCoverage*";
var coverageExcludeByFile = "*/*Designer.cs";
var packagesDir = artifactsDir + "/packages";
var srcDir = "./src";
var buildPropsFile = srcDir + "/Directory.Build.props";
var testsRootDir = srcDir + "/tests";
var samplesDir = "./samples";
var commonAssemblyVersionFile = srcDir + "/common/AssemblyVersion.cs";


var coverageFilter = "+[Alphacloud.MessagePack.AspNetCore.Formatters]* -[Tests*]*";
var solutionFile = srcDir + "/Alphacloud.MessagePack.sln";

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

Setup<BuildInfo>(context =>
{
    var buildInfo = BuildInfo.Get(context);

    Information("Building version {0} (tagged: {1}, local: {2}, release branch: {3})...", buildInfo.Version.NuGet, 
        buildInfo.IsTagged, buildInfo.IsLocal, buildInfo.IsReleaseBranch);
    CreateDirectory(artifactsDir);
    CleanDirectory(artifactsDir);
    githubCredentials = new Credentials(
      context.EnvironmentVariable("GITHUB_USER"),
      context.EnvironmentVariable("GITHUB_PASSWORD")
    );

    return buildInfo;
});

Teardown((context) =>
{
    // Executed AFTER the last task.
});

Task("SetVersion")
    .Does<BuildInfo>(build =>
    {
        CreateAssemblyInfo(commonAssemblyVersionFile, new AssemblyInfoSettings{
            FileVersion = build.Version.NuGet,
            InformationalVersion = build.Version.Informational,
            Version = build.Version.Milestone
        });
    });


Task("UpdateAppVeyorBuildNumber")
    .WithCriteria(() => AppVeyor.IsRunningOnAppVeyor)
    .ContinueOnError()
    .Does<BuildInfo>(build =>
    {
        AppVeyor.UpdateBuildVersion(build.Version.Full);
    });


Task("Restore")
    .Does(() =>
    {
        DotNetCoreRestore(srcDir);
    });


Task("RunXunitTests")
    .DoesForEach<BuildInfo, FilePath>(GetFiles($"{testsRootDir}/**/*.csproj"), 
    (build, testProj, ctx) => {
        var projectPath = testProj.GetDirectory();
        var projectFilename = testProj.GetFilenameWithoutExtension();
        Information("Calculating code coverage for {0} ...", projectFilename);

        var openCoverSettings = new OpenCoverSettings {
            OldStyle = true,
            ReturnTargetCodeOffset = 0,
            ArgumentCustomization = args => args.Append("-mergeoutput").Append("-hideskipped:File;Filter;Attribute"),
            WorkingDirectory = projectPath,
        }
        .WithFilter(coverageFilter)
        .ExcludeByAttribute(coverageExcludeByAttribute)
        .ExcludeByFile(coverageExcludeByFile);

        Func<string,ProcessArgumentBuilder> buildProcessArgs = (buildCfg) => {
            var pb = new ProcessArgumentBuilder()
                .AppendSwitch("--configuration", buildCfg)
                .AppendSwitch("--filter", "Category!=IntegrationTests")
                .AppendSwitch("--results-directory", artifactsDirAbsolutePath.FullPath)
                .Append("--no-restore")
                .Append("--no-build");
            if (!build.IsLocal) {
                pb.AppendSwitch("--test-adapter-path", ".")
                    .AppendSwitch("--logger", "AppVeyor");
            }
            else {
                pb.AppendSwitch("--logger", $"trx;LogFileName={projectFilename}.trx");
            }
            return pb;
        };

        // run open cover for debug build configuration
        OpenCover(
            tool => tool.DotNetCoreTool(projectPath.ToString(),
                "test",
                buildProcessArgs("Debug")
            ),
            testCoverageOutputFile,
            openCoverSettings);

        // run tests again if Release mode was requested
        if (build.IsRelease) {
            Information("Running Release mode tests for {0}", projectFilename.ToString());
            DotNetCoreTool(testProj.FullPath,
                "test",
                buildProcessArgs("Release")
            );
        }
    })
    .DeferOnError();

Task("CleanPreviousTestResults")
    .Does(() =>
    {
        if (FileExists(testCoverageOutputFile))
            DeleteFile(testCoverageOutputFile);
        DeleteFiles(artifactsDir + "/*.trx");
        if (DirectoryExists(codeCoverageReportDir))
            DeleteDirectory(codeCoverageReportDir, recursive: true);
    });

Task("GenerateCoverageReport")
    .WithCriteria<BuildInfo>((ctx, build) => build.IsLocal)
    .Does<BuildInfo>(build =>
    {
        ReportGenerator(testCoverageOutputFile, codeCoverageReportDir);
    });

Task("UploadCoverage")
    .WithCriteria<BuildInfo>((ctx, build) => !build.IsLocal)
    .Does<BuildInfo>(build => {
        CoverallsIo(testCoverageOutputFile);
    });

Task("RunUnitTests")
    .IsDependentOn("Build")
    .IsDependentOn("CleanPreviousTestResults")
    .IsDependentOn("RunXunitTests")
    .IsDependentOn("GenerateCoverageReport")
    .IsDependentOn("UploadCoverage")
    .Does<BuildInfo>(build =>
    {
        Information("Done Test");
    });

Task("UpdateReleaseNotesLink")
    .WithCriteria<BuildInfo>((ctx, build) => build.IsTagged)
    .Does<BuildInfo>(build => {
        var releaseNotes = $"https://github.com/{repoOwner}/{repoName}/releases/tag/{build.Version.Milestone}";
        Information("Updating ReleaseNotes Link to {1}", releaseNotes);
        XmlPoke(buildPropsFile,
            "/Project/PropertyGroup[@Label=\"Package\"]/PackageReleaseNotes",
            releaseNotes
        );
    });


Task("Build")
    .IsDependentOn("SetVersion")
    .IsDependentOn("UpdateAppVeyorBuildNumber")
    .IsDependentOn("UpdateReleaseNotesLink")
    .IsDependentOn("Restore")
    .Does<BuildInfo>(build =>
    {
        if (build.IsRelease) {
            Information("Running {0} build for code coverage", "Debug");
            // need Debug build for code coverage
            DotNetCoreBuild(srcDir, new DotNetCoreBuildSettings {
                NoRestore = true,
                Configuration = "Debug",
            });
        }
        Information("Running {0} build", build.Config);
        DotNetCoreBuild(srcDir, new DotNetCoreBuildSettings {
            NoRestore = true,
            Configuration = build.Config,
        });
    });



Task("CreateNugetPackages")
    .Does<BuildInfo>(build => {
        DotNetCorePack(srcDir, new DotNetCorePackSettings {
            Configuration = build.Config,
            OutputDirectory = packagesDir,
            NoRestore = true,
            NoBuild = true,
            ArgumentCustomization = args => args.Append($"-p:Version={build.Version.NuGet}")
        });
    });

Task("CreateRelease")
    .WithCriteria<BuildInfo>((ctx, build) => build.IsRepository && build.IsReleaseBranch && !build.IsPullRequest)
    .Does<BuildInfo>(build => {
        GitReleaseManagerCreate(githubCredentials.UserName, githubCredentials.Password, repoOwner, repoName,
            new GitReleaseManagerCreateSettings {
              Milestone = build.Version.Milestone,
              TargetCommitish = "master"
        });
    });

Task("CloseMilestone")
    .WithCriteria<BuildInfo>((ctx, build) => build.IsRepository && build.IsTagged && !build.IsPullRequest)
    .Does<BuildInfo>(build => {
        GitReleaseManagerClose(githubCredentials.UserName, githubCredentials.Password, repoOwner, repoName, build.Version.Milestone);
    });

Task("Default")
    .IsDependentOn("UpdateAppVeyorBuildNumber")
    .IsDependentOn("Build")
    .IsDependentOn("RunUnitTests")
    .IsDependentOn("CreateNugetPackages")
    .IsDependentOn("CreateRelease")
    .IsDependentOn("CloseMilestone")
    .Does(
        () => {}
    );


// EXECUTION
RunTarget(target);
