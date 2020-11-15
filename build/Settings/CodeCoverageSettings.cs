namespace _build.Settings
{
    public class CodeCoverageSettings
    {
        public string ExcludeByFile { get; set; } = "*/*Designer.cs";
        public string ExcludeByAttribute { get; set; } = "*.ExcludeFromCodeCoverage*";
        public string ExcludeFilter { get; set; } = "-[Tests*]*";
        public string IncludeFilter { get; set; }
    }
}