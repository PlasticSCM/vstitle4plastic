using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;

using EnvDTE80;
using EnvDTE;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    [PackageRegistration(UseManagedResourcesOnly = true)]

    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidplasticSCMVisualStudioTitleChangerPkgString)]
    [ProvideAutoLoad(Microsoft.VisualStudio.VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class PlasticSCMVisualStudioTitleChangerPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            IVsActivityLog log = GetService(typeof(SVsActivityLog)) as IVsActivityLog;

            mDTE = (DTE2)(GetGlobalService(typeof(DTE)));


            mBuilder = new WindowTitleBuilder(mDTE);
            mTitleChanger = new WindowTitleChanger(mDTE, log);
            mSelectorWatcher = new SelectorWatcher(mBuilder, log);
            mTitleUpdater = new WindowTitleUpdater(mBuilder);

            mDTE.Events.SolutionEvents.AfterClosing += SolutionClosed;
            mDTE.Events.SolutionEvents.Opened += SolutionOpened;

            mTitleUpdater.Start();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            mDTE.Events.SolutionEvents.AfterClosing -= SolutionClosed;
            mDTE.Events.SolutionEvents.Opened -= SolutionOpened;

            if (mTitleUpdater != null)
                mTitleUpdater.Stop();

            if (mSelectorWatcher != null)
                mSelectorWatcher.Dispose();
        }

        void SolutionOpened()
        {
            mSelectorWatcher.Initialize(mDTE.Solution.FullName);
        }

        void SolutionClosed()
        {
            if (mSelectorWatcher == null)
                return;

            mSelectorWatcher.Dispose();
        }


        WindowTitleBuilder mBuilder;
        WindowTitleChanger mTitleChanger;
        SelectorWatcher mSelectorWatcher;
        WindowTitleUpdater mTitleUpdater;

        DTE2 mDTE;

        const int TIMER_INTERVAL = 1000;
    }
}
