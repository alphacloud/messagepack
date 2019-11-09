// params
public class Params {
  public string RepoOwner { get; set; }
  public string RepoName {get; set;}
}



public class BuildInfo {
  public string Target { get; set; }
  public string Config { get; set; }

  public bool IsDebug { get; set; }
  public bool IsRelease {get;set;}

  public bool IsLocal { get; set; }
  public bool IsPullRequest { get; set; }
  public bool IsRepository { get; set; }
  public bool IsDevelopBranch { get; set; }
  public bool IsReleaseBranch { get; set; }

  public bool IsTagged { get; set; }
  public string AppVeyorJobId { get; set; }


  public static BuildInfo Get(ICakeContext context)
  {
    if (context == null)
      throw new ArgumentNullException(nameof(context));
    var target = context.Argument("target", "Default");
    var config = context.Argument("buildConfig", "Release");
    var buildSystem = context.BuildSystem();

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
    };
  }
}  
