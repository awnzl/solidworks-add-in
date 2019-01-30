import psutil
import datetime
import os

# Usage:
#   python cpu_usage_logger
#   to exit - press ctrl+c

# with open((os.path.expanduser("~") + "\\Documents\\cpu_usage_log.csv"), 'a') as f:
#     try:
#         while True:
#             f.write(datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f, ") +
#                             str(psutil.cpu_percent(interval=1)) + ",\n")
#     except KeyboardInterrupt:
#         pass

with open(os.path.join(os.path.expanduser("~"), "Documents", "cpu_usage_log.csv"), 'a') as f:
    try:
        while True:
            f.write(datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S.%f, ") +
                            str(psutil.cpu_percent(interval=1)) + ",\n")
    except KeyboardInterrupt:
        pass