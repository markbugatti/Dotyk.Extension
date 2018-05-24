//using System;
//using System.Threading.Tasks;
//using Dotyk.Store.Model;
//using Microsoft.Extensions.Logging;

//namespace Dotyk.Store.Cli
//{
//    public class ShareOption : AuthCommonOptions
//    {
//        public string AppId { get; set; }
//        public string Role { get; set; }
//        public string UserId { get; set; }

//        protected override async Task ExecuteOverrideAsync(ILogger logger)
//        {
//            using (logger.BeginScope("ExecuteShare"))
//            {
//                try
//                {
//                    var role = PackageRole.GetPackageRole(Role);

//                    if (role == null)
//                        throw new ArgumentException("Invalid role: " + Role);

//                    var client = await Utils.CreateAndAuthenticateStoreClientAsync(this, logger);

//                    await client.AddAccess(AppId, UserId, role);
//                }
//                catch (Exception ex)
//                {
//                    logger.LogError(new EventId(), ex, "Failed to add access to app");
//                    throw;
//                }
//            }
//        }
//    }
//}
