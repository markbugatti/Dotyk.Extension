using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace Dotyk.Store.Cli
{
    public class PackageOption : CommonOptions
    {
        public string Project { get; set; }
        public string Solution { get; set; }
        public string Output { get; set; }
        public string Configuration { get; set; } = "Release";

        protected override async Task ExecuteOverride(ILogger logger)
        {
            var packager = Utils.PreparePackager(Configuration, logger);

            using (var packageStream = await packager.CreatePackage(
                   Project,
                   Solution ?? Utils.ResolveSolutionPath(Project),
                   default(CancellationToken)))
            {
                packageStream.Seek(0, SeekOrigin.Begin);

                if (Output != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Output));
                    using (var outFile = File.Create(Output))
                    {
                        await packageStream.CopyToAsync(outFile);
                    }
                }
            }
        }
    }
}
