using System.Diagnostics;

namespace Bloxstrap.Roblox
{
    public static class RobloxMultiInstanceManager
    {
        public const int DefaultInstanceCount = 2;
        public const int MaxInstanceCount = 8;

        public static int ParseInstanceCount(string? value)
        {
            if (!int.TryParse(value, out int count))
                return DefaultInstanceCount;

            return Math.Clamp(count, 2, MaxInstanceCount);
        }

        public static int LaunchAdditionalInstances(string executablePath, string arguments, string workingDirectory, int totalInstances)
        {
            const string LOG_IDENT = "RobloxMultiInstanceManager::LaunchAdditionalInstances";
            int count = Math.Clamp(totalInstances, 2, MaxInstanceCount);
            int launched = 0;

            for (int i = 1; i < count; i++)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = executablePath,
                        Arguments = arguments,
                        WorkingDirectory = workingDirectory,
                        UseShellExecute = false
                    });

                    launched++;
                    App.Logger.WriteLine(LOG_IDENT, $"Started additional Roblox instance {i + 1}/{count}");
                }
                catch (Exception ex)
                {
                    App.Logger.WriteLine(LOG_IDENT, $"Failed to start additional Roblox instance {i + 1}/{count}");
                    App.Logger.WriteException(LOG_IDENT, ex);
                }
            }

            return launched;
        }
    }
}