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
            UpdateWindowTitle();
        }

        internal void UpdateWindowTitle()
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
            mDTE.Events.DebuggerEvents.OnEnterBreakMode += OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnEnterRunMode += OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnEnterDesignMode += OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnContextChanged += OnIdeEvent;
            mDTE.Events.SolutionEvents.AfterClosing += SolutionClosed;
            mDTE.Events.SolutionEvents.Opened += SolutionOpened;
            mDTE.Events.SolutionEvents.Renamed += OnIdeEvent;
            mDTE.Events.WindowEvents.WindowCreated += OnIdeEvent;
            mDTE.Events.WindowEvents.WindowClosing += OnIdeEvent;
            mDTE.Events.WindowEvents.WindowActivated += OnIdeEvent;
            mDTE.Events.DocumentEvents.DocumentOpened += OnIdeEvent;
            mDTE.Events.DocumentEvents.DocumentClosing += OnIdeEvent;
        }

        void DisposeEvents()
        {
            mDTE.Events.DebuggerEvents.OnEnterBreakMode -= OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnEnterRunMode -= OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnEnterDesignMode -= OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnContextChanged -= OnIdeEvent;
            mDTE.Events.SolutionEvents.AfterClosing -= SolutionClosed;
            mDTE.Events.SolutionEvents.Opened -= SolutionOpened;
            mDTE.Events.SolutionEvents.Renamed -= OnIdeEvent;
            mDTE.Events.WindowEvents.WindowCreated -= OnIdeEvent;
            mDTE.Events.WindowEvents.WindowClosing -= OnIdeEvent;
            mDTE.Events.WindowEvents.WindowActivated -= OnIdeEvent;
            mDTE.Events.DocumentEvents.DocumentOpened -= OnIdeEvent;
            mDTE.Events.DocumentEvents.DocumentClosing -= OnIdeEvent;
        }

        void SolutionOpened()
        {
            SelectorWatcher.ResetWatcher(mDTE.Solution.FullName, mBuilder);
            OnIdeEvent();
        }

        void SolutionClosed()
        {
            mBuilder.SetSelector(string.Empty);
            SelectorWatcher.Dispose();
            OnIdeEvent();
        }
        
        void OnIdeEvent(Window gotfocus, Window lostfocus)
        {
            OnIdeEvent();
        }

        void OnIdeEvent(Document document)
        {
            OnIdeEvent();
        }

        void OnIdeEvent(Window window)
        {
            OnIdeEvent();
        }

        void OnIdeEvent(string oldname)
        {
            OnIdeEvent();
        }

        void OnIdeEvent(dbgEventReason reason)
        {
            OnIdeEvent();
        }

        void OnIdeEvent(dbgEventReason reason, ref dbgExecutionAction executionaction)
        {
            OnIdeEvent();
        }

        void OnIdeEvent(Process newProc, Program newProg, EnvDTE.Thread newThread, StackFrame newStkFrame)
        {
            OnIdeEvent();
        }

        void OnIdeEvent()
        {
            UpdateWindowTitleAsync(this, EventArgs.Empty);
        }

        readonly object mUpdateWindowTitleLock = new object();

        static WindowTitleChanger mInstance;
        DTE2 mDTE;
        WindowTitleBuilder mBuilder;
    }
}


