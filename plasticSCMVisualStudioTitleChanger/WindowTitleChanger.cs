using System;
using System.Threading;

using EnvDTE80;
using EnvDTE;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    internal class WindowTitleChanger
    {
        internal static void Initialize(DTE2 dte)
        {
            mInstance = new WindowTitleChanger(dte);
        }

        internal static void Dispose()
        {
            if (mInstance != null)
                return;

            mInstance.DisposeEvents();
        }

        internal static WindowTitleChanger GetInstance()
        {
            return mInstance;
        }

        internal void UpdateWindowTitleAsync(object state, EventArgs e)
        {
            System.Threading.Tasks.Task.Factory.StartNew(mInstance.DoUpdateWindowTitle);
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

        public static void BeginInvokeOnUIThread(Action action)
        {
            var dispatcher = System.Windows.Application.Current.Dispatcher;
            if (dispatcher == null)
                return;

            dispatcher.BeginInvoke(action);
        }

        WindowTitleChanger(DTE2 dte)
        {
            mBuilder = new WindowTitleBuilder();
            
            mDTE = dte;
            mDTE.Events.SolutionEvents.AfterClosing += SolutionClosed;
            mDTE.Events.SolutionEvents.Opened += SolutionOpened;
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

        static WindowTitleChanger mInstance;
        DTE2 mDTE;
        WindowTitleBuilder mBuilder;
    }
}


