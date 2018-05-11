using System;
using System.Threading.Tasks;
using CommandLine;
using Dotyk.Store.Model;
using Microsoft.Extensions.Logging;

namespace Dotyk.Store.Cli
{
    public class ShareOption : AuthCommonOptions
    {
        [Option("AppId", HelpText = "Application Id", Required = true)]
        public string AppId { get; set; }

        [Option("Role", HelpText = "Role to grant. (PACKAGE_PUBLISH, PACKAGE_CHANGEACCESS, PACKAGE_CHANGEFEED)", Required = true)]
        public string Role { get; set; }

        [Option("User", HelpText = "Dotyk.Me user id (usually email)", Required = true)]
        public string UserId { get; set; }

        protected override async Task ExecuteOverride(ILogger logger)
        {
            using (logger.BeginScope("ExecuteShare"))
            {
                try
                {
                    var role = PackageRole.GetPackageRole(Role);

                    if (role == null)
                        throw new ArgumentException("Invalid role: " + Role);

                    var client = await Utils.CreateAndAuthenticateStoreClient(this, logger);

                    await client.AddAccess(AppId, UserId, role);
                }
                catch (Exception ex)
                {
                    logger.LogError(new EventId(), ex, "Failed to add access to app");
                    throw;
                }
            }
        }
    }
}
