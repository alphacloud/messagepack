using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;


public abstract class BuildBase: NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    public readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] public readonly Solution Solution;
    [GitRepository] public readonly GitRepository GitRepository;
    [GitVersion] public readonly GitVersion GitVersion;

//    public AbsolutePath SourceDirectory => RootDirectory / "src";
//    public AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

}

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : BuildBase
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    /// <inheritdoc />
    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();


    }

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target SetVersion => _ => _
        .Executes(() =>
        {
            throw new NotImplementedException();
        });

    Target UpdateAppVeyorBuildNumber => _ => _
        .Executes(() =>
        {
            throw new NotImplementedException();
        });

    Target Restore => _ => _
        .After(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target RunXunitTests => _ => _
        .Executes(() =>
        {
            throw new NotImplementedException();
        });

    Target CleanPreviousTestsResults => _ => _
        .Executes(() =>
        {
            throw new NotImplementedException();
        });

    Target GenerateCoverageReport => _ => _
        .Executes(() =>
        {
            throw new NotImplementedException();
        });

    Target UploadCoverage => _ => _
        .Executes(() =>
        {
            throw new NotImplementedException();
        });


    Target RunUniteTests => _ => _
        .DependsOn(Compile, CleanPreviousTestsResults, RunXunitTests, GenerateCoverageReport, UploadCoverage)
        .Executes(() =>
        {
            throw new NotImplementedException();
        });



    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });


    Target CreateNugetPackages => _ => _
        .Executes(() =>
        {
            throw new NotImplementedException();
        });

    Target CreateRelease => _ => _
        .Executes(() =>
        {
            throw new NotImplementedException();
        });

    Target CloseMilestone => _ => _
        .Executes(() =>
        {
            throw new NotImplementedException();
        });


}
