Works with SolidWorks 2019

Usage:  
Place AMD Bike Assembly to the disk C, full path must be the next:  
`C:\VAYU\AMD - RADEON\SKLOPNI_PINKY\AMD_Bike_by_paX.SLDASM`  

Place files from `release` directory to `C:\VAYU` folder and run `registrate_add_in.bat` as administrator.  

If you don't see add-in on the SolidWorks' Taskpane, go to `Tools` menu -> `Add-Ins...` -> find `BikeMacro` -> set checkbox.  

Make sure, that you configure GPU-Z to logging needed information (see `additional scripts/Readme.md`).  
Use GPU-Z to logging video-card information and `additional scripts/cpu_usage_logger.py` for logging cpu-load information.  
Use `logs_parser.py` from `additional scripts` directory to create charts from log files.  