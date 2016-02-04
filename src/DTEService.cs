using EnvDTE;
using EnvDTE80;

using Microsoft.VisualStudio.Shell;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    static class DTEService
    {
        internal static DTE2 Get()
        {
            if (mInstance == null)
                mInstance = (DTE2)Package.GetGlobalService(typeof(DTE));

            return mInstance;
        }

        static DTE2 mInstance = null;
    }
}
