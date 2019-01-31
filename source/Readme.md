To install add-in into your SolidWorks:  
Run cmd.exe as Administrator  
Then execute:
```
C:\Windows\Microsoft.NET\Framework64\v_LAST_INSTALLED_VERSION\RegAsm.exe /codebase "Path\To\Your\solid_macro.dll"
```
example:
```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe /codebase "C:\VAYU\solid_macro.dll"
```

Usage:  
Place AMD Bike Assembly to the disk C, full path must be the next:  
`C:\VAYU\AMD - RADEON\SKLOPNI_PINKY\AMD_Bike_by_paX.SLDASM`  
Open SolidWorks, on the taskpane find an icon with letter `b`, then open the assembly (press first button).  
If you don't see it, go to "Tools" menu, then choose "add-in" and find in the list of add-ins "solid_macro" - set checkbox.  

First - open an assembly.  
Then execute script-button.  

You might execute macros multiple times, but before that each time you need to delete all changes.  
For each execution during SW working session, the macros save a new log file with adding index.  
Log files will be saved in `c:\VAYU\` folder.  

For screw macro just press `screw macro` button.  