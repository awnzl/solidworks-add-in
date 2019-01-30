To create chart, you need `bike_log.json`, `cpu_usage_log.csv` and `GPU-Z_Log.txt`.  

Bike log-files saves by macros in `C:\VAYU\` folder.  
`cpu_usage_log.csv` creates by `cpu_usage_logger.py` and stores to `Documets` folder.  

To save `GPU-Z_Log.txt` you need to configure logging in GPU-Z:  
First, go to settings and leave set only the next checkboxes: GPU Core Clock, GPU Memory Clock, GPU Load.  
Than close settings and go to `Sensors` tab and set checkbox to `Log to file` and specify the `Document` folder for saving the log file.  


Use `bike_logs_parser.py` to create chart:  
    `python bike_logs_parser.py -l /c/VAYU/bike_log2.json`  

