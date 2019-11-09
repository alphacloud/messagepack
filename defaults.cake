// params
public class Params {
  public string RepoOwner { get; set; }
  public string RepoName {get; set;}
}


public class BuildVersion {
  public string NuGet { get; }
  public string Full { get; }
  public string Informational { get; }
  public string NextMajor { get; }
  public string CommitHash { get; }
  public string Milestone { get; }

  public BuildVersion(string nuget, string full, string informational, string nextMajor, string commitHash, string milestone) {
    NuGet = nuget;
    Full = full;
    Informational = informational;
    NextMajor = nextMajor;
    CommitHash = commitHash;
    Milestone = milestone;
  }
}

public class BuildInfo {
  public string Target { get; protected set; }
  public string Config { get; protected set; }

  public bool IsDebug { get; protected set; }
  public bool IsRelease {get; protected set;}

  public bool IsLocal { get; protected set; }
  public bool IsPullRequest { get; protected set; }
  public bool IsRepository { get; protected set; }
  public bool IsDevelopBranch { get; protected set; }
  public bool IsReleaseBranch { get; protected set; }

  public bool IsTagged { get; protected set; }

  public string AppVeyorJobId { get; protected set; }

  public BuildVersion Version { get; protected set; }

  public static BuildInfo Get(ICakeContext context)
  {
    if (context == null)
      throw new ArgumentNullException(nameof(context));
    var target = context.Argument("target", "Default");
    var config = context.Argument("buildConfig", "Release");
    var buildSystem = context.BuildSystem();

    // Calculate version and commit hash
    GitVersion semVersion = context.GitVersion();
    var version = new BuildVersion(
        semVersion.NuGetVersion,
        semVersion.FullBuildMetaData,
        semVersion.InformationalVersion,
        $"{semVersion.Major+1}.0.0",
        semVersion.Sha,
        semVersion.MajorMinorPatch
    );

    return new BuildInfo {
      Target = target,
      Config = config,
      IsDebug = string.Equals(config, "Debug", StringComparison.OrdinalIgnoreCase),
      IsRelease = string.Equals(config, "Release", StringComparison.OrdinalIgnoreCase),
      IsLocal = buildSystem.IsLocalBuild,
      IsPullRequest = buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest,
      IsDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", buildSystem.AppVeyor.Environment.Repository.Branch),
      IsReleaseBranch = buildSystem.AppVeyor.Environment.Repository.Branch.IndexOf("releases/", StringComparison.OrdinalIgnoreCase) >= 0
       || buildSystem.AppVeyor.Environment.Repository.Branch.IndexOf("hotfixes/", StringComparison.OrdinalIgnoreCase) >= 0,
      IsTagged = buildSystem.AppVeyor.Environment.Repository.Tag.IsTag,
      AppVeyorJobId = buildSystem.AppVeyor.Environment.JobId,
      Version = version
    };
  }
}  
