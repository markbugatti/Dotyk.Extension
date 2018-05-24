using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dotyk.Store.Cli
{
    public class PackageOption : CommonOptions
    {
        public string Project { get; set; }
        public string Solution { get; set; }
        public string Output { get; set; }
        public string Configuration { get; set; } = "Release";

        protected override async Task ExecuteOverrideAsync(ILogger logger)
        {
            var packager = Utils.PreparePackager(Configuration, logger);

            try
            {
                using (var packageStream = await packager.CreatePackage(
                   Project,
                   Solution ?? Utils.ResolveSolutionPath(Project),
                   default(CancellationToken)))
                {
                    packageStream.Seek(0, SeekOrigin.Begin);

                    if (Output != null)
                    {
                        try
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(Output)))
                                Directory.CreateDirectory(Path.GetDirectoryName(Output));
                            using (var outFile = File.Create(Output))
                            {
                                await packageStream.CopyToAsync(outFile);
                            }
                            logger.LogInformation("Package succesfully created and copied");
                        }
                        catch (System.Exception ex)
                        {
                            throw new System.Exception(ex.Message);
                        }
                    }
                    else
                        logger.LogInformation("Package succesfully created");
                }
            }
            catch (System.Exception ex)
            {
                logger.LogError("cannot create package: " + ex.Message);
            }
        }
    }
}
