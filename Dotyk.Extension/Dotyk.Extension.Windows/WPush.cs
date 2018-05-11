namespace Dotyk.Extension
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("5ac3d10c-9fb7-4167-b95d-e88a30836527")]
    public class WPush : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WPush"/> class.
        /// </summary>
        public WPush() : base(null)
        {
            this.Caption = "WPush";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new WPushControl();
        }
    }
}
