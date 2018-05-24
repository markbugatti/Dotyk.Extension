using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MessageBox = System.Windows.Forms.MessageBox;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Dotyk.Extension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class WDotykCmdCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 4131;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("01732301-0a80-4a99-b1f9-0700410a9879");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="WDotykCmdCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private WDotykCmdCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static WDotykCmdCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Verify the current thread is the UI thread - the call to AddCommand in WDotykCmdCommand's constructor requires
            // the UI thread.
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new WDotykCmdCommand(package, commandService);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            DTE2 dte = await this.ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE2;

            OutputsWindow.outputPane.Activate();

            try
            {
                ToolWindowPane window = this.package.FindToolWindow(typeof(WDotykCmd), 0, true);
                if ((null == window) || (null == window.Frame))
                {
                    throw new NotSupportedException("Cannot create tool window");
                }

                if (dte.Solution.Count != 0)
                    if (dte.Solution.Projects.Count != 0)
                    // заполнить окно команды pack, вынести в отдельную функцию класса Filler
                    {

                        WDotykCmdControl wDotykCmdControl = window.Content as WDotykCmdControl;
                        Filler.fillPack(new PackBox() {
                            //SolName = wDotykCmdControl.solution,
                            Conf = wDotykCmdControl.PackConf,
                            ProjName = wDotykCmdControl.PackProj,
                            OutpFolderName = wDotykCmdControl.PackOutp
                        }, dte.Solution, ref wDotykCmdControl.solution);

                    }

                IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
                windowFrame.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, VSSETFRAMEPOS.SFP_fTab);
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());

            }
            catch (Exception)
            {
                MessageBox.Show("Не выбран проект");
            }
        }
    }
}
