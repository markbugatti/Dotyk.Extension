using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using EnvDTE;

namespace Dotyk.Extension
{
    internal class PackBox
    {
        public ComboBox Conf { get; set; }
        public TextBox ProjName { get; set; }
        public String SolName { get; set; }
        public TextBox OutpFolderName { get; set; }
    }

    internal class Filler
    {
        public static void FillConfigurations(ComboBox comboBox, ConfigurationManager configuration)
        {
            comboBox.Items.Clear();
            foreach (Configuration item in configuration)
            {
                bool exist = false;
                foreach (string text in comboBox.Items)
                {
                    if (item.ConfigurationName == text)
                    {
                        exist = true;
                        break;
                    }
                }
                if (!exist)
                {
                    comboBox.Items.Add(item.ConfigurationName);
                }
            }
           comboBox.SelectedValue = configuration.ActiveConfiguration.ConfigurationName;
        }

        public static Project GetCurrentProject(Projects projects, string targetProjName)
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

        public static void fillPack(PackBox packBox, Solution solution, ref string solutionName)
        {
            var targetProjName = solution.Properties.Item("StartupProject").Value.ToString();
            EnvDTE.Project project = GetCurrentProject(solution.Projects, targetProjName);

            FillConfigurations(packBox.Conf, project.ConfigurationManager);

            packBox.SolName= solution.FullName;
            solutionName = solution.FullName;
            packBox.ProjName.Text = project.FullName;
            packBox.OutpFolderName.Text = Path.GetDirectoryName(solution.FullName) + @"\";
        }
    }

}
