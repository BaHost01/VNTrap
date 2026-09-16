namespace Bloxstrap
{
    public static class DiagnosticsManager
    {
        private static int _reportCount;

        public static string? LastReportPath { get; private set; }

        public static string WriteCrashReport(Exception exception, string source)
        {
            const string LOG_IDENT = "DiagnosticsManager::WriteCrashReport";

            try
            {
                string directory = Path.Combine(Paths.Base, "Logs", "CrashReports");
                Directory.CreateDirectory(directory);

                string timestamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
                int sequence = Interlocked.Increment(ref _reportCount);
                string path = Path.Combine(directory, $"{App.ProjectName}_{timestamp}_{sequence}.txt");

                string report = $"""{App.ProjectName} crash report
Generated: {DateTime.UtcNow:O}
Source: {source}

Application
- Version: {App.Version}
- Process: {Paths.Process}
- Application: {Paths.Application}
- Install location: {Paths.Base}
- Temp: {Paths.Temp}

Build
- Action build: {App.IsActionBuild}
- Production build: {App.IsProductionBuild}
- Commit: {App.BuildMetadata.CommitHash}
- Ref: {App.BuildMetadata.CommitRef}

Environment
- OS: {Environment.OSVersion}
- 64-bit OS: {Environment.Is64BitOperatingSystem}
- 64-bit process: {Environment.Is64BitProcess}
- Runtime: {Environment.Version}
- Machine: {Environment.MachineName}

Exception
{exception}
""";

                File.WriteAllText(path, report, Encoding.UTF8);
                LastReportPath = path;

                App.Logger.WriteLine(LOG_IDENT, $"Crash report written to '{path}'");
                return path;
            }
            catch (Exception reportException)
            {
                App.Logger.WriteException(LOG_IDENT, reportException);
                return string.Empty;
            }
        }
    }
}
