using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Logging;
using Dotyk.Extension.Logging.Outputs;
using Dotyk.Extension.Logging.Outputs.Internal;

namespace Dotyk.Store.Cli
{
    public abstract class CommonOptions
    {

        public LogLevel LogVerbosity { get; set; }

        public Task Execute()
        {
            var logger = new OutputsLogger("Publisher", (m, l) => l >= LogVerbosity, true);

            //We use this hack to disable async log processing for consolelogger
            //https://github.com/aspnet/Logging/blob/dev/src/Microsoft.Extensions.Logging.Console/Internal/ConsoleLoggerProcessor.cs
            var processor =
                Array.Find(
                    logger.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic),
                    i => i.FieldType == typeof(OutputsLoggerProcessor))
                ?.GetValue(logger)
                as OutputsLoggerProcessor;

            processor?.Dispose();

            return ExecuteOverride(logger);
        }

        protected abstract Task ExecuteOverride(ILogger logger);
    }
}
