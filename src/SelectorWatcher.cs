using System;
using System.IO;

using Microsoft.VisualStudio.Shell.Interop;

using Codice.CmdRunner;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    internal class SelectorWatcher
    {
        internal SelectorWatcher(WindowTitleBuilder builder)
        {
            mBuilder = builder;
        }

        internal void StartWatcher(string solutionPath)
        {
            mWkPath = GetWorkspacePath(solutionPath);

            if (string.IsNullOrEmpty(mWkPath))
                return;

            UpdateSelector(mBuilder);
            InitializeWatcher(mWkPath);
        }

        internal void StopWatcher()
        {
            mBuilder.SetSelector(string.Empty);

            if (mWatcher == null)
                return;

            mWatcher.Changed -= OnSelectorChanged;
            mWatcher.Dispose();
            mWatcher = null;
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
            string wkPath;
            string error;

            int cmdres = CmdRunner.ExecuteCommandWithResult(
                string.Format("cm gwp . --format=\"{{1}}\"", solutionpath),
                Directory.GetParent(solutionpath).FullName, out wkPath, out error, false);

            if (cmdres != 0 || !string.IsNullOrEmpty(error))
                return null;

            return wkPath.Trim();
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
                "cm wi --machinereadable --fieldseparator", mWkPath, out selectorInfo, out error, false);

            if (cmdres != 0 || !string.IsNullOrEmpty(error))
                return;

            builder.SetSelector(selectorInfo.Trim());
        }

        WindowTitleBuilder mBuilder;
        FileSystemWatcher mWatcher;
        string mWkPath;

        const string DEFAULT_WK_CONFIG_DIR = ".plastic";
        const string SELECTOR_FILE = "plastic.selector";

        static IVsActivityLog mLog = ActivityLog.Get();
    }
}
