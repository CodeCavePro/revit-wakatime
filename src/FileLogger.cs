using System;
using System.IO;
using System.Windows.Forms;
using WakaTime;

namespace CodeCave.WakaTime.Revit
{
    internal class FileLogger : ILogService
    {
        protected string logFolder;

        public FileLogger(string logFolder)
        {
            if (string.IsNullOrWhiteSpace(logFolder))
                throw new ArgumentException("Log folder has to be a valid folder path", nameof(logFolder));

            this.logFolder = logFolder;
        }

        internal void Debug(string msg)
        {
            if (!WakaTimeConfigFile.Debug)
                return;

            Log(LogLevel.Debug, msg);
        }

        internal void Info(string msg)
        {
            Log(LogLevel.Info, msg);
        }

        internal void Warning(string msg)
        {
            Log(LogLevel.Warning, msg);
        }

        internal void Error(string msg, Exception ex = null)
        {
            var exceptionMessage = $"{msg}: {ex}";

            Log(LogLevel.HandledException, exceptionMessage);
        }

        internal void Log(LogLevel level, string msg)
        {
            Log($"[Wakatime {Enum.GetName(level.GetType(), level)} {DateTime.Now.ToString("hh:mm:ss tt")}] {msg}");
        }

        public void Log(string msg)
        {
            try
            {
                using (var writer = CreateStreamWriter())
                {
                    if (msg.EndsWith(Environment.NewLine, StringComparison.OrdinalIgnoreCase))
                        writer.Write(msg);
                    else
                        writer.WriteLine(msg);
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error writing to WakaTime.log", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private StreamWriter CreateStreamWriter()
        {
            var filename = Path.Combine(LogFolder, "wakatime.log");
            var writer = new StreamWriter(File.Open(filename, FileMode.Append, FileAccess.Write));
            return writer;
        }

        private string LogFolder
        {
            get
            {
                // Create folder if it does not exist
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                return logFolder;
            }
        }
    }
}
