namespace Bloxstrap
{
    public class Logger
    {
        private const int MaxHistoryEntries = 2000;
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly object _historyLock = new();
        private FileStream? _filestream;

        public readonly List<string> History = new();
        public bool Initialized = false;
        public bool NoWriteMode = false;
        public string? FileLocation;

        public string AsDocument
        {
            get
            {
                lock (_historyLock)
                    return String.Join('\n', History);
            }
        }

        public void Initialize(bool useTempDir = false)
        {
            const string LOG_IDENT = "Logger::Initialize";

            string directory = useTempDir ? Paths.TempLogs : Path.Combine(Paths.Base, "Logs");
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
            string filename = $"{App.ProjectName}_{timestamp}.log";
            string location = Path.Combine(directory, filename);

            WriteLine(LOG_IDENT, $"Initializing at {location}");

            if (Initialized)
            {
                WriteLine(LOG_IDENT, "Failed to initialize because logger is already initialized");
                return;
            }

            try
            {
                Directory.CreateDirectory(directory);

                if (File.Exists(location))
                {
                    WriteLine(LOG_IDENT, "Failed to initialize because log file already exists");
                    return;
                }

                _filestream = File.Open(location, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            }
            catch (IOException ex)
            {
                WriteException(LOG_IDENT, ex);
                return;
            }
            catch (UnauthorizedAccessException)
            {
                if (NoWriteMode)
                    return;

                WriteLine(LOG_IDENT, $"Failed to initialize because the application cannot write to {directory}");

                Frontend.ShowMessageBox(
                    String.Format(Strings.Logger_NoWriteMode, directory),
                    System.Windows.MessageBoxImage.Warning,
                    System.Windows.MessageBoxButton.OK
                );

                NoWriteMode = true;
                return;
            }

            Initialized = true;
            FileLocation = location;

            string[] pending;
            lock (_historyLock)
                pending = History.ToArray();

            if (pending.Length > 0)
                _ = WriteToLog(String.Join("\r\n", pending));

            WriteLine(LOG_IDENT, "Finished initializing!");

            if (Paths.Initialized && Directory.Exists(Paths.Logs))
            {
                foreach (FileInfo log in new DirectoryInfo(Paths.Logs).GetFiles())
                {
                    if (log.LastWriteTimeUtc.AddDays(7) > DateTime.UtcNow)
                        continue;

                    WriteLine(LOG_IDENT, $"Cleaning up old log file '{log.Name}'");

                    try
                    {
                        log.Delete();
                    }
                    catch (Exception ex)
                    {
                        WriteException(LOG_IDENT, ex);
                    }
                }
            }
        }

        private void WriteLine(string message)
        {
            string timestamp = DateTime.UtcNow.ToString("s") + "Z";
            string output = $"{timestamp} {message}";
            string sanitized = output.Replace(Paths.UserProfile, "%UserProfile%", StringComparison.InvariantCultureIgnoreCase);

            Debug.WriteLine(output);
            _ = WriteToLog(sanitized);

            lock (_historyLock)
            {
                if (History.Count >= MaxHistoryEntries)
                    History.RemoveRange(0, History.Count - MaxHistoryEntries + 1);

                History.Add(sanitized);
            }
        }

        public void WriteLine(string identifier, string message) => WriteLine($"[{identifier}] {message}");

        public void WriteException(string identifier, Exception ex)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            string hresult = "0x" + ex.HResult.ToString("X8");
            WriteLine($"[{identifier}] ({hresult}) {ex}");

            Thread.CurrentThread.CurrentUICulture = Locale.CurrentCulture;
        }

        private async Task WriteToLog(string message)
        {
            if (!Initialized || _filestream is null)
                return;

            try
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                await _filestream.WriteAsync(Encoding.UTF8.GetBytes($"{message}\r\n")).ConfigureAwait(false);
                await _filestream.FlushAsync().ConfigureAwait(false);
            }
            catch (ObjectDisposedException)
            {
                // The logger is shutting down; there is nothing useful left to write.
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Logger write failed: {ex}");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
