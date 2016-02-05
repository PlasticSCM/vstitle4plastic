using System;
using System.IO;

using Microsoft.VisualStudio.Shell.Interop;

using Codice.CmdRunner;

namespace CodiceSoftware.VsTitle4Plastic
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

            if (!File.Exists(selectorFile))
            {
                mLog.LogEntry(
                    (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_WARNING,
                    "SelectorWatcher",
                    string.Format("The selector file {0} does not exist", selectorFile));

                return;
            }

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
                string.Format("cm gwp . --format=\"{{1}}\""),
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
                string.Format("cm wi --machinereadable --fieldseparator={0}", FIELD_SEPARATOR), 
                mWkPath, out selectorInfo, out error, false);

            if (cmdres != 0 || !string.IsNullOrEmpty(error))
                return;

            string[] chunks = selectorInfo.Trim().Split(
                new string[] {FIELD_SEPARATOR}, StringSplitOptions.RemoveEmptyEntries);

            if(chunks.Length != 3)
            {
                builder.SetSelector(string.Empty);
                return;
            }

            string selector = string.Format("{0}:{1}@{2}",
                chunks[0].ToLower(), chunks[1], chunks[2]);

            builder.SetSelector(selector);
        }

        WindowTitleBuilder mBuilder;
        FileSystemWatcher mWatcher;
        string mWkPath;

        const string DEFAULT_WK_CONFIG_DIR = ".plastic";
        const string SELECTOR_FILE = "plastic.selector";
        const string FIELD_SEPARATOR = "####";

        static IVsActivityLog mLog = ActivityLog.Get();
    }
}
