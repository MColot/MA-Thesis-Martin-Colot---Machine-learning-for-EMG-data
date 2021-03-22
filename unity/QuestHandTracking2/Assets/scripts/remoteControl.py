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
	match = re.search("([a-z]*[A-Z]*[0-9]*)*", s)
	return match.group(0) == s



def startRecording():
	global currentRecording
	recordName =  input("	> Enter record name: ")
	if not isValidFileName(recordName):
		print("	> Invalid Name")
		return
	
	f = open("startRecording.txt", "w")
	f.write(recordName)
	f.close()

	currentRecording = recordName

	print("	> sending start recording signal to oculus Quest")
	command = "adb push startRecording.txt sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data"
	runBashCommandWithDisplay(command)

	#here : send trigger to delsys

	os.remove("startRecording.txt")



def stopRecording():
	global currentRecording
	print("	> sending start recording signal to oculus Quest")
	command = "adb shell touch sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data/stopRecording.txt"
	runBashCommandWithDisplay(command)


	print("	> starting download of the recording : {}".format(currentRecording))
	command = "adb pull sdcard/Android/data/com.DefaultCompany.QuestHandTracking2/files/data/{}.txt {}".format(currentRecording, os.getcwd().replace("\\", "/") + "/")
	print(command)

	runBashCommandWithDisplay(command)

	#here : send trigger to delsys


def exitProg():
	global exit
	exit = True
	print("	> Exiting the program")


exit = False
commands = {"start": startRecording, "stop": stopRecording, "exit": exitProg}
currentRecording = ""

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
