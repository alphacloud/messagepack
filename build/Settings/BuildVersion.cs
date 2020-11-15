namespace _build
{
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
}