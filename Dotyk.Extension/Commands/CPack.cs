using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using PackageOption = Dotyk.Store.Cli.PackageOption;
using EnvDTE;
using EnvDTE80;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Dotyk.Extension.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CPack
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("050bba8e-f798-42ab-b562-dea1f1bf3e45");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CPack"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private CPack(AsyncPackage package, OleMenuCommandService commandService)
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
        public static CPack Instance
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
            // Verify the current thread is the UI thread - the call to AddCommand in CPack's constructor requires
            // the UI thread.
            ThreadHelper.ThrowIfNotOnUIThread();

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new CPack(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync(default(CancellationToken));
            DTE2 dte = await this.ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE2;

            ToolWindowPane pane = this.package.FindToolWindow(typeof(WDotykCmd), 0, true);
            WDotykCmdControl wDotyk = pane.Content as WDotykCmdControl;

            // Show ToolWindow.OoutputWindow
            

            if (dte.Solution.Count != 0)
                if (dte.Solution.Projects.Count != 0)
                {
                    if (!string.IsNullOrEmpty(wDotyk.PackProj.Text))
                    {
                        OutputsWindow.outputPane.Clear();
                        PackageOption packageOption = new PackageOption()
                        {
                            Configuration = wDotyk.PackConf.Text,
                            LogVerbosity = (LogLevel)Enum.Parse(typeof(LogLevel), wDotyk.PackVerb.SelectedValue.ToString()),
                            Project = wDotyk.PackProj.Text,
                            Output = wDotyk.PackOutp.Text + wDotyk.PackFileName.Text + @".package.zip",
                            Solution = wDotyk.solution
                        };

                        try
                        {
                            var a = OutputsWindow.outputPane.Activate();
                            await packageOption.ExecuteAsync();
                        }
                        catch (Exception ex)
                        {
                            Debug.Write(ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("field \"ProjectPath\" is required");
                    }
                }
                else
                    MessageBox.Show("No Project");
            else
                MessageBox.Show("No Solution");
        }
    }
}
