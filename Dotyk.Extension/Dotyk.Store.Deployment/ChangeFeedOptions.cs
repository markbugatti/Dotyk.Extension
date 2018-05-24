//using System.Threading.Tasks;
//using Dotyk.Store.Model;
//using Microsoft.Extensions.Logging;

//namespace Dotyk.Store.Cli
//{
//    public class ChangeFeedOptions : AuthCommonOptions
//    {
//        public PackageFeed Feed { get; set; }
//        public string AppId { get; set; }
//        public string Version { get; set; }

//        protected override async Task ExecuteOverrideAsync(ILogger logger)
//        {
//            using (logger.BeginScope("ChangeFeed"))
//            {
//                var client = await Utils.CreateAndAuthenticateStoreClientAsync(this, logger);

//                logger.LogTrace("Moving package {AppId} ({Version}) to feed '{feed}'", AppId, Version, Feed);

//                await client.ChangeFeed(AppId, Version, Feed);

//                logger.LogInformation("Moved package {AppId} ({Version}) to feed '{feed}'", AppId, Version, Feed);
//            }
//        }
//    }
//}
