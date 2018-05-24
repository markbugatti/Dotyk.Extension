namespace Dotyk.Extension
{
    using Dotyk.Store.Cli;
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using WinForms = System.Windows.Forms;
    using LogLevel = Microsoft.Extensions.Logging.LogLevel;
    using System.Threading.Tasks;
    using System.IO;

    /// <summary>
    /// Interaction logic for WDotykCmdControl.
    /// </summary>
    public partial class WDotykCmdControl : UserControl
    {
        internal String solution;
        private WinForms.OpenFileDialog fileDialog;
        private WinForms.FolderBrowserDialog folderBrowser;
        private string packProjFullName;
        internal string packOutpFolderName = null;
        private string pushPackgFullName;
        /// <summary>
        /// Initializes a new instance of the <see cref="WDotykCmdControl"/> class.
        /// </summary>
        public WDotykCmdControl()
        {
            this.InitializeComponent();
            fileDialog = new WinForms.OpenFileDialog() {
                InitialDirectory = @"C:\Users\Mark\AppData\Local\Temp\Kodisoft\Dotyk\Build"
            };
            folderBrowser = new WinForms.FolderBrowserDialog();
        }
        


        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions", Justification = "Sample code")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Default event handler naming pattern")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "WDotykCmd");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (fileDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                if ((sender as Button).Name == "PackProjPathBtn")
                {
                    PackProj.Text = fileDialog.FileName;
                    packProjFullName = PackProj.Text;
                }
                if ((sender as Button).Name == "PackOutpPathBtn")
                {
                    PackOutp.Text = fileDialog.FileName;
                    packOutpFolderName = PackProj.Text;
                }
                if ((sender as Button).Name == "PushPackgPathBtn")
                {
                    PushPackage.Text = fileDialog.FileName;
                    pushPackgFullName = PackProj.Text;
                }
            }
        }

        private void PackOutpPathBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PackOutp.Text))
            {
                folderBrowser.SelectedPath = PackOutp.Text;
            }
            if (folderBrowser.ShowDialog() == WinForms.DialogResult.OK)
            {
                if ((sender as Button).Name == "PackOutpPathBtn")
                {
                    PackOutp.Text = folderBrowser.SelectedPath + @"\";
                    packOutpFolderName = PackProj.Text;
                }
            }
        }

        private void PackBtn_Click(object sender, RoutedEventArgs e)
        {
            //var menuItem = new MenuCommand(this.Execute, menuCommandID);
        }
    }
}