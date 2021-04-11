import serial

EVENTDURATION = 100# 0.005   # In sec
EVENTTOSEND = 255

# Opening serial port for USB2TTL8 device
try:
	serialPort = serial.Serial('COM7', baudrate=128000, timeout=0.01)
	print("Serial port opened - USB2TTL8 device LED should be green")
except:
	print("No serial port opened - USB2TTL8 device not found!")
	serialPort = None

if serialPort != None:
	serialPort.write(str.encode(f"WRITE {0}\n"))

input()

if serialPort != None:
	serialPort.write(str.encode(f"WRITE {EVENTTOSEND}\n"))


input()

if serialPort != None:
	serialPort.write(str.encode(f"WRITE {0}\n"))

input()

if serialPort != None:
	serialPort.write(str.encode(f"WRITE {EVENTTOSEND}\n"))

input()
# Closing serial port (USB2TTL8 device) if previously opened

if serialPort != None:
	serialPort.write(str.encode(f"WRITE {0}\n"))

try:
	serialPort.close()
except:
	pass
