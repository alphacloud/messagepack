namespace _build
{
    public class Paths {
        public DirectoryPath RootDir { get; }
        public string SrcDir { get; set; }
        public string ArtifactsDir {get; set; }
        public string TestCoverageOutputFile { get; set; }
        public string TestCoverageReportDir { get; set; }
        public string PackagesDir { get; set; }
        public string BuildPropsFile { get; set; }
        public string TestsRootDir { get; set; }
        public string SamplesRootDir { get; set; }
        public string CommonAssemblyVersionFile { get; set; }

        public Paths(BuildBase context)
        {
            RootDir = context.MakeAbsolute(context.Directory("./"));
            SrcDir = "./src";
            ArtifactsDir = "./artifacts";
            TestCoverageOutputFile = ArtifactsDir + "/OpenCover.xml";
            TestCoverageReportDir = ArtifactsDir + "/CodeCoverageReport";
            PackagesDir = ArtifactsDir + "/packages";
            BuildPropsFile = SrcDir + "/Directory.Build.props";
            TestsRootDir = SrcDir + "/tests";
            CommonAssemblyVersionFile = SrcDir + "/common/AssemblyVersion.cs";
        }

    }
}
