using CommandLine;

namespace Dotyk.Store.Cli
{
    public abstract class ServerCommonOptions : CommonOptions
    {
        public string ServerUrl { get; set; }
    }
}
