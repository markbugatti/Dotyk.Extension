using System.Threading.Tasks;
using CommandLine;
using Dotyk.Store.Model;
using Microsoft.Extensions.Logging;

namespace Dotyk.Store.Cli
{
    public class ChangeFeedOptions : AuthCommonOptions
    {
        [Option("Feed", HelpText = "(Default, Release, Staging, Test). New feed.", Required = true)]
        public PackageFeed Feed { get; set; }

        [Option("AppId", HelpText = "Application Id to manage", Required = true)]
        public string AppId { get; set; }

        [Option("Version", HelpText = "Package version to modify", Required = true)]
        public string Version { get; set; }

        protected override async Task ExecuteOverride(ILogger logger)
        {
            using (logger.BeginScope("ChangeFeed"))
            {
                var client = await Utils.CreateAndAuthenticateStoreClient(this, logger);

                logger.LogTrace("Moving package {AppId} ({Version}) to feed '{feed}'", AppId, Version, Feed);

                await client.ChangeFeed(AppId, Version, Feed);

                logger.LogInformation("Moved package {AppId} ({Version}) to feed '{feed}'", AppId, Version, Feed);
            }
        }
    }
}
