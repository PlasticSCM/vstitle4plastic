using System;
using System.Windows;

using Microsoft.VisualStudio.Shell.Interop;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    internal static class WindowTitleSetter
    {
        internal static void SetWindowTitle(string newTitle)
        {
            try
            {
                if (Application.Current.MainWindow.Title == newTitle)
                    return;

                Application.Current.MainWindow.Title = newTitle;
            }
            catch (Exception ex)
            {
                mLog.LogEntry(
                    (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
                    "WindowTitleSetter",
                    string.Format("An error occured while changing the VS window title: {0}", ex.Message));
            }
        }

        static IVsActivityLog mLog = ActivityLog.Get();
    }
}


