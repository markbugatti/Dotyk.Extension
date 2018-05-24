using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MessageBox = System.Windows.Forms.MessageBox;
using Microsoft.VisualStudio.Shell;

namespace Dotyk.Extension.Commands
{
    class CFuncs
    {
        internal static Project GetCurrentProject(Projects projects, string targetProjName)
        {
            foreach (Project project in projects)
            {
                if (project.Name == targetProjName)
                {
                    return project;
                }
            }
            return null;
        }

        internal static void Deploy(DTE2 dte, Project project, Configuration conf)
        {
            ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            OutputsWindow.outputPane.OutputString("Hello world");
            // текущий путь
            string str = conf.Properties.Item("OutputPath").Value.ToString();
            var path = Path.Combine(Path.GetDirectoryName(project.FullName), str, @"Appx\AppxManifest.xml");
            XmlDocument xmlDocument = new XmlDocument();

            string command = "\"" + Path.GetDirectoryName(dte.FullName) + "\\devenv\" " +
                "\"" + dte.Solution.FullName + "\" " +
                "/deploy \"" + conf.ConfigurationName + "|" + conf.PlatformName + "\" /project " + "\"" +
                project.FullName + "\" " + "/projectconfig \"" + conf.ConfigurationName + "|" + conf.PlatformName + "\"";

            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                }
            };
            //process.OutputDataReceived += Process_OutputDataReceived1;
            process.OutputDataReceived += new DataReceivedEventHandler((sender1, e1) =>
            {
                ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                OutputsWindow.outputPane.OutputString(e1.Data + Environment.NewLine);
            });

            process.Start();
            process.BeginOutputReadLine();
            //StreamReader reader = process.StandardOutput;
            using (StreamWriter pWriter = process.StandardInput)
            {
                if (pWriter.BaseStream.CanWrite)
                {
                    pWriter.WriteLine(command);
                }
            }

            process.WaitForExit();

            xmlDocument.Load(path);
            /*}*/

            XmlNodeList xmlNodes = xmlDocument.GetElementsByTagName("Package");
            foreach (XmlNode xmlNode in xmlNodes.Item(0).ChildNodes)
            {
                if (xmlNode.Name.Equals("Applications"))
                {
                    bool manifested = false;
                    if (xmlNode.ChildNodes.Count == 1)
                    {
                        // change manifest
                        var app = xmlNode.FirstChild as XmlElement;
                        var id = app?.GetAttribute("id") as string;
                        for (int i = 0; i < 9; i++)
                        {
                            XmlNode copyApp = app.Clone();
                            ///todo change id
                            string val = app.Attributes["Id"].Value + i.ToString();
                            copyApp.Attributes["Id"].Value = val;
                            xmlNode.AppendChild(copyApp);
                        }
                        manifested = true;
                    }
                    // version up
                    foreach (XmlNode item in xmlNodes.Item(0).ChildNodes)
                    {
                        if (item.Name.Equals("Identity"))
                        {
                            var version = Version.Parse(item.Attributes["Version"].Value.ToString());
                            item.Attributes["Version"].Value = new Version(version.Major, version.Minor, version.Build, version.Revision + 1).ToString();
                        }
                    }

                    xmlDocument.Save(path);
                    // custom deploy
                    using (PowerShell PowerShellInstance = PowerShell.Create())
                    {
                        PowerShellInstance.AddCommand("Add-AppxPackage").AddParameter("register").AddArgument(path);
                        var result = PowerShellInstance.Invoke();
                        //OutputsWindow.outputPane.OutputString(PowerShellInstance.Streams.Verbose.ToString());
                        //Debug.Write(PowerShellInstance.Streams.Verbose.ToString());
                    }
                    if (manifested)
                        MessageBox.Show("Success deployed");
                    else
                        MessageBox.Show("Success version update");
                    break;
                }
            }
        }
    }
}
