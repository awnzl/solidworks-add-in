import io
import os
import re
import wmi
import json
import datetime
from backports import csv   # pip install backports.csv
import psutil               # pip install psutil
import matplotlib           # pip install matplotlib
import matplotlib.pyplot as plt
from matplotlib.gridspec import GridSpec
from argparse import ArgumentParser, RawDescriptionHelpFormatter

'''
install next:
  pip install psutil
  pip install matplotlib
  pip install backports.csv

Setup and configure GPU-Z:
  Set Log file to Documents with the name 'GPU-Z_Log'
  In Settings leave only 'GPU Core Clock', 'GPU Memory Clock' and
  'GPU Load' checkbox

Example of desired GPU_log:
        Date        , GPU Core Clock [MHz] , GPU Memory Clock [MHz] , GPU Load [%] ,
        2019-01-21 11:48:13 ,              300.0   ,                300.0   ,          0   ,
        2019-01-21 11:48:14 ,              300.0   ,                300.0   ,          0   ,

Check log file's time format:
        example: 2018-12-19 15:39:37.843
        pattern: %Y-%m-%d %H:%M:%S
if log file's format is different, adjust pattern in
string: 'startTime = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")'
'''

def getTitle(isScrewMacro):
    computer = wmi.WMI()
    os_info = computer.Win32_OperatingSystem()[0]
    cpu_info = computer.Win32_Processor()[0]
    try:
        gpu_info = computer.Win32_VideoController()[0]
    except:
        gpu_info = '???'

    system_ram = float(os_info.TotalVisibleMemorySize) / 1048576  # KB to GB

    return (("Screw Macro: " if isScrewMacro else "Bike Macro: ") +
            cpu_info.Name.strip() + " | " +
            "RAM: " + str(system_ram).strip()[:5] + " Gb | " +
            gpu_info.Name.strip() + ", " +
            "Driver ver: " + gpu_info.DriverVersion.strip())


def parsOHMlogs(stime, cpu_data, gpu_data, logfile):
    isNotFound = True

    with io.open(logfile, newline='', encoding='utf-8') as f:
        for line in f:
            row_array = line.split(',')
            #0, 1,               2,               3,               4,               5,               6,               7,               8,               9,               10,              11,         12,         13,         14,                     15,             16,                 17,               18,               19,                 20,              21,                  22
            # ,/amdcpu/0/load/1,/amdcpu/0/load/2,/amdcpu/0/load/3,/amdcpu/0/load/4,/amdcpu/0/load/0,/amdcpu/1/load/1,/amdcpu/1/load/2,/amdcpu/1/load/3,/amdcpu/1/load/4,/amdcpu/1/load/0,/ram/load/0,/ram/data/0,/ram/data/1,/atigpu/0/temperature/0,/atigpu/0/fan/0,/atigpu/0/control/0,/atigpu/0/clock/0,/atigpu/0/clock/1,/atigpu/0/voltage/0,/atigpu/0/load/0,/hdd/0/temperature/0,/hdd/0/load/0
            # Time,"CPU Core #1","CPU Core #2","CPU Core #3","CPU Core #4","CPU Total","CPU Core #1","CPU Core #2","CPU Core #3","CPU Core #4","CPU Total","Memory","Used Memory","Available Memory","GPU Core","GPU Fan","GPU Fan","GPU Core","GPU Memory","GPU Core","GPU Core","Temperature","Used Space"
            # 0,    1,            2,            3,            4,            5,          6,            7,            8,            9,            10,         11,      12,           13,                14,        15,       16,       17,        18,          19,        20,        21,           22
            # 23 for 8 cores of cpu.
            # 12/21/2018 13:06:53,20.3125,11.1111107,20.3125,9.523809,15.3149843,15.625,14.0625,20.3125,15.625,16.40625,44.3697853,14.1754227,17.7729454,36,0,26,33,167,0.75000006,0,31,37.11359
            # 12/21/2018 13:06:54,3.125,0,0,0,0.78125,3.125,0,6.25,1.5625,2.734375,44.3686752,14.1750679,17.7733,35,0,26,30,167,0.75000006,0,31,37.11359
            nCpuCores = 8.0 # need to be adjusted for different CPUs
            if isNotFound and stime == row_array[0]:
                isNotFound = False
            elif not isNotFound:
                cpu_data.append((float(row_array[1]) +
                                 float(row_array[2]) +
                                 float(row_array[3]) +
                                 float(row_array[4]) +
                                 float(row_array[6]) +
                                 float(row_array[7]) +
                                 float(row_array[8]) +
                                 float(row_array[9])) / nCpuCores)
                gpu_data.append(float(row_array[20]))


def getCPUlogData(stime, cpu_log_file):
    ret = []
    isNotFound = True

    with io.open(cpu_log_file, newline='', encoding='utf-8') as f:
        for line in f:
            row_array = line.split(',')
                                       # %Y-%m-%d %H:%M:%S.%f
                                       # 2018-12-21 11:57:56.608369, 23.6,
            if isNotFound and stime == row_array[0].split('.')[0]:
                isNotFound = False
            elif not isNotFound:
                ret.append(float(row_array[1]))

    return ret


def getGPUlogData(stime, gpu_log_file):

    load, mem, core = [], [], []
    isNotFound = True

    # with io.open(gpu_log_file, newline='', encoding='utf-8') as f:
    #     for row in csv.reader(f):
    #         row_array = re.sub("['\[\]]", '', str(row)).split(',')

    #         if isNotFound and stime == row_array[0].split('.')[0].strip():
    #             isNotFound = False
    #         elif not isNotFound and row_array[3].strip().isdigit():
    #             load.append(int(row_array[3]))
    #             mem.append(float(row_array[2]) / 100)
    #             core.append(float(row_array[1]) / 12.5)

    for line in io.open(gpu_log_file, newline='', encoding='utf-8'):
        row_array = line.split(',')

        if isNotFound and stime == row_array[0].split('.')[0].strip():
            isNotFound = False
        elif not isNotFound and row_array[3].strip().isdigit():
            load.append(int(row_array[3]))
            mem.append(float(row_array[2]) / 100)
            core.append(float(row_array[1]) / 12.5)

    # ret = []
    # idx = 0
    # while idx < (len(gpu) - 10):
    #     ret.append((int(gpu[idx]) +
    #                 int(gpu[idx+1])) / 2.0)
    #     idx+=2
    return load, mem, core


# return arrays with the timestamps and labels
def getStepsData(logpath):
    stime, time_limits, labels, total_time = 0, [0], [], 0
    all_time = 0

    with open(logpath) as f:
        for line in f:
            if "Step" in line:
                line_data = line.split("\": ")
                all_time += float(line_data[1][:-2])
                time_limits.append(all_time)
                labels.append(line_data[0][2:])
            elif "Start" in line:
                stime = datetime.datetime.strptime(line.split("\": \"")[1][:-3], "%d/%m/%y %I:%M:%S %p")
            elif "Total time" in line:
                total_time = float(line.split("\": ")[1][:-2])

    return stime, time_limits, labels, total_time


def process(steps_log_path, isScrewMacro, isOHM, ohmLogPath):
    docsFolder = (os.path.expanduser("~") + "\\Documents\\")

    # uses for mark steps on the chart
    startTime, steps_limits, labels, chart_length = getStepsData(steps_log_path)

    cpu_data, gpu_load, gpu_mem, gpu_core = [], [], [], []
    if isOHM:
        parsOHMlogs(startTime.strftime("%m/%d/%Y %H:%M:%S"), # 12/21/2018 13:06:54
                    cpu_data, gpu_load, ohmLogPath)
    else:
        try:
            cpu_data = getCPUlogData(startTime.strftime("%Y-%m-%d %H:%M:%S"), # 2018-12-20 17:47:03.733
                                docsFolder + "cpu_usage_log.csv")
        except Exception as ex:
            print("function getCPUlogData:", ex)
        try:
            gpu_load, gpu_mem, gpu_core = getGPUlogData(startTime.strftime("%Y-%m-%d %H:%M:%S"), # 2018-12-20 17:47:03.733
                                docsFolder + "GPU-Z_Log.txt")
        except Exception as ex:
            print("function getGPUlogData:", ex)


    fig = plt.figure()

    fig.set_figheight(8)
    fig.set_figwidth(15 if isScrewMacro else 35)

    # first sets the size (num of rows and columns), second sets the space in the first (by indexes)
    ax0 = plt.subplot2grid((7, 1), (0, 0), rowspan=4)
    ax1 = plt.subplot2grid((7, 1), (4, 0), rowspan=1)
    ax2 = plt.subplot2grid((7, 1), (5, 0), rowspan=2)

    chart_length += 5 if isScrewMacro else 50
    ax0.set_ylim(0, 100)
    ax0.set_xlim(0, chart_length)
    ax1.get_yaxis().set_ticks([])
    ax1.set_xlim(0, chart_length)
    ax2.set_ylim(15, 100)
    ax2.set_xlim(0, chart_length)

    for idx in range(len(steps_limits) - 1):
        ax0.axvspan(steps_limits[idx], steps_limits[idx+1], 0, 100, facecolor=('#d5dbe7' if idx % 2 else '#a4c5fc'), alpha=0.5)
        ax1.axvspan(steps_limits[idx], steps_limits[idx+1], 0, 100, facecolor=('#d5dbe7' if idx % 2 else '#a4c5fc'), alpha=0.5)
        ax2.axvspan(steps_limits[idx], steps_limits[idx+1], 0, 100, facecolor=('#d5dbe7' if idx % 2 else '#a4c5fc'), alpha=0.5)

    for time, label in zip(steps_limits, labels):
        ax0.text(time, 100, label.split('(')[1][:-1], rotation=90, verticalalignment='top')
        ax2.text(time, 100, label.split('(')[0], rotation=90, verticalalignment='top')


    ax0.plot(cpu_data, '-g', label='cpu')
    ax0.plot(gpu_load, '-b', label='gpu_load')
    ax1.plot(gpu_mem, '-r', label='gpu_mem_clock')
    ax2.plot(gpu_core, '-y', label='gpu_core_clock')
    ax0.legend()
    ax1.legend()
    ax2.legend()

    ax0.set(ylabel='Load (%)', title=getTitle(isScrewMacro))
    ax1.set(ylabel='min/max')
    ax2.set(xlabel='time (s)', ylabel='Clock (%)')

    fig.savefig(str(datetime.datetime.now().strftime("%Y.%m.%d_%H.%M.%S")) + "_log.png")
    plt.show()


def getUsage():
    return ('''Instruction:
    Run GPU-Z
    Run cpu_usage_logger.py
    Run SolidWorks' bike or screw macros
    After macros will be done, stop cpu_usage_logger.py to save
    cpu-usage log-file and then run logs_parser.py to build results graph:
        python logs_parser.py -l[s] path/to/log.json
        -s - option for screw-macro log-files''')


if __name__ == '__main__':
    parser = ArgumentParser(formatter_class=RawDescriptionHelpFormatter,
                            description=getUsage())
    parser.add_argument("-l", required=True)
    parser.add_argument("-s", action="store_true")
    parser.add_argument("-ohm", nargs=1)
    args = parser.parse_args()
    process(str(args.l),
            True if args.s else False,
            True if args.ohm else False,
            re.sub("['\[\]]", '', str(args.ohm)) if args.ohm else None)

