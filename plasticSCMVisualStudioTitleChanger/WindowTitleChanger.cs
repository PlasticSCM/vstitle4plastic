using System;
using System.Threading;

using EnvDTE80;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    internal class WindowTitleChanger
    {
        internal WindowTitleChanger(DTE2 dte, IVsActivityLog log)
        {
            mBuilder = new WindowTitleBuilder();

            mDTE = dte;
            mDTE.Events.SolutionEvents.AfterClosing += SolutionClosed;
            mDTE.Events.SolutionEvents.Opened += SolutionOpened;

            mLog = log;
        }

        internal void Dispose()
        {
            mDTE.Events.SolutionEvents.AfterClosing -= SolutionClosed;
            mDTE.Events.SolutionEvents.Opened -= SolutionOpened;
        }

        internal void UpdateWindowTitleAsync(object state, EventArgs e)
        {
            System.Threading.Tasks.Task.Factory.StartNew(DoUpdateWindowTitle);
        }

        void DoUpdateWindowTitle()
        {
            try
            {
                lock (mUpdateWindowTitleLock)
                {
                    ChangeWindowTitle();
                }
            }
            catch (Exception ex)
            {
                mLog.LogEntry(
                    (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, 
                    "WindowTitleChanger",
                    string.Format("An error occured while updating the window title: {0}", ex.Message));
            }
        }

        void ChangeWindowTitle()
        {
            BeginInvokeOnUIThread(() =>
            {
                try
                {
                    string newTitle = mBuilder.BuildWindowTitle(mDTE);
                    System.Windows.Application.Current.MainWindow.Title = mDTE.MainWindow.Caption;
                    if (System.Windows.Application.Current.MainWindow.Title != newTitle)
                    {
                        System.Windows.Application.Current.MainWindow.Title = newTitle;
                    }
                }
                catch (Exception)
                {
                }
            });
        }

        public void BeginInvokeOnUIThread(Action action)
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;
            if (dispatcher == null)
                return;

            dispatcher.BeginInvoke(action);
        }

        void DisposeEvents()
        {
            mDTE.Events.SolutionEvents.AfterClosing -= SolutionClosed;
            mDTE.Events.SolutionEvents.Opened -= SolutionOpened;
        }

        void SolutionOpened()
        {
            mSelectorWatcher = new SelectorWatcher(mDTE.Solution.FullName, mBuilder, mLog);
            mSelectorWatcher.Initialize();
        }

        void SolutionClosed()
        {
            if (mSelectorWatcher == null)
                return;

            mSelectorWatcher.Dispose();
            mSelectorWatcher = null;
        }

        readonly object mUpdateWindowTitleLock = new object();

        DTE2 mDTE;
        WindowTitleBuilder mBuilder;
        SelectorWatcher mSelectorWatcher;
        IVsActivityLog mLog;
    }
}


