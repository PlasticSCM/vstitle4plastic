using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
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
        public plasticSCMVisualStudioTitleChangerPackage()
        {
        }

    }
}
