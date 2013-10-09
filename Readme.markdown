# SingleAppLauncher

This program automatically launches the most recent installed version of a program. If your Windows application installs into a folder with a version number, you can bundle this program with your application to fascilitate easy program updates.

## Example Usage

- Root Folder
    - app.exe
    - app.exe.config
    - 0.91
        - program.exe
    - 0.92
        - program.exe

app.exe.config contents:
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0" />
  </startup>
  <appSettings>
    <add key="ExecutablePath" value="program.exe" />
    <add key="LaunchArguments" value="" />
    <add key="WaitTime" value="0" />
    <add key="RunAsAdmin" value="No" />
  </appSettings>
</configuration>
```

app.exe will automatically launch '0.92\program.exe'. If you set the path of program's shortcuts to app.exe, the shortcut will remain valid even if the user installs an update.

## Program Options
You can specify app.exe options as parameters in your app.exe shortcut or in the app.exe.config file.

|Command-Line Argument|app.exe.config Option|Description                       |
|---------------------|---------------------|----------------------------------|
|path                 |ExecutablePath       |relative path of file to launch   |
|args                 |LaunchArguments      |arguments to use for launched file|
|wait                 |WaitTime             |time to wait before launch        |
|runAsAdmin           |RunAsAdmin           |No, Ask, Yes                      |
|help, ?              |n/a                  |Show cmdline help                 |

Settings specified as command-line arguments override settings in app.exe.config.

## License
MIT License. Please check LICENSE for full details.
