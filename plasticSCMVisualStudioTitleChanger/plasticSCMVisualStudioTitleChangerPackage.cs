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
    public sealed class plasticSCMVisualStudioTitleChangerPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            WindowTitleChanger.Initialize((DTE2)(GetGlobalService(typeof(DTE))));
            this.ResetTitleTimer = new System.Windows.Forms.Timer { Interval = TIMER_INTERVAL };
            this.ResetTitleTimer.Tick += WindowTitleChanger.GetInstance().UpdateWindowTitleAsync;
            this.ResetTitleTimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            this.ResetTitleTimer.Dispose();
            WindowTitleChanger.Dispose();
            base.Dispose(disposing);
        }

        System.Windows.Forms.Timer ResetTitleTimer;

        const int TIMER_INTERVAL = 30000;
    }
}
