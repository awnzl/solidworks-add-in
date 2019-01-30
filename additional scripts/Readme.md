To create a chart, you need `bike_log.json`, `cpu_usage_log.csv` and `GPU-Z_Log.txt`.  

Bike log-files saves by macros to `C:\VAYU\` folder.  
`cpu_usage_log.csv` creates by `cpu_usage_logger.py` and stores to `Documets` folder.  

To save `GPU-Z_Log.txt` you need to configure logging in GPU-Z:  
First, go to settings and leave set only the next checkboxes: GPU Core Clock, GPU Memory Clock, GPU Load.  
Than close settings and go to `Sensors` tab and set a checkbox to `Log to file` and choose the `Document` folder for saving the log file.  

You need to run `cpu_usage_logger.py` and GPU-Z before starting executing of the macro.  
Before using `bike_logs_parser.py` you must to stop `cpu_usage_logger.py` to save log file (to stop - press `ctrl + c`).  

Use `bike_logs_parser.py` to create chart:  
    `python bike_logs_parser.py -l /c/VAYU/bike_log2.json`  