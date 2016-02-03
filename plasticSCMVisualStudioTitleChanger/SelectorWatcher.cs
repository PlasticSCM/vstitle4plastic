﻿using System;
using System.IO;

using Codice.CmdRunner;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    internal class SelectorWatcher
    {
        internal static void ResetWatcher(
            string solutionPath, 
            WindowTitleBuilder builder)
        {
            if(mWatcher != null)
                mWatcher.Dispose();

            string wkPath = GetWorkspacePath(solutionPath);

            if (string.IsNullOrEmpty(wkPath))
                return;

            mBuilder = builder;
            mWkPath = wkPath;
            UpdateSelector(builder);
            InitializeWatcher(wkPath);
        }

        internal static void Dispose()
        {
            mBuilder.SetSelector(string.Empty);

            if (mWatcher == null)
                return;

            mWatcher.Dispose();
        }

        static void InitializeWatcher(string wkspacePath)
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

        static string GetWorkspacePath(string solutionpath)
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

        static void OnSelectorChanged(object sender, FileSystemEventArgs e)
        {
            UpdateSelector(mBuilder);
        }

        internal static void UpdateSelector(WindowTitleBuilder builder)
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

        static WindowTitleBuilder mBuilder;
        static FileSystemWatcher mWatcher;
        static string mWkPath;

        const string DEFAULT_WK_CONFIG_DIR = ".plastic";
        const string SELECTOR_FILE = "plastic.selector";
    }
}
