# vstitle4plastic
Visual Studio extension to show the Plastic SCM workspace selector inside the Visual Studio window title.

This plugin modifies in a "hacked" way the window title to include information about the current selector. The extension needs to recalculate the whole window title every time, so this could cause the window title not to be the expected under certain conditions.

The selector is only shown when a solution is open, when one or several documents are open but no solution is open, it does not show any selector info (because each document could be on a different workspace).

If you have a custom "WkConfigDir" setting on your client.conf file, you must edit the DEFAULT_WK_CONFIG_DIR constant value on "SelectorWatcher.cs" file in the plugin source code and set its value to your custom value for this setting.

When adding a solution to source control, if the solution is not contained inside a plastic workspace, you will need to close and reopen the solution again to see the selector in the window title. The same thing occurs if you create the plastic workspace outside Visual Studio.

The solution includes a reference to CmdRunner.dll (source code available at https://github.com/PlasticSCM/plastic-cmdrunner). This utility allows to run commands from an external application. If you want to include the CmdRunner source code inside the plugin solution, you will have to generate a key file to sing it. 

To install the package, compile the solution with Visual Studio and execute the VsTitle4Plastic.vsix file on the output directory.
