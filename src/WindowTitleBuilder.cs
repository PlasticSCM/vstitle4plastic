using System;
using System.IO;
using System.Text.RegularExpressions;

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CodiceSoftware.plasticSCMVisualStudioTitleChanger
{
    internal class WindowTitleBuilder
    {
        internal string BuildWindowTitle()
        {
            if (mIdeName == null && DTEService.Get().MainWindow != null)
                mIdeName = GetIDEName(SELECTOR_PATTERN);

            if (string.IsNullOrEmpty(mIdeName))
                return null;

            return GetNewTitle(mIdeName, GetWindowTitlePattern());
        }

        internal void SetSelector(string selector)
        {
            lock (mSelectorLock)
            {
                mSelector = selector;
            }
        }

        string GetNewTitle(string ideName, string pattern)
        {
            DTE2 dte = DTEService.Get();
            Solution solution = dte.Solution;
            Document activeDocument = dte.ActiveDocument;
            Window activeWindow = dte.ActiveWindow;

            if (activeDocument == null && (solution == null || string.IsNullOrEmpty(solution.FullName)))
            {
                var window = dte.ActiveWindow;
                if (window == null || window.Caption == dte.MainWindow.Caption)
                {
                    return ideName;
                }
            }

            pattern = pattern.Replace(DOCUMENT_NAME, GetActiveDocumentName(activeDocument, activeWindow));
            pattern = pattern.Replace(SOLUTION_NAME, GetSolutionName(solution));
            pattern = pattern.Replace(IDE_NAME, ideName);
            pattern = pattern.Replace(PLASTIC_SELECTOR, GetSelectorString());

            return pattern;
        }

        string GetWindowTitlePattern()
        {
            DTE2 dte = DTEService.Get();
            Solution solution = dte.Solution;

            if (solution == null || solution.FullName == string.Empty)
            {
                var document = dte.ActiveDocument;
                var window = dte.ActiveWindow;
                if ((document == null || string.IsNullOrEmpty(document.FullName)) &&
                    (window == null || string.IsNullOrEmpty(window.Caption)))
                    return IDE_NAME;

                return string.Format("{0} - {1}", DOCUMENT_NAME, IDE_NAME);
            }

            if (dte.Debugger == null || dte.Debugger.CurrentMode == dbgDebugMode.dbgDesignMode)
                return string.Format("{0}{1} - {2}", SOLUTION_NAME, PLASTIC_SELECTOR, IDE_NAME);

            if (dte.Debugger.CurrentMode == dbgDebugMode.dbgBreakMode)
                return string.Format("{0} (Debugging){1} - {2}", SOLUTION_NAME, PLASTIC_SELECTOR, IDE_NAME);

            if (dte.Debugger.CurrentMode == dbgDebugMode.dbgRunMode)
                return string.Format("{0} (Running){1} - {2}", SOLUTION_NAME, PLASTIC_SELECTOR, IDE_NAME);

            return IDE_NAME;
        }

        string GetIDEName(string selectorPattern)
        {
            try
            {
                Match m = GetMatchingPattern(DTEService.Get().MainWindow.Caption, selectorPattern);

                if (!m.Success || m.Groups.Count < 2)
                    return null;

                if (m.Groups.Count >= 3)
                    return m.Groups[2].Captures[0].Value;

                return m.Groups[1].Captures[0].Value;
            }
            catch (Exception ex)
            {
                mLog.LogEntry(
                    (UInt32)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR,
                    "WindowTitleBuilder",
                    string.Format("Could'n get IDE name: {0}", ex.Message));
            }

            return string.Empty;
        }

        Match GetMatchingPattern(string currentTitle, string selectorPattern)
        {
            DTE2 dte = DTEService.Get();

            Match match = new Regex(
                  @"^(.*) - (" + dte.Name + ".*) " + Regex.Escape(string.Format("{0} {1}", selectorPattern, "(.*)")) + "$",
                  RegexOptions.RightToLeft).Match(currentTitle);

            if (match.Success)
                return match;

            match = new Regex(@"^(.*) - (" + dte.Name + @".* \(.+\)) \(.+\)$", RegexOptions.RightToLeft).Match(currentTitle);

            if (match.Success)
                return match;

            match = new Regex(@"^(.*) - (" + dte.Name + ".*)$", RegexOptions.RightToLeft).Match(currentTitle);

            if (match.Success)
                return match;

            match = new Regex(@"^(" + dte.Name + ".*)$", RegexOptions.RightToLeft).Match(currentTitle);
            return match;
        }

        string GetActiveDocumentName(Document activeDocument, Window activeWindow)
        {
            if (activeDocument != null)
                return Path.GetFileName(activeDocument.FullName);

            if (activeWindow != null && activeWindow.Caption != DTEService.Get().MainWindow.Caption)
                return activeWindow.Caption;

            return string.Empty;
        }

        string GetSolutionName(Solution solution)
        {
            return solution == null || string.IsNullOrEmpty(solution.FullName) ?
                string.Empty :
                Path.GetFileNameWithoutExtension(solution.FullName);
        }

        string GetSelectorString()
        {
            string selector = GetSelector();

            if (string.IsNullOrEmpty(selector))
                return selector;

            return string.Format(" - {0} {1} ", SELECTOR_PATTERN, selector);
        }

        string GetSelector()
        {
            lock (mSelectorLock)
            {
                return mSelector;
            }
        }

        string mIdeName;
        string mSelector = string.Empty;
        object mSelectorLock = new object();

        const string DOCUMENT_NAME = "[documentName]";
        const string SOLUTION_NAME = "[solutionName]";
        const string IDE_NAME = "[ideName]";
        const string PLASTIC_SELECTOR = "[selector]";

        const string SELECTOR_PATTERN = "PlasticSCM: ";

        static IVsActivityLog mLog = ActivityLog.Get();
    }
}