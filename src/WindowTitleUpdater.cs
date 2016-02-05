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
            WindowTitleSetter.SetWindowTitle(
                mTitleBuilder.BuildWindowTitle());
        }

        Timer mResetTitleTimer;
        WindowTitleBuilder mTitleBuilder;

        const int TIMER_INTERVAL = 1000;
    }
}
