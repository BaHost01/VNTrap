using System.Diagnostics;

namespace Bloxstrap.Roblox
{
    public static class RobloxPerformanceManager
    {
        public static void Apply(Process process)
        {
            const string LOG_IDENT = "RobloxPerformanceManager::Apply";

            try
            {
                if (process.HasExited)
                    return;

                process.PriorityClass = ProcessPriorityClass.AboveNormal;
                process.PriorityBoostEnabled = true;

                App.Logger.WriteLine(LOG_IDENT, $"Applied performance scheduling to Roblox PID {process.Id}");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, "Unable to apply process performance settings");
                App.Logger.WriteException(LOG_IDENT, ex);
            }
        }

        public static void ApplyToRunningPlayers()
        {
            const string LOG_IDENT = "RobloxPerformanceManager::ApplyToRunningPlayers";

            foreach (var process in Process.GetProcessesByName("RobloxPlayerBeta"))
            {
                try
                {
                    Apply(process);
                }
                finally
                {
                    process.Dispose();
                }
            }

            App.Logger.WriteLine(LOG_IDENT, "Finished applying performance scheduling to running Roblox clients");
        }
    }
}