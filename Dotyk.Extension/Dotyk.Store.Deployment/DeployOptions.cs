namespace Dotyk.Store.Cli
{
    public class AllVerbs
    {
        public PackagePushOption PackagePushOption { get; set; } = new PackagePushOption();
        public PackageOption PackageOption { get; set; } = new PackageOption();
        public InstallOption InstallOption { get; set; } = new InstallOption();
        public PushOption PushOption { get; set; } = new PushOption();
        //public ShareOption ShareOption { get; set; } = new ShareOption();
       // public ChangeFeedOptions ChangeFeedOption { get; set; } = new ChangeFeedOptions();
       // public GetHistoryOptions GetHistoryOption { get; set; } = new GetHistoryOptions();
    }
}
