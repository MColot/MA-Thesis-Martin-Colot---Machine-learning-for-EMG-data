import subprocess
import re
import os
import threading
import argparse
import pytrigno
from datetime import datetime

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
	global host, delsysIsRecording, EMGchannelCount, waitingForDelsysThread
	print("	> Sending recording signal to delsys")
	try:
		delsysRecorder = pytrigno.TrignoEMG(channel_range=(0, EMGchannelCount - 1), samples_per_read=270, host=host)
		data = []
		delsysRecorder.start()
		delsysIsRecording = True
		print("	> Sucess")
		while delsysIsRecording:
			data.append(delsysRecorder.read())
			waitingForDelsysThread = False #TODO : make sure that delsysRecorder.read() is blocking
			print("	> Receiving data from delsys")
		delsysRecorder.stop()
		saveEmgData(data)
	except Exception as e:
		print("	> Failure : {}".format(e))
		waitingForDelsysThread = False
		delsysIsRecording = False



def saveEmgData(data):
	print("	> Saving EMG data")
	with open(currentRecording + "EMG.txt", "w") as file:
		for line in data:
			file.write(line)
			file.write("\n")


def recordQuest():
	global currentRecording, waitingForDelsysThread, questIsRecording, triggerTimestamps
	triggerTimestamps = []

	print("	> Sending recording signal to Quest")
	
	f = open("startRecording.txt", "w")
	f.write(currentRecording)
	f.close()

	command = "adb push startRecording.txt sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data"
	runBashCommandWithDisplay(command)
	os.remove("startRecording.txt")

	questIsRecording = True

def recordDelsys():
	global waitingForDelsysThread, delsysIsRecording
	waitingForDelsysThread = True
	t = threading.Thread(target=emgDataCollection)
	t.start()

	while waitingForDelsysThread:
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
	global currentRecording, delsysIsRecording, questIsRecording, triggerTimestamps
	if not (questIsRecording or delsysIsRecording):
		print("	> nothing is recording")
		return
	if questIsRecording:
		print("	> Sending start recording signal to oculus Quest")
		command = "adb shell touch sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data/stopRecording.txt"
		runBashCommandWithDisplay(command)

		print("	> Starting download of the recording : {}".format(currentRecording))
		command = "adb pull sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data/{}.txt {}".format(currentRecording, os.getcwd().replace("\\", "/") + "/")

		runBashCommandWithDisplay(command)
	if delsysIsRecording:
		sendNewTrigger = True
		while sendNewTrigger and len(triggerTimestamps) < 2:
			sendNewTrigger = input(f"	> You are recording EMG and have only sent {len(triggerTimestamps)} trigger. \n		Do you want to send a new one? (y or n) : ") == "y"
			if sendNewTrigger:
				trigger()

	questIsRecording = False
	delsysIsRecording = False
	print(f" > trigger were sent at {triggerTimestamps}")


def trigger():
	global delsysIsRecording, triggerTimestamps
	if delsysIsRecording:
		pass
		#TODO: send trigger signal to delsys : http://data.delsys.com/DelsysServicePortal/api/web-api/DelsysAPI.Utils.TrignoTrigger.html
	triggerTimestamps.append(datetime.now())
	print(f"	> trigger has been sent at timestamp {triggerTimestamps[-1]}")

def exitProg():
	global exit
	exit = True
	print("	> Exiting the program")


EMGchannelCount = 16
exit = False
commands = {"start": startRecording, "stop": stopRecording, "startQuest":startRecordingQuest, "startDelsys":startRecordingDelsys, "trigger": trigger, "exit": exitProg}
currentRecording = ""
waitingForDelsysThread = False
questIsRecording = False
delsysIsRecording = False
host = None
triggerTimestamps = []

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
		print("trigger: send synchronization trigger")
		print("exit: exit the program")
		print("------------------------------")

		inputCommand = input("Enter command: ")
		if inputCommand in commands:
			commands[inputCommand]()
		else:
			print("	> Wrong command")
		print("")
