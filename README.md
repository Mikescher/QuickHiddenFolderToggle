QuickHiddenFolderToggle
=======================

Displays an icon in the Windows TrayNotification Area to quickly toggle the explorer "show hidden files/directories" option with a single click.

# [> Download Latest](https://github.com/Mikescher/QuickHiddenFolderToggle/releases)


## How it works

It change the value of

~~~
HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\Hidden
~~~

and sends a refresh to all Windows Explorer instances.
