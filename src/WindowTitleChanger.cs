using System;
using System.Threading;

using EnvDTE80;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    internal class WindowTitleChanger
    {
        internal WindowTitleChanger(
            DTE2 dte, 
            IVsActivityLog log)
        {
            mDTE = dte;
            mLog = log;
        }

        internal static void ChangeWindowTitle(string newTitle)
        {
            try
            {
                System.Windows.Application.Current.MainWindow.Title = mDTE.MainWindow.Caption;
                if (System.Windows.Application.Current.MainWindow.Title != newTitle)
                {
                    System.Windows.Application.Current.MainWindow.Title = newTitle;
                }
            }
            catch (Exception ex)
            {
                mLog.LogEntry(
                    (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
                    "WindowTitleChanger",
                    string.Format("An error occured while changing the VS window title: {0}", ex.Message));
            }
        }

        static void BeginInvokeOnUIThread(Action action)
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;
            if (dispatcher == null)
                return;

            dispatcher.BeginInvoke(action);
        }

        static DTE2 mDTE;
        static IVsActivityLog mLog;
    }
}


