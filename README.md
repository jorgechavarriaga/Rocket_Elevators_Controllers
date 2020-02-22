***
PROGRAM:			CODEBOXX ODSSEY

WEEK 2:				THE MECHANICS OF INTERPRETED LANGUAGES 

DELIVERABLE:  RESIDENTIAL CONTROLLER - JAVASCRIPT AND PYTHON  

BY:           JORGE CHAVARRIAGA     

DATE:         21-FEB-2020    

***

Contains the following files:

* Residential_Controller.html
* logo.png
* Residential_Controller.js
* Residential_Controller.py
* Residential_Controller.gif

The Residentail_Controller.html is used to summarize the deliverable's requirements and to call the javascript file.

Once the html file is opened, click on one of the buttons with the scenarios (1 to 3) or (custom scenario - Make your own 
scenario) and the inspector console opens to see the program running.

If you prefer, you can also build the scenarios from the console by following the steps below:

1. Create a new column object with the parameters, number of floors and number of elevators:

* let columnTest1 = new Column(10,2)

2. Configure for each elevator [i]: Current position elevator (current floor), address (direction UP / DOWN / IDLE) and its 
   status (status IDLE / MOVING):
* columnTest1.elevatorArray[i].currentFloor = (enter a valid current floor);
* columnTest1.elevatorArray[i].direction = (enter a valid direction);
* columnTest1.elevatorArray[i].status = `(enter a valid status);

3. Run the function to request the elevator, with userRequestedFloor, userRequestedDirection (UP/DOWN):
* columnTest1.requestElevator(userRequestedFloor, userRequestedDirection);

Once the function is executed, the first method will be executed (Method 1 must return the chosen elevator and move the
elevators in its treatment).

The program will return the following information: On which floor is the user and which direction want to go; The elevator
number selected (closest) and on which floor it is located; the requested floor number is entered to an array, move the
elevator to the floor of the call, inform that the floor arrived, remove the call from the array, open the door 
and remain in an available state waiting for the user to enter and select the desired floor.

4. Run the function to send the user to the desired floor, with elevator, userRequesFloor:
* columnTest1.requestFloor(columnTest1.elevatorArray[elevator*], userRequesFloor);

	(*Keep in mind that the elevator number has to be the same one obtained in method 1, and as it is an array, 
	index 0 elevator 1, index 1 elevator 2)
	
Once this function is executed, the second method will be executed (Method 2 must move the elevators in its treatment).

With the following results for method 2: Floor requested by the user and which elevator will attend. Enter the desired 
floor to an array, in case of going up, arrange the array ascendingly (if you is going down, order it descendingly), 
close the door and start going up showing each floor, then show that you reach the desired floor, remove the floor from
the array, open the door and is available.

The python program works exactly the same. A new column is created, the initial setup of the elevators is done and
the functions are executed to request the elevator and subsequently to request the desired floor.

Note that Python parameters are entered directly into the program (not the console). Then the program is executed 
(Visual Code: right click Run Python File in Terminal).
