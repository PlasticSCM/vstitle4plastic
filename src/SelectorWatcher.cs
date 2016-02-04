using System;
using System.IO;

using Microsoft.VisualStudio.Shell.Interop;

using Codice.CmdRunner;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    internal class SelectorWatcher
    {
        internal SelectorWatcher(
            WindowTitleBuilder builder,
            IVsActivityLog log)
        {
            mBuilder = builder;
            mLog = log;
        }

        internal void Initialize(string solutionPath)
        {
            mWkPath = GetWorkspacePath(solutionPath);

            if (string.IsNullOrEmpty(mWkPath))
                return;

            UpdateSelector(mBuilder);
            InitializeWatcher(mWkPath);
        }

        internal void Dispose()
        {
            mBuilder.SetSelector(string.Empty);

            if (mWatcher == null)
                return;

            mWatcher.Dispose();
        }

        void InitializeWatcher(string wkspacePath)
        {
            string plasticWkFolder = Path.Combine(wkspacePath, DEFAULT_WK_CONFIG_DIR);
            string selectorFile = Path.Combine(plasticWkFolder, SELECTOR_FILE);

            mWatcher = new FileSystemWatcher();
            mWatcher.Path = Path.GetDirectoryName(selectorFile);
            mWatcher.Filter = Path.GetFileName(selectorFile);
            mWatcher.IncludeSubdirectories = false;
            mWatcher.EnableRaisingEvents = true;
            mWatcher.NotifyFilter = NotifyFilters.LastWrite;

            mWatcher.Changed += new FileSystemEventHandler(OnSelectorChanged);
        }

        string GetWorkspacePath(string solutionpath)
        {
            string wkInfo;
            string error;

            int cmdres = CmdRunner.ExecuteCommandWithResult(
                string.Format("cm gwp {0}", solutionpath), 
                Directory.GetParent(solutionpath).FullName, out wkInfo, out error, false);

            if (cmdres != 0 || !string.IsNullOrEmpty(error))
                return null;

            string[] fields = wkInfo.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (fields.Length < 2)
                return null;

            return fields[1];
        }

        void OnSelectorChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                UpdateSelector(mBuilder);
            }
            catch(Exception ex)
            {
                mLog.LogEntry(
                    (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
                    "SelectorWatcher",
                    string.Format("An error occured while updating the selector: {0}", ex.Message));
            }
        }

        void UpdateSelector(WindowTitleBuilder builder)
        {
            string selectorInfo;
            string error;

            int cmdres = CmdRunner.ExecuteCommandWithResult(
                "cm wi", mWkPath, out selectorInfo, out error, false);

            if (cmdres != 0 || !string.IsNullOrEmpty(error))
                return;

            string[] lines = selectorInfo.Split(
                new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            builder.SetSelector(lines[0]);
        }

        WindowTitleBuilder mBuilder;
        FileSystemWatcher mWatcher;
        string mWkPath;
        IVsActivityLog mLog;

        const string DEFAULT_WK_CONFIG_DIR = ".plastic";
        const string SELECTOR_FILE = "plastic.selector";
    }
}
