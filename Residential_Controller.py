# ***********************************************************************************************************
# *** PROGRAM:      CODEBOXX ODYSSEY                                                                       ***               
# *** WEEK 2:       THE MECHANICS OF INTERPRETED LANGUAGES                                                ***
# *** DELIVERABLE:  RESIDENTIAL CONTROLLER - PYTHON                                                       ***
# *** BY:           JORGE CHAVARRIAGA                                                                     ***
# *** DATE:         21-FEB-2020                                                                           ***
# ***********************************************************************************************************

import os                                                               # Import os library
cls = lambda: os.system("cls")                                          # Function for clear the console screen 
pause = lambda: os.system("pause")                                      # Function for pause between scenarios

# Create a Class Column with Number of Floors and Number of Elevators
class Column:
    def __init__(self, numberOfFloorToServe, numberOfElevatorInBuilding):
        self.numberOfFloorToServe = numberOfFloorToServe                # Number of Floors 
        self.numberOfElevatorInBuilding = numberOfElevatorInBuilding    # Number of Elevators
        self.elevatorArray = []                                         # Array to manage number of Elevators
        self.externalButtonArray = []                                   # Array to manage External Buttons
        for i in range(numberOfElevatorInBuilding):
            self.elevatorArray.append(Elevator(i + 1, 1, numberOfFloorToServe))
        self.setUpExternalButtonArray()
    
    # Method for External Button List (Floor 1 @ Floor 9 UP - Floor 10 @ 2 DOWN)
    def setUpExternalButtonArray(self):
        for i in range(1, self.numberOfFloorToServe):
            self.externalButtonArray.append(ExternalButton(i, "UP"))
        for i in range(2, (self.numberOfFloorToServe + 1)):
            self.externalButtonArray.append(ExternalButton(i, "DOWN"))
 
    # Method for Elevator By Id 
    def findElevatorId(self, id):
        for elevator in self.elevatorArray:
            if elevator.id == id:
                print("        Elevator Selected:")
                print(f"        Elevator # {elevator.id} at Floor: {elevator.currentFloor}")
                return elevator
    
    # Method for Find Best Elevator with Requested Floor and Direction
    def findBestElevatorOption(self, requestedFloor, direction):
        idleStatus = False
        bestGap = self.numberOfFloorToServe
        elevatorBestGap = 0
        for elevator in self.elevatorArray:
            if elevator.status == "IDLE":
                idleStatus = True
            elif (elevator.direction == "UP") and (direction == "UP") and (requestedFloor > elevator.currentFloor):
                elevator.sameDirection = True
            elif (elevator.direction == "DOWN") and (direction == "DOWN") and (requestedFloor < elevator.currentFloor):
                elevator.sameDirection = True
            else:
                elevator.sameDirection = False
        if idleStatus == True:
            for elevator in self.elevatorArray:
                if elevator.status == "IDLE":
                    gap = abs(elevator.currentFloor - requestedFloor)
                    if gap < bestGap:
                        elevatorBestGap = elevator.id
                        bestGap = gap
            return self.findElevatorId(elevatorBestGap)
        if idleStatus == False:
            for elevator in self.elevatorArray:
                if elevator.sameDirection == True:
                    gap = abs(elevator.currentFloor - requestedFloor)
                    if gap < bestGap:
                        elevatorBestGap = elevator.id
                        bestGap = gap
            return self.findElevatorId(elevatorBestGap)                  
        
    # Method for Request Elevator with Requested Floor and Direction
    def requestElevator(self, requestedFloor, direction):
        print("")
        print(f"        User in Floor: {requestedFloor} and going {direction}")
        elevator = self.findBestElevatorOption(requestedFloor, direction)
        elevator.addToQueue(requestedFloor)
        elevator.moveElevator()

    # // Method for Request Floor with Elevator and Requested Floor
    def requestFloor(self, elevator, requestedFloor):
        print(f"        User request Floor: {requestedFloor} with Selected Elevator #: {elevator.id} at Floor: {elevator.currentFloor}")           
        elevator.addToQueue(requestedFloor)
        elevator.closeDoor()
        elevator.moveElevator()
        
# Create a Class for the External Button with Request Floor and Direction
class ExternalButton:
    def __init__(self, requestFloor, direction):
        self.requestFloor = requestFloor                                            # User going to Floor Number
        self.direction = direction                                                  # Push button for Going UP/DOWN
        
# Create a Class Elevator with Elevator Id, Current Floor and Number of Floors
class Elevator:
    def __init__(self, id, currentFloor, numberOfFloorToServe):
        self.id = id                                                                # Elevator[0] Id1, Elevator[1] Id2  
        self.currentFloor = currentFloor                                            # Elevator current Floor
        self.numberOfFloorToServe = numberOfFloorToServe                            # Number of Floors
        self.internalButtonArray = []                                                # Destination Floor:  [0] Floor1, [1] Floor2, ... [9] Floor10
        self.queue = []                                                             # Elevator Queue
        self.door = "CLOSED"                                                        # Door status: CLOSED/OPENED
        self.status = "IDLE"                                                        # Elevator Status: IDLE/MOVING
        self.samedirection = False                                                  # Elevator Same direction of service                
        self.direction = ""                                                         # Elevator Direction: UP/DOWN/null(not moving)
        for i in range (1, self.numberOfFloorToServe + 1):                          
            self.internalButtonArray.append(InternalButton(i))        
        
    # Method for adding request to the Queue
    def addToQueue(self, requestedFloor):
        print(f"        Add Floor: {requestedFloor} to Queue {self.queue}")
        self.queue.append(requestedFloor)
        print(f"        Floor: {requestedFloor} Added to Queue {self.queue}")                            
        if self.direction == "UP":
            self.queue.sort()  
            print(f"       Floor: {requestedFloor} Sorted Ascending {self.queue}")  # Without parameter sort Ascending
        if self.direction == "DOWN":
            self.queue.sort(reverse=True)                                           # Parameter (reverse=True) for sort descending
            print(f"       Floor: {requestedFloor} Sorted Descending {self.queue}") # Without parameter sort Descending

    # Method for moving the Elevator
    def moveElevator(self):
        print("        Move Elevator")
        while self.queue:
            if self.door == "OPENED":
                self.closeDoor()
            firstReq = self.queue[0]                                            # Remove first service from the queue
            if firstReq == self.currentFloor:
                print(f"        Arrive at Floor: {firstReq}")
                print(f"        Remove request from Queue: [{self.queue[0]}]")
                del self.queue[0]                                               
                self.openDoor()
            if firstReq > self.currentFloor:
                self.status = "MOVING"
                self.direction = "UP"
                self.moveUp()
            if firstReq < self.currentFloor:
                self.status = "MOVING"
                self.direction = "DOWN"
                self.moveDown()       
        if self.queue:
            pass
        else:
            self.status = "IDLE"
        print(f"        Elevator Status: {self.status}")
        
    # Method for moving Up the Elevator
    def moveUp(self):
        self.currentFloor = (self.currentFloor + 1)
        print(f"        Going UP - Floor: {self.currentFloor}")         # Actual Floor + 1 (Going Up increasing)
        
    # Method for moving Down the Elevator
    def moveDown(self):
        self.currentFloor = (self.currentFloor - 1)
        print(f"        Going DOWN - Floor: {self.currentFloor}")       # Actual Floor - 1 (Going down decreasing)
        
    # Method for status Door Opened
    def openDoor(self):
        self.door = "Door Opened"
        print("        Open Door")
    
    # Method for status Door Closed
    def closeDoor(self):
        self.door = "Door Closed"
        print("        Close Door")
        
# Create a Class for Internal Button with Requested Floor
class InternalButton:
    def __init__(self, floor):
        self.floor = floor                                                          # Destination floor
     
myColumn = Column(10,2)
 
 ############################################ TESTING ######################################################
 
 # ****** SCENARIO 1 ******
def scenario1Method1RequestElevator():
    cls()
    print(" ****** SCENARIO 1")
    print(" ****** Elevator 1: Idle @ Floor 2 - Elevator 2: Idle @ Floor 6  -  User on Floor: 3 and Going UP to Floor: 7")
    print("")
    print(" ****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION")
    myColumn.elevatorArray[0].currentFloor = 2
    myColumn.elevatorArray[0].direction = ""
    myColumn.elevatorArray[0].status = "IDLE"
    myColumn.elevatorArray[1].currentFloor = 6
    myColumn.elevatorArray[1].direction = ""
    myColumn.elevatorArray[1].status = "IDLE"
    myColumn.requestElevator(3,"UP")
scenario1Method1RequestElevator()

def scenario1Method2RequestFloor():
    print("")
    print(" ****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR" )
    print(" ****** User requested Floor: 7")
    print("")
    elevator = myColumn.elevatorArray[0]
    myColumn.requestFloor(elevator, 7)
scenario1Method2RequestFloor()

pause()
cls()
   
# ****** SCENARIO 2 ******
def scenario2Method1RequestElevator():
    cls()
    print(" ****** SCENARIO 2")
    print(" ****** Elevator 1: Idle @ Floor 10 - Elevator 2: Idle @ Floor 3  -  User on Floor: 1 and Going UP to Floor: 6")
    print("")
    print(" ****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION")
    myColumn.elevatorArray[0].currentFloor = 10
    myColumn.elevatorArray[0].direction = ""
    myColumn.elevatorArray[0].status = "IDLE"
    myColumn.elevatorArray[1].currentFloor = 3
    myColumn.elevatorArray[1].direction = ""
    myColumn.elevatorArray[1].status = "IDLE"
    myColumn.requestElevator(1,"UP")
scenario2Method1RequestElevator()

def scenario2Method2RequestFloor():
    print("")
    print(" ****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR" )
    print(" ****** User requested Floor: 6")
    print("")
    elevator = myColumn.elevatorArray[1]
    myColumn.requestFloor(elevator, 6)
scenario2Method2RequestFloor()

pause()
cls()
   
def scenario2aMethod1RequestElevator():
    cls()
    print(" ****** 2 MINUTES LATER")
    print(" ****** Elevator 1: Idle @ Floor 10 - Elevator 2: Idle @ Floor 6  -  User on Floor: 3 and Going UP to Floor: 5")
    print("")
    print(" ****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION")
    myColumn.elevatorArray[0].currentFloor = 10
    myColumn.elevatorArray[0].direction = ""
    myColumn.elevatorArray[0].status = "IDLE"
    myColumn.elevatorArray[1].currentFloor = 6
    myColumn.elevatorArray[1].direction = ""
    myColumn.elevatorArray[1].status = "IDLE"
    myColumn.requestElevator(3,"UP")
scenario2aMethod1RequestElevator()

def scenario2aMethod2RequestFloor():
    print("")
    print(" ****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR" )
    print(" ****** User requested Floor: 5")
    print("")
    elevator = myColumn.elevatorArray[1]
    myColumn.requestFloor(elevator, 5)
scenario2aMethod2RequestFloor()

pause()
cls()
   
def scenario2bMethod1RequestElevator():
    cls()
    print(" ****** FINALLY")
    print(" ****** Elevator 1: Idle @ Floor 10 - Elevator 2: Idle @ Floor 5  -  User on Floor: 9 and Going DOWN to Floor: 2")
    print("")
    print(" ****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION")
    myColumn.elevatorArray[0].currentFloor = 10
    myColumn.elevatorArray[0].direction = ""
    myColumn.elevatorArray[0].status = "IDLE"
    myColumn.elevatorArray[1].currentFloor = 5
    myColumn.elevatorArray[1].direction = ""
    myColumn.elevatorArray[1].status = "IDLE"
    myColumn.requestElevator(9,"DOWN")
scenario2bMethod1RequestElevator()

def scenario2bMethod2RequestFloor():
    print("")
    print(" ****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR" )
    print(" ****** User requested Floor: 2")
    print("")
    elevator = myColumn.elevatorArray[0]
    myColumn.requestFloor(elevator, 2)
scenario2bMethod2RequestFloor()
pause()
cls()

# ****** SCENARIO 3 ******   
def scenario3Method1RequestElevator():
    cls()
    print(" ****** SCENARIO 3")
    print(" ****** Elevator 1: Idle @ Floor 10 - Elevator 2: Moving from Floor 3 @ Floor 6  -  User on Floor: 3 and Going DOWN to Floor: 2")
    print("")
    print(" ****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION")
    myColumn.elevatorArray[0].currentFloor = 10
    myColumn.elevatorArray[0].direction = ""
    myColumn.elevatorArray[0].status = "IDLE"
    myColumn.elevatorArray[1].currentFloor = 3
    myColumn.elevatorArray[1].direction = "UP"
    myColumn.elevatorArray[1].status = "MOVING"
    myColumn.requestElevator(3,"DOWN")
scenario3Method1RequestElevator()

def scenario3Method2RequestFloor():
    print("")
    print(" ****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR" )
    print(" ****** User requested Floor: 6")
    print("")
    elevator = myColumn.elevatorArray[0]
    myColumn.requestFloor(elevator, 2)
scenario3Method2RequestFloor()

pause()
cls()
   
def scenario3aMethod1RequestElevator():
    cls()
    print(" ****** 5 MINUTES LATER")
    print(" ****** Elevator 1: Idle @ Floor 2 - Elevator 2: Idle @ Floor 6  -  User on Floor: 10 and Going DOWN to Floor: 3")
    print("")
    print(" ****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION")
    myColumn.elevatorArray[0].currentFloor = 2
    myColumn.elevatorArray[0].direction = ""
    myColumn.elevatorArray[0].status = "IDLE"
    myColumn.elevatorArray[1].currentFloor = 6
    myColumn.elevatorArray[1].direction = ""
    myColumn.elevatorArray[1].status = "IDLE"
    myColumn.requestElevator(10,"DOWN")
scenario3aMethod1RequestElevator()

def scenario3aMethod2RequestFloor():
    print("")
    print(" ****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR" )
    print(" ****** User requested Floor: 3")
    print("")
    elevator = myColumn.elevatorArray[1]
    myColumn.requestFloor(elevator, 3)
scenario3aMethod2RequestFloor()

pause()
cls()
