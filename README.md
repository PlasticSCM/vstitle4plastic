# vstitle4plastic

Visual Studio extension to show the current branch, label or changeset where the Plastic SCM workspace is pointing.

This plugin modifies in a "hacked" way the window title to include information about the Plastic SCM workspace. The extension needs to recalculate the whole window title every time, so this could cause the window title to contain unexpected data under certain conditions.

The window title is only customized when a solution is open. When one or several documents are opened with no active solution, the extension will not show any Plastic SCM related info: each document could be in a different workspace.

If you have a custom "WkConfigDir" setting in your client.conf file, you must edit the DEFAULT_WK_CONFIG_DIR constant value located in the "SelectorWatcher.cs" file (see plugin source code) and set its value to your custom value. The same happens with DEFAULT_PLASTIC_COMMAND constant if you have a custom command name for Plastic (default is "cm").

When adding a solution to source control, if the solution is not contained inside a plastic workspace you will need to close and reopen the solution again to see the workspace information in the window title. The same thing occurs if the plastic workspace is created outside Visual Studio.

The solution includes a reference to CmdRunner.dll (source code available at https://github.com/PlasticSCM/plastic-cmdrunner). This tool allows developers to run commands from an external application. If you want to include the CmdRunner source code inside the plugin solution, you will have to generate a key file to sign it. 

**Supported versions: greater or equal than Visual Studio 2012.**

These are some examples of customized window title with branch, label and changeset info:

![Window title with branch info](screenshots/branchTitle.png)

![Window title with label info](screenshots/labelTitle.png)

![Window title with changeset info](screenshots/changesetTitle.png)


