using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Dotyk.Store.Model;
using Microsoft.Extensions.Logging;

namespace Dotyk.Store.Cli
{
    public class PushOption : AuthCommonOptions
    {
        public string PackagePath { get; set; }
        public PackageFeed Feed { get; set; }

        protected override async Task ExecuteOverride(ILogger logger)
        {
            using (var packageStream = File.OpenRead(PackagePath))
            {
                await Utils.PushPackage(this, logger, packageStream);
            }
        }
    }
}
