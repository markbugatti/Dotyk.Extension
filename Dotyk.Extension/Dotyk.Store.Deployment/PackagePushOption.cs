using System.Threading;
using System.Threading.Tasks;
//using Dotyk.Store.Model;
using Microsoft.Extensions.Logging;

namespace Dotyk.Store.Cli
{
    public enum PackageFeed
    {
        Default = 0,
        Release = 1,
        Staging = 2,
        Test = 3
    }

    public class PackagePushOption : AuthCommonOptions
    {
        public string Project { get; set; }
        public string Solution { get; set; }
        public string Output { get; set; }
        public string Configuration { get; set; } = "Release";
        public PackageFeed Feed { get; set; }

        protected override async Task ExecuteOverrideAsync(ILogger logger)
        {
            var packager = Utils.PreparePackager(Configuration, logger);

            using (var packageStream = await packager.CreatePackage(
                    Project,
                    Solution ?? Utils.ResolveSolutionPath(Project),
                    default(CancellationToken)))
            {
                await Utils.PushPackageAsync(new PushOption
                {
                    Login = Login,
                    Password = Password,
                    Register = Register,
                    ServerUrl = ServerUrl,
                    DotykMeToken = DotykMeToken,
                    UseDotykMe = UseDotykMe,
                    Feed = Feed
                }, logger, packageStream);
            }
        }
    }
}
