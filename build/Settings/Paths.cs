namespace _build
{
    using Nuke.Common;
    using Nuke.Common.IO;


    public class Paths {
        public AbsolutePath RootDir { get; }
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
            RootDir = NukeBuild.RootDirectory;
            SrcDir = "./src";
            ArtifactsDir = "./artifacts";
            TestCoverageOutputFile = ArtifactsDir + "/OpenCover.xml";
            TestCoverageReportDir = ArtifactsDir + "/CodeCoverageReport";
            PackagesDir = ArtifactsDir + "/packages";
            BuildPropsFile = SrcDir + "/Directory.Build.props";
            TestsRootDir = SrcDir + "/tests";
            SamplesRootDir = SrcDir + "/Samples";
            CommonAssemblyVersionFile = SrcDir + "/common/AssemblyVersion.cs";
        }

    }
}
