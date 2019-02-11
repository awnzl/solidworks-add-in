from argparse import ArgumentParser, RawDescriptionHelpFormatter
import io
import re


def get_labels_row(file_name):
    '''
    returns an array of steps labels and
    an array of values of steps' execution time'''
    columns_num, labels, values = 0, [], []
    for line in open(file_name):
        if "Step" in line or "Total time" in line:
            line_data = line.split("\": ")
            labels.append(re.sub(",", '',line_data[0][2:]))
            labels.append('Deviation')
            values.append(float(line_data[1][:-2]))
            columns_num += 1
    return columns_num, labels, values


def get_data_row(file_name):
    '''returns an array of values of steps' execution time'''
    values = []
    for line in open(file_name):
        if "Step" in line or "Total time" in line:
            values.append(float(line.split("\": ")[1][:-2]))
    return values


def fill_file_content(content):
    'add csv content separator to data'
    return str(content) + ";\t"


def calculate_average(values, columns_num, rows_num):
    average_values_row = []
    for idx in range(columns_num):
        average_values_row.append(sum(values[i] for i in range(idx, columns_num * rows_num, columns_num)) / rows_num)
    return average_values_row


def calculate_deviations(values, columns_num, rows_num):
    result = []

    'fill first data-row'
    for v in values[:columns_num]:
        result.append(v)
        result.append(0.0)

    'switch-dictionary with average values of each column'
    av = dict(enumerate(values[:columns_num]))

    idx = 0
    for v in values[columns_num:]:
        result.append(v)
        result.append(v / av[idx % columns_num] - 1)
        idx += 1

    return result

def get_total_time_values_list(log_prefix, log_limit):
    values = []

    for idx in range(1, log_limit + 1):
        filename = r"c:\VAYU\\" + log_prefix + "_log" + str(idx) + ".json"
        for line in open(filename):
            if "Total time" in line:
                line_data = line.split("\": ")
                values.append(str(line_data[1][:-2]) + '\n')
    return values

def process(log_prefix, log_limit):
    log_path = r"c:\VAYU\\" + log_prefix + "_log1.json"

    columns_num, labels, values = get_labels_row(log_path)

    'add first row of csv file to list of the result'
    file_content = []
    file_content += map(fill_file_content, labels)
    file_content[-1] = file_content[-1][:-1] + '\n'

    for idx in range(2, log_limit + 1):
        values += get_data_row(r"c:\VAYU\\" + log_prefix + "_log" + str(idx) + ".json")

    'add second row with average values'
    average_values_row = calculate_average(values, columns_num, log_limit)
    values = calculate_deviations(average_values_row + values, columns_num, log_limit + 1)
    values = list(map(fill_file_content, values))

    'add new-lines to the end of each row'
    for idx in range(columns_num * 2 - 1, columns_num * 2 * log_limit, columns_num * 2):
        values[idx] = values[idx][:-1] + '\n'

    file = open(r"c:\VAYU\result.csv", 'w', encoding='utf-8')
    file.writelines(file_content + values)
    file.close()


def GetUsage():
    return ('''Usage:
    specify the log's prefix (for example 'bike' or 'folder\bike')
    and the number of the last log (5 for bike_log5.json):
        python logs_to_csv.py -p bike -n 5''')


if __name__ == '__main__':
    parser = ArgumentParser(formatter_class=RawDescriptionHelpFormatter,
                            description=GetUsage())
    parser.add_argument("-p", required=True)
    parser.add_argument("-n", required=True)
    args = parser.parse_args()
    process(str(args.p), int(args.n))