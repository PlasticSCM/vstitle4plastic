using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Windows.Forms;

namespace CodiceSoftware.VsTitle4Plastic
{
    internal class WindowTitleUpdater
    {
        internal WindowTitleUpdater(WindowTitleBuilder titleBuilder)
        {
            mTitleBuilder = titleBuilder;
        }

        internal void Start()
        {
            mResetTitleTimer = new Timer { Interval = TIMER_INTERVAL };
            mResetTitleTimer.Tick += UpdateWindowTitle;
            mResetTitleTimer.Start();
        }

        internal void Stop()
        {
            if (mResetTitleTimer == null)
                return;

            mResetTitleTimer.Tick -= UpdateWindowTitle;
            mResetTitleTimer.Dispose();
        }

        void UpdateWindowTitle(object sender, EventArgs e)
        {
            try
            {
                WindowTitleSetter.SetWindowTitle(
                    mTitleBuilder.BuildWindowTitle());
            }
            catch(Exception ex)
            {
                mLog.LogEntry(
                    (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_WARNING,
                    "WindowTitleUpdater",
                    string.Format("An error occured while updating the window title: {0}", ex.Message));
            }
        }

        Timer mResetTitleTimer;
        WindowTitleBuilder mTitleBuilder;

        const int TIMER_INTERVAL = 1000;

        static IVsActivityLog mLog = ActivityLog.Get();
    }
}
