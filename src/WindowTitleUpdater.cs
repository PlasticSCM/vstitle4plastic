using System;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    internal class WindowTitleUpdater
    {
        internal WindowTitleUpdater(WindowTitleBuilder titleBuilder)
        {
            mTitleBuilder = titleBuilder;
        }

        internal void Start()
        {
            this.mResetTitleTimer = new System.Windows.Forms.Timer { Interval = TIMER_INTERVAL };
            this.mResetTitleTimer.Tick += UpdateWindowTitle;
            this.mResetTitleTimer.Start();
        }

        internal void Stop()
        {
            if (this.mResetTitleTimer == null)
                return;

            this.mResetTitleTimer.Tick -= UpdateWindowTitle;
            this.mResetTitleTimer.Dispose();
        }

        void UpdateWindowTitle(object sender, EventArgs e)
        {
            WindowTitleChanger.UpdateWindowTitle(mTitleBuilder.BuildWindowTitle());
        }

        System.Windows.Forms.Timer mResetTitleTimer;
        WindowTitleBuilder mTitleBuilder;

        const int TIMER_INTERVAL = 1000;
    }
}
