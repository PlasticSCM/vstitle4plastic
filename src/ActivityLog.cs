using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    static class ActivityLog
    {
        internal static void Initialize(IVsActivityLog log)
        {
            mInstance = log;
        }

        internal static IVsActivityLog Get()
        {
            if (mInstance == null)
                throw new Exception("Log is not initialized");

            return mInstance;
        }

        static IVsActivityLog mInstance;
    }
}
