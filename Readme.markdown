SingleAppLauncher
=================
This program automatically launches the most recent installed version of a program. If your Windows application installs into a folder with a version number, you can bundle this program with your application to fascilitate easy program updates.

An Example
----------
- Root Folder
    - app.exe
    - app.exe.config
    - 0.91
        - program.exe
    - 0.92
        - program.exe

If app.exe.config is setup so that app.exe looks for and launches 'program.exe', it will automatically launch '0.92\program.exe'.
