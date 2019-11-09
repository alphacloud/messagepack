public class Params {
    public string RepoOwner { get; set; }
    public string RepoName {get;set;}
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
    public string AppveyorJobId { get; set; }


    public static BuildInfo Get(ICakeContext context, Params p) 
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
        var target = context.Argument("target", "Default");
        va config = context.Argument("buildConfig", "Release");
        var buildSystem = context.BuildSystem();

        return new BuildInfo {
            Target = target,
            Config = config,
            IsDebug = string.Equals(buildConfig, "Debug", StringComparison.OrdinalIgnoreCase),
            IsRelease = string.Equals(buildConfig, "Release", StringComparison.OrdinalIgnoreCase),
            IsLocalBuild = buildSystem.IsLocalBuild,
            IsPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest,
            IsDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("develop", AppVeyor.Environment.Repository.Branch),
            IsReleaseBranch = AppVeyor.Environment.Repository.Branch.IndexOf("releases/", StringComparison.OrdinalIgnoreCase) >= 0
    || AppVeyor.Environment.Repository.Branch.IndexOf("hotfixes/", StringComparison.OrdinalIgnoreCase) >= 0;

var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;
var appVeyorJobId = AppVeyor.Environment.JobId;


        }
    }
}