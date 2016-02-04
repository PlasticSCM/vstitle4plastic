using System;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    [PackageRegistration(UseManagedResourcesOnly = true)]

    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidVsTitle4PlasticPkgString)]
    [ProvideAutoLoad(Microsoft.VisualStudio.VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class VsTitle4Plastic : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            ActivityLog.Initialize(GetService(typeof(SVsActivityLog)) as IVsActivityLog);

            mWindowTitleBuilder = new WindowTitleBuilder();
            mSelectorWatcher = new SelectorWatcher(mWindowTitleBuilder);
            mTitleUpdater = new WindowTitleUpdater(mWindowTitleBuilder);

            DTEService.Get().Events.SolutionEvents.AfterClosing += SolutionClosed;
            DTEService.Get().Events.SolutionEvents.Opened += SolutionOpened;

            mTitleUpdater.Start();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            DTEService.Get().Events.SolutionEvents.AfterClosing -= SolutionClosed;
            DTEService.Get().Events.SolutionEvents.Opened -= SolutionOpened;

            if (mTitleUpdater != null)
                mTitleUpdater.Stop();

            if (mSelectorWatcher != null)
                mSelectorWatcher.StopWatcher();
        }

        void SolutionOpened()
        {
            mSelectorWatcher.StartWatcher(DTEService.Get().Solution.FullName);
        }

        void SolutionClosed()
        {
            if (mSelectorWatcher == null)
                return;

            mSelectorWatcher.StopWatcher();
        }

        WindowTitleBuilder mWindowTitleBuilder;
        SelectorWatcher mSelectorWatcher;
        WindowTitleUpdater mTitleUpdater;
    }
}
