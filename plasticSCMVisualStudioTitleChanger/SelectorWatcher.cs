
using System.IO;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    internal class SelectorWatcher
    {
        internal static void ResetWatcher(string newWkPath, WindowTitleBuilder builder)
        {
            mWkPath = newWkPath;

            if(mWatcher != null)
                mWatcher.Dispose();

            Initialize(newWkPath);

            mBuilder = builder;
            UpdateSelector(builder);
        }

        internal static void Dispose()
        {
            if (mWatcher == null)
                return;

            mWatcher.Dispose();
        }

        static void Initialize(string wkspacePath)
        {
            mWkPath = wkspacePath;
            string plasticWkFolder = Path.Combine(wkspacePath, ".plastic");

            mWatcher = new FileSystemWatcher(plasticWkFolder);
            mWatcher.IncludeSubdirectories = false;
            mWatcher.NotifyFilter = NotifyFilters.FileName;
            mWatcher.EnableRaisingEvents = true;

            mWatcher.Path = "plastic.selector";
            mWatcher.NotifyFilter = NotifyFilters.LastWrite;

            mWatcher.Changed += new FileSystemEventHandler(OnSelectorChanged);
        }


        static void OnSelectorChanged(object sender, FileSystemEventArgs e)
        {
            UpdateSelector(mBuilder);
        }

        internal static void UpdateSelector(WindowTitleBuilder builder)
        {
            builder.SetSelector("");
        }

        static WindowTitleBuilder mBuilder;
        static FileSystemWatcher mWatcher;
        static string mWkPath;
    }
}
