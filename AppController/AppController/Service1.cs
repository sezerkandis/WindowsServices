using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace AppController
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        Timer timer = new Timer();
        protected override void OnStart(string[] args)
        {
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000;
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            List<KeyValuePair<string, string>> apps = new List<KeyValuePair<string, string>>(); // key: working directory, value application name without .exe extension.
            apps.Add(new KeyValuePair<string, string>(@"your application parent folder path", "application name"));

            try
            {
                for (int appIndex = 0; appIndex < apps.Count; appIndex++)
                {
                    List<int> processIDs = GetProcessID(apps[appIndex].Value);

                    //Settext("Process is running : " + processIDs.Count.ToString());

                    if (processIDs.Count == 0) // if app is not runnning now
                    {
                        string workingDirectory = apps[appIndex].Key;
                        string clientFilePath = Path.Combine(apps[appIndex].Key, apps[appIndex].Value + ".exe");

                        if (File.Exists(clientFilePath))
                        {
                            Process p = new Process();
                            p.StartInfo.WorkingDirectory = workingDirectory;
                            p.StartInfo.FileName = clientFilePath;
                            p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

                            if (File.Exists(clientFilePath))
                                p.Start();

                            //Settext(apps[appIndex].Key + " was started.");
                        }
                        else
                        {
                            Settext("no such file or directory: " + clientFilePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Settext("process start process error: " + ex.Message);
            }
        }

        public List<int> GetProcessID(string appName)
        {
            List<int> processes = new List<int>();
            try
            {
                Process[] processlist = Process.GetProcesses();
                foreach (Process _pr in processlist)
                {
                    if (_pr.ProcessName.StartsWith(appName))
                        processes.Add(_pr.Id);
                }
            }
            catch (Exception ex)
            {
                Settext("Functions - GetProcessID Error: " + ex.Message);
            }

            return processes;
        }

        private void Settext(string message)
        {
            string path = @"C:\Logs";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filepath = Path.Combine(path, "AppController_ServiceLog.txt");
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                    sw.WriteLine(message);
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                    sw.WriteLine(message);
            }
        }
    }
}
