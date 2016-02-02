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
                        string newTitle = "Hello world";
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
            mDTE = dte;
            mDTE.Events.DebuggerEvents.OnEnterBreakMode += this.OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnEnterRunMode += this.OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnEnterDesignMode += this.OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnContextChanged += this.OnIdeEvent;
            mDTE.Events.SolutionEvents.AfterClosing += this.OnIdeEvent;
            mDTE.Events.SolutionEvents.Opened += this.OnIdeEvent;
            mDTE.Events.SolutionEvents.Renamed += this.OnIdeEvent;
            mDTE.Events.WindowEvents.WindowCreated += this.OnIdeEvent;
            mDTE.Events.WindowEvents.WindowClosing += this.OnIdeEvent;
            mDTE.Events.WindowEvents.WindowActivated += this.OnIdeEvent;
            mDTE.Events.DocumentEvents.DocumentOpened += this.OnIdeEvent;
            mDTE.Events.DocumentEvents.DocumentClosing += this.OnIdeEvent;
        }

        void DisposeEvents()
        {
            mDTE.Events.DebuggerEvents.OnEnterBreakMode -= this.OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnEnterRunMode -= this.OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnEnterDesignMode -= this.OnIdeEvent;
            mDTE.Events.DebuggerEvents.OnContextChanged -= this.OnIdeEvent;
            mDTE.Events.SolutionEvents.AfterClosing -= this.OnIdeEvent;
            mDTE.Events.SolutionEvents.Opened -= this.OnIdeEvent;
            mDTE.Events.SolutionEvents.Renamed -= this.OnIdeEvent;
            mDTE.Events.WindowEvents.WindowCreated -= this.OnIdeEvent;
            mDTE.Events.WindowEvents.WindowClosing -= this.OnIdeEvent;
            mDTE.Events.WindowEvents.WindowActivated -= this.OnIdeEvent;
            mDTE.Events.DocumentEvents.DocumentOpened -= this.OnIdeEvent;
            mDTE.Events.DocumentEvents.DocumentClosing -= this.OnIdeEvent;
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
    }
}


