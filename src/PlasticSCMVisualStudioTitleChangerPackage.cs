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

            mTitleChanger = new WindowTitleChanger((DTE2)(GetGlobalService(typeof(DTE))), log);
            this.ResetTitleTimer = new System.Windows.Forms.Timer { Interval = TIMER_INTERVAL };
            this.ResetTitleTimer.Tick += mTitleChanger.UpdateWindowTitleAsync;
            this.ResetTitleTimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            this.ResetTitleTimer.Dispose();
            mTitleChanger.Dispose();
            base.Dispose(disposing);
        }

        System.Windows.Forms.Timer ResetTitleTimer;
        WindowTitleChanger mTitleChanger;

        const int TIMER_INTERVAL = 1000;
    }
}
