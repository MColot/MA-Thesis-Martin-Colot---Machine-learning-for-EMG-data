import subprocess
import re
import os
import threading
import argparse
import pytrigno

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


def isValidFileName(s):
	match = re.search("([a-z]*[A-Z]*[0-9]*)*", s)
	return match.group(0) == s



def emgDataCollection():
	global host, delsysIsRecording, EMGchannelCount, waitingForThread
	print("	> Sending recording signal to delsys")
	try:
		delsysRecorder = pytrigno.TrignoEMG(channel_range=(0, EMGchannelCount - 1), samples_per_read=270, host=host)
		data = []
		delsysRecorder.start()
		delsysIsRecording = True
		waitingForThread = False
		print("	> Sucess")
		while delsysIsRecording:
			data.append(delsysRecorder.read())
		delsysRecorder.stop()
		saveEmgData(data)
	except Exception as e:
		print("	> Failure : {}".format(e))
		waitingForThread = False
		delsysIsRecording = False



def saveEmgData(data):
	print("	> Saving EMG data")
	with open(currentRecording + "EMG.txt", "w") as file:
		for line in data:
			file.write(data)
			file.write("\n")


def recordQuest():
	global currentRecording, waitingForThread, questIsRecording
	
	f = open("startRecording.txt", "w")
	f.write(currentRecording)
	f.close()

	command = "adb push startRecording.txt sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data"
	runBashCommandWithDisplay(command)
	os.remove("startRecording.txt")

	questIsRecording = True

def recordDelsys():
	global waitingForThread, delsysIsRecording
	waitingForThread = True
	t = threading.Thread(target=emgDataCollection)
	t.start()

	while waitingForThread:
		pass

def askRecordName():
	global currentRecording
	currentRecording =  input("	> Enter record name: ")
	if not isValidFileName(currentRecording):
		print("	> Invalid Name")
		return

def startRecording():
	askRecordName()
	recordQuest()
	recordDelsys()

def startRecordingQuest():
	askRecordName()
	recordQuest()
	
def startRecordingDelsys():
	askRecordName()
	recordDelsys()


def stopRecording():
	global currentRecording, delsysIsRecording, questIsRecording
	if questIsRecording:
		print("	> Sending start recording signal to oculus Quest")
		command = "adb shell touch sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data/stopRecording.txt"
		runBashCommandWithDisplay(command)

		print("	> Starting download of the recording : {}".format(currentRecording))
		command = "adb pull sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data/{}.txt {}".format(currentRecording, os.getcwd().replace("\\", "/") + "/")

		runBashCommandWithDisplay(command)

	questIsRecording = False
	delsysIsRecording = False


def exitProg():
	global exit
	exit = True
	print("	> Exiting the program")


EMGchannelCount = 12
exit = False
commands = {"start": startRecording, "stop": stopRecording, "startQuest":startRecordingQuest, "startDelsys":startRecordingDelsys, "exit": exitProg}
currentRecording = ""
waitingForThread = False
questIsRecording = False
delsysIsRecording = False
host = None

if __name__ == "__main__":
	parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawTextHelpFormatter)
	parser.add_argument('-a', '--addr',
						dest='host',
						default='localhost',
						help="IP address of the machine running TCU. Default is localhost.")
	args = parser.parse_args()
	host = args.host

	print("\nConnecting to {} as machine running TCU\n".format(host))
	while not exit:
		print("------------------------------")
		print("Interface for the EMG-Hand Tracking synchronization system")
		print("Commands:")
		print("start: start recording")
		print("stop: stop recording")
		print("startQuest: start recording of quest only")
		print("startDelsys: start recording of delsys only")
		print("exit: exit the program")
		print("------------------------------")

		inputCommand = input("Enter command: ")
		if inputCommand in commands:
			commands[inputCommand]()
		else:
			print("	> Wrong command")
		print("")
