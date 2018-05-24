namespace Dotyk.Extension
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using WinForms = System.Windows.Forms;
    using System;
    using Task = System.Threading.Tasks.Task;
    using Microsoft.Extensions.Logging;
    using global::Dotyk.Store.Cli;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// Interaction logic for WPushControl.
    /// </summary>
    public partial class WPushControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WPushControl"/> class.
        /// </summary>
        internal ComboBox comboBoxConf;
        internal TextBox textBoxPackage;
        private Button pushButton;
        private TextBox textBoxAuthToken;
        private TextBox textBoxServer;
        private ComboBox comboBoxVerb;
        private CheckBox checkBoxRegister;
        private WinForms.OpenFileDialog fileBrowser;
        private string packagePath;

        public WPushControl()
        {
            this.InitializeComponent();
            comboBoxConf = FindName("Configuration") as ComboBox;
            comboBoxVerb = FindName("Verbosity") as ComboBox;
            textBoxPackage = FindName("Package") as TextBox;
            pushButton = FindName("Push") as Button;
            textBoxAuthToken = FindName("AuthToken") as TextBox;
            textBoxServer = FindName("Server") as TextBox;
            checkBoxRegister = FindName("Register") as CheckBox;
            fileBrowser = new WinForms.OpenFileDialog();
            fileBrowser.InitialDirectory = @"C:\Users\Mark\AppData\Local\Temp\Kodisoft\Dotyk\Build";
        }

        private async void PushButton_Click(object sender, RoutedEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            PushOption pushOption = new PushOption()
            {
                Register = checkBoxRegister.IsChecked ?? false,
                ServerUrl = textBoxServer.Text,
                LogVerbosity = (LogLevel)Enum.Parse(typeof(LogLevel), comboBoxVerb.SelectedValue.ToString()),
                PackagePath = packagePath,//packagePath,
                Feed = /*Store.Model.*/PackageFeed.Test,
                DotykMeToken = textBoxAuthToken.Text,
                UseDotykMe = true
            };
            try
            {
                OutputsWindow.outputPane.Activate();
                await PushAsync(pushOption);
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
            }
        }

        private static async Task PushAsync(PushOption pushOption)
        {
            try
            {
                await pushOption.ExecuteAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Package_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (fileBrowser.ShowDialog() == WinForms.DialogResult.OK)
            {
                packagePath = fileBrowser.FileName;
                textBoxPackage.Text = packagePath;
            }
        }
    }
}