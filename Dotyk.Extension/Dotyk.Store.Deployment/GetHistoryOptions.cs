using System;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Dotyk.Store.Model;
using Microsoft.Extensions.Logging;

namespace Dotyk.Store.Cli
{

    public class GetHistoryOptions : ServerCommonOptions
    {
        public string AppId { get; set; }
        public int? Limit { get; set; }
        public PackageFeed? Feed { get; set; }

        protected override async Task ExecuteOverride(ILogger logger)
        {
            var client = Utils.CreateStoreClient(this, logger);

            logger.LogTrace("Getting history for {appid}, feed {feed} with limit {limit}", AppId, Feed, Limit);

            var history = await client.GetPackageHistory(AppId, Feed, Limit);

            logger.LogTrace("Outputing history");

            Console.WriteLine("Package version history:");
            Console.WriteLine($"Package {history.AppId}");
            Console.WriteLine();
            Console.WriteLine("------");
            Console.WriteLine("Latest packages per feed:");

            foreach (var item in history.LasestVersions.OrderBy(i => i.Feed))
                OutputItem(item);

            Console.WriteLine();
            Console.WriteLine("------");
            Console.WriteLine("Package history:");

            foreach (var item in history.History.OrderByDescending(i => { Version.TryParse(i.Version, out var res); return res; }))
                OutputItem(item);

            Console.WriteLine("------");
        }

        private static void OutputItem(PackageHistoryItem item)
        {
            Console.WriteLine($"{item.Feed}\t{item.Version}\t{item.Published?.LocalDateTime}");
        }
    }
}
