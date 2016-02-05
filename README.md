# vstitle4plastic
Visual Studio extension to show the Plastic SCM workspace selector inside the Visual Studio window title.

This plugin modifies in a "hacked" way the window title to include information about the current selector. The extension needs to recalculate the whole window title every time, so this could cause the window title to contain unexpected data under certain conditions.

The selector is only shown when a solution is open. When one or several documents are opened with no active solution, the extension will not show any selector info: each document could be in a different workspace.

If you have a custom "WkConfigDir" setting in your client.conf file, you must edit the DEFAULT_WK_CONFIG_DIR constant value located in the "SelectorWatcher.cs" file (see plugin source code) and set its value to your custom value.

When adding a solution to source control, if the solution is not contained inside a plastic workspace you will need to close and reopen the solution again to see the selector in the window title. The same thing occurs if the plastic workspace is created outside Visual Studio.

The solution includes a reference to CmdRunner.dll (source code available at https://github.com/PlasticSCM/plastic-cmdrunner). This tool allows developers to run commands from an external application. If you want to include the CmdRunner source code inside the plugin solution, you will have to generate a key file to sign it. 

To install the package, compile the solution with Visual Studio and execute the VsTitle4Plastic.vsix file at the output directory.
