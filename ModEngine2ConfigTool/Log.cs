using Sherlog;
using Sherlog.Appenders;
using Sherlog.Formatters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ModEngine2ConfigTool
{
    public class Log
    {
        public static Logger Instance { get; }

        static Log()
        {
            ConfigureLogging();
            Instance = Logger.GetLogger(typeof(Log));
        }

        private static void ConfigureLogging()
        {
            var messageFormatter = new LogMessageFormatter();
            var timestampFormatter = new TimestampFormatter(
                () => DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));

            var consoleAppender = new ConsoleAppender(new Dictionary<LogLevel, ConsoleColor>
            {
                {LogLevel.Trace, ConsoleColor.Cyan},
                {LogLevel.Debug, ConsoleColor.Blue},
                {LogLevel.Info, ConsoleColor.White},
                {LogLevel.Warn, ConsoleColor.Yellow},
                {LogLevel.Error, ConsoleColor.Red},
                {LogLevel.Fatal, ConsoleColor.Magenta},
            });

            var logsDir = Path.Combine(
                Path.GetTempPath(), 
                "Metis Mod Launcher Logs");

            if(!Directory.Exists(logsDir))
            {
                Directory.CreateDirectory(logsDir);
            }

            var logFileName = Path.Combine(
                logsDir,
                string.Format(
                    CultureInfo.InvariantCulture, 
                    "{0:yyyy-M-dd}.log", 
                    DateTime.Now));

            var fileAppender = new FileWriterAppender(logFileName);

            Logger.AddAppender((logger, level, message) =>
            {
                message = messageFormatter.FormatMessage(logger, level, message);
                message = timestampFormatter.FormatMessage(logger, level, message);
                consoleAppender.WriteLine(logger, level, message);
                fileAppender.WriteLine(logger, level, message);
            });
        }
    }
}
