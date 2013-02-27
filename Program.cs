using System;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using NDesk.Options;
using SingleAppLauncher.Resources;
using System.Threading;

namespace SingleAppLauncher
{
    internal sealed class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            string partialExePath = ConfigurationManager.AppSettings["ExecutablePath"];
            string argsForLaunchProgram = ConfigurationManager.AppSettings["LaunchArguments"];
            bool showHelp = false;
            int waitTime = int.Parse(ConfigurationManager.AppSettings["WaitTime"]);

            RunAsAdmin runAsAdmin = (RunAsAdmin)Enum.Parse(typeof(RunAsAdmin),
                ConfigurationManager.AppSettings["RunAsAdmin"]);

            OptionSet options = new OptionSet()
            {
                {
                    "p|path=", (string v) =>
                    {
                        if (!string.IsNullOrEmpty(v))
                        {
                            partialExePath = v;
                        }
                    }
                },
                {
                    "a|args=", (string v) =>
                    {
                        argsForLaunchProgram = v;
                    }
                },
                {
                    "w|wait=", (int v) =>
                    {
                        waitTime = v;
                    }
                },
                {
                    "r|runAsAdmin=", (string v) =>
                    {
                        runAsAdmin = (RunAsAdmin)Enum.Parse(typeof(RunAsAdmin),
                            v);
                    }
                },
                {
                    "h|?|help", (string v) =>
                    {
                        showHelp = !string.IsNullOrEmpty(v);
                    }
                }
            };

            options.Parse(args);

            if (showHelp)
            {
                ShowHelpMessage();
                return 0;
            }

            Version latestVersion = null;

            foreach (var dir in Directory.EnumerateDirectories(Environment.CurrentDirectory))
            {
                try
                {
                    Version versionOfDir = new Version(new DirectoryInfo(dir).Name);

                    if (latestVersion == null || versionOfDir.CompareTo(latestVersion) > 0)
                    {
                        latestVersion = versionOfDir;
                    }
                }
                catch (ArgumentException)
                {
                    // Skip directory
                }
            }

            if (latestVersion == null)
            {
                ShowErrorMessageBox(AppResources.VersionDirectoryNotFoundError);
                return 1;
            }
            else if (string.IsNullOrEmpty(partialExePath))
            {
                ShowErrorMessageBox(AppResources.EmptyPathError);
                return 1;
            }

            if (waitTime > 0)
            {
                Thread.Sleep(waitTime);
            }

            bool adminMode = false;

            if (RunningXP())
            {
                // XP does not have UAC, so LaunchApp() 'launch as admin'
                // functionality is not needed.
                adminMode = false;
            }
            else if (runAsAdmin == RunAsAdmin.Yes)
            {
                adminMode = true;
            }
            else if (runAsAdmin == RunAsAdmin.Ask)
            {
                DialogResult result = MessageBox.Show(
                    AppResources.AdminPrompt,
                    AppResources.MessageBoxCaption,
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    adminMode = true;
                }
                else if (result == DialogResult.Cancel)
                {
                    return 1; // do not launch program
                }
            }

            return LaunchApp(latestVersion, partialExePath, argsForLaunchProgram, adminMode);
        }

        static int LaunchApp(Version latestVersion, string partialExePath, string args, bool runAsAdmin)
        {
            int statusCode = 0;
            string completeExePath = Path.Combine(Environment.CurrentDirectory,
                                                  latestVersion.ToString(),
                                                  partialExePath);

            var info = new ProcessStartInfo(completeExePath)
            {
                WorkingDirectory = Path.Combine(Environment.CurrentDirectory,
                                                latestVersion.ToString()),
                Arguments = args
            };

            if (runAsAdmin)
            {
                info.Verb = "runas";
                info.UseShellExecute = true;
            }

            try
            {
                Process.Start(info);
            }
            catch (Exception e)
            {
                ShowErrorMessageBox(e.GetType().ToString());
                statusCode = 1;
            }

            return statusCode;
        }

        static void ShowHelpMessage()
        {
            ShowInfoMessageBox(AppResources.HelpMessage);
        }

        static void ShowInfoMessageBox(string message)
        {
            MessageBox.Show(message, AppResources.MessageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(message, AppResources.MessageBoxCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static bool RunningXP()
        {
            Version osVersion = System.Environment.OSVersion.Version;
            return osVersion.Major == 5;
        }
    }
}
