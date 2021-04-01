import time
import subprocess
import os


def runBashCommand(bashCommand):
	process = subprocess.Popen(bashCommand.split(), stdout=subprocess.PIPE)
	output, error = process.communicate()
	return output, error


def runBashCommandWithDisplay(bashCommand):
	QuestOutput = runBashCommand(bashCommand)
	if QuestOutput[1] is None:
		print("	> Sucess")
	else:
		print(" > Error : {}".format(QuestOutput[1]))


def variance(v):
	mean = sum(v) / len(v)
	var = 0
	for w in v:
		var += (w-mean)**2 / len(v)
	return var


it = 50
delays = []

commandPush = "adb shell touch sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data/pingTest.txt"
commandPull = "adb pull sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data/pingTest.txt {}".format(os.getcwd().replace("\\", "/") + "/")
commandDelete = "adb shell rm sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data/pingTest.txt"

for i in range(it):
	startTime = time.time()
	print(i)
	runBashCommandWithDisplay(commandPush)
	done = False
	while not done:
		print("	> waiting...")
		runBashCommandWithDisplay(commandPull)
		with open("pingTest.txt", 'r') as f:
			done = f.readline() == "test"
	delays.append(time.time() - startTime)
	runBashCommandWithDisplay(commandDelete)
	with open("pingTest.txt", 'r+') as f:
		f.truncate(0)


print("ping : {} seconds in average with standart deviation of {}".format(sum(delays)/it, variance(delays)**0.5))

#ping : 0.11846395015716553 seconds in average with standart deviation of 0.011223303120945188