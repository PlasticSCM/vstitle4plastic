using System;
using System.Threading;

using EnvDTE80;
using EnvDTE;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    internal class WindowTitleChanger
    {
        internal WindowTitleChanger(DTE2 dte)
        {
            mBuilder = new WindowTitleBuilder();

            mDTE = dte;
            mDTE.Events.SolutionEvents.AfterClosing += SolutionClosed;
            mDTE.Events.SolutionEvents.Opened += SolutionOpened;
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

            }
        }

        void ChangeWindowTitle()
        {
            try
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
            catch (Exception ex)
            {
            }
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
            SelectorWatcher.ResetWatcher(mDTE.Solution.FullName, mBuilder);
        }

        void SolutionClosed()
        {
            SelectorWatcher.Dispose();
        }

        readonly object mUpdateWindowTitleLock = new object();

        DTE2 mDTE;
        WindowTitleBuilder mBuilder;
    }
}


