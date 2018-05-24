using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using Microsoft.VisualStudio.Threading;

namespace Dotyk.Extension
{
    /// <summary>
    /// One Intance Output Window
    /// </summary>
    internal sealed class OutputsWindow
    {
        private readonly AsyncPackage package;
        
        // instance of class
        public static OutputsWindow Instance
        {
            get;
            private set;
        }

        public static IVsOutputWindowPane outputPane;

        public OutputsWindow(AsyncPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

         //   outputPane = CreatePane(new Guid(), "Dotyk Logger", true, true) as IVsOutputWindowPane;
        }


        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        public async Task CreatePaneAsync(Guid paneGuid, string title, bool visible, bool clearWithSolution)
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
            IVsOutputWindow output =
                (IVsOutputWindow)ServiceProvider.GetService(typeof(SVsOutputWindow));
            
            // Create a new pane.  
            output.CreatePane(
                ref paneGuid,
                title,
                Convert.ToInt32(visible),
                Convert.ToInt32(clearWithSolution));

            // Retrieve the new pane.  
            output.GetPane(ref paneGuid, out outputPane);
            //pane.OutputString("This is the Created Pane \n");
            outputPane.Activate();
            outputPane.OutputString("hello");
        }

        //void DeletePane(Guid paneGuid)
        //{

        //    IVsOutputWindow output =
        //    (IVsOutputWindow)ServiceProvider.GetService(typeof(SVsOutputWindow));

        //    IVsOutputWindowPane pane;
        //    output.GetPane(ref paneGuid, out pane);

        //    if (pane == null)
        //    {
        //        pane = CreatePane(paneGuid, "New Pane\n", true, true);
               
        //    }
        //    else
        //    {
        //        output.DeletePane(ref paneGuid);
        //    }
        //}

        //private void OutputCommandHandler(object sender, EventArgs e)
        //{
        //    CreatePaneAsync(new Guid(), "Created Pane", true, false);
        //}


        public static async Task InitializeAsync(AsyncPackage package)
        {
            
            ThreadHelper.ThrowIfNotOnUIThread();
            Instance = new OutputsWindow(package);
            await Instance.CreatePaneAsync(new Guid(), "Dotyk Logger", true, true);
        }

    }
}
