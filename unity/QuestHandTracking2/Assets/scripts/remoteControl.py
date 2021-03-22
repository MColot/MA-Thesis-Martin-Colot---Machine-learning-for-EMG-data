import subprocess
import re
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


def isValidFileName(s):
	return 1



def startRecording():
	recordName =  input("	> Enter record name: ")
	if not isValidFileName(recordName):
		print("	> Invalid Name")
		return
	
	f = open("startRecording.txt", "w")
	f.write(recordName)
	f.close()

	command = "adb push startRecording.txt sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data"
	print("	> sending start recording signal to oculus Quest")
	runBashCommandWithDisplay(command)

	os.remove("startRecording.txt")



def stopRecording():
	command = "adb shell touch sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data/stopRecording.txt"
	print("	> sending start recording signal to oculus Quest")
	runBashCommandWithDisplay(command)

def exitProg():
	global exit
	exit = True
	print("	> Exiting the program")


exit = False
commands = {"start": startRecording, "stop": stopRecording, "exit": exitProg}

if __name__ == "__main__":
	while not exit:
		print("------------------------------")
		print("Interface for the EMG-Hand Tracking synchronization system")
		print("Commands:")
		print("start: start recording")
		print("stop: stop recording")
		print("exit: exit the program")
		print("------------------------------")

		inputCommand = input("Enter command: ")
		if inputCommand in commands:
			commands[inputCommand]()
		else:
			print("	> Wrong command")
		print("")
