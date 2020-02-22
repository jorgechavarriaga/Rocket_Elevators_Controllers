/************************************************************************************************************
*** PROGRAM:      CODEBOXX ODYSSEY                                                                         ***
*** WEEK 2:       THE MECHANICS OF INTERPRETED LANGUAGES                                                  ***
*** DELIVERABLE:  RESIDENTIAL CONTROLLER - JAVASCRIPT                                                     ***
*** BY:           JORGE CHAVARRIAGA                                                                       ***
*** DATE:         21-FEB-2020                                                                             ***
*************************************************************************************************************/

// Create a Class Column with Number of Floors and Number of Elevators
class Column{                                                
    constructor(numberOfFloorToServe, numberOfElevatorInBuilding){
        this.numberOfFloorToServe = numberOfFloorToServe;               // Number of Floors             
        this.numberOfElevatorInBuilding = numberOfElevatorInBuilding;   // Number of Elevators
        this.elevatorArray = [];                                        // Array to manage number of Elevators
        this.externalButtonArray = [];                                  // Array to manage External Buttons 
        for (let i = 0; i < this.numberOfElevatorInBuilding; i++){
			this.elevatorArray.push(new Elevator(i + 1, 1, numberOfFloorToServe));
		}
        this.setUpExternalButtonArray();
    }

    // Method for External Button List (Floor 1 @ Floor 9 UP - Floor 10 @ 2 DOWN)
    setUpExternalButtonArray(){                          
        for (let i = 1; i < this.numberOfFloorToServe; i++){
            this.externalButtonArray.push(new ExternalButton(i, `UP`));
        }
        for (let i = 2; i < this.numberOfFloorToServe + 1; i++){
            this.externalButtonArray.push(new ExternalButton(i, `DOWN`));
        }
    }

    // Method for Elevator By Id 
    findElevatorId(id){  
        for (let i = 0; i < this.elevatorArray.length; i++){
            if (this.elevatorArray[i].id == id) {
                console.log(`       Elevator Selected:`);
                console.log(`       Elevator # ${this.elevatorArray[i].id} at Floor: ${this.elevatorArray[i].currentFloor}`);
                return this.elevatorArray[i];
            }
        }                               
    }

    // Method for Find Best Elevator with Requested Floor and Direction
    findBestElevatorOption(requestedFloor, direction){        
        let idleStatus = false;
        let bestGap = this.numberOfFloorToServe;
        let elevatorBestGap = 0;                   
        for (let i = 0; i < this.elevatorArray.length; i++) {
            if (this.elevatorArray[i].status == `IDLE`) {
                idleStatus = true;
            } else if ((this.elevatorArray[i].direction == `UP`) && (direction == `UP`) && (requestedFloor > this.elevatorArray[i].currentFloor)) {
                this.elevatorArray[i].sameDirecton = true
            } else if ((this.elevatorArray[i].direction == `DOWN`) && (direction == `DOWN`) && (requestedFloor < this.elevatorArray[i].currentFloor)) {
                this.elevatorArray[i].sameDirecton = true
            } else {
                this.elevatorArray[i].sameDirecton = false
            }
        }
        if (idleStatus == true) {
            for (let i = 0; i < this.elevatorArray.length; i++) {
                if (this.elevatorArray[i].status == `IDLE`) {
                    let gap = Math.abs(this.elevatorArray[i].currentFloor - requestedFloor);
                    if (gap < bestGap) {
                        elevatorBestGap = this.elevatorArray[i].id;
                        bestGap = gap;
                    }
                }
            }
            return this.findElevatorId(elevatorBestGap)
        }
        if (idleStatus == false) {
            for (let i = 0; i < this.elevatorArray.length; i++) {
                if (this.elevatorArray[i].sameDirecton == true) {
                    let gap = Math.abs(this.elevatorArray[i].currentFloor - requestedFloor);
                    if (gap < bestGap) {
                        elevatorBestGap = this.elevatorArray[i].id;
                        bestGap = gap;
                    }
                }
            }
            return this.findElevatorId(elevatorBestGap)
        }
    }

    // Method for Request Elevator with Requested Floor and Direction
    requestElevator(requestedFloor, direction){         
        console.log(`       User in Floor: ${requestedFloor} and going ${direction}`);        
        let elevator = this.findBestElevatorOption(requestedFloor, direction);
		elevator.addToQueue(requestedFloor);
        elevator.moveElevator();
		return elevator;
    }

    // Method for Request Floor with Elevator and Requested Floor
    requestFloor(elevator, requestedFloor){  
        console.log(`       User request Floor: ${requestedFloor} with Selected Elevator #: ${elevator.id} at Floor: ${elevator.currentFloor}`);           
        elevator.addToQueue(requestedFloor);
		elevator.closeDoor();
		elevator.moveElevator();
    }
}

// Create a Class for the External Button with Request Floor and Direction
class ExternalButton{                                   
    constructor(requestFloor, direction){
        this.requestFloor = requestFloor;                               // User going to Floor Number
		this.direction = direction;                                     // Push button for Going UP/DOWN
    }
}

// Create a Class Elevator with Elevator Id, Current Floor and Number of Floors
class Elevator{                                         
    constructor(id, currentFloor, numberOfFloorToServe){
        this.id = id; 								                    // Elevator[0] Id1, Elevator[1] Id2
        this.currentFloor = currentFloor; 			                    // Elevator current Floor
		this.numberOfFloorToServe = numberOfFloorToServe;               // Number of Floors
        this.internalButtonArray = [];                                  // Destination Floor:  [0] Floor1, [1] Floor2, ... [9] Floor10 
        this.queue = [];                                                // Elevator Queue
		this.door = `CLOSED`;                                           // Door status: CLOSED/OPENED
        this.status = `IDLE`; 						                    // Elevator Status: IDLE/MOVING
        this.sameDirecton = false;                                      // Elevator Same direction of service
        this.direction = ``; 						                    // Elevator Direction: UP/DOWN/null(not moving)
        for (let i = 1; i < this.numberOfFloorToServe + 1; i++) {
			this.internalButtonArray.push(new InternalButton(i));
		}
    }
    // Method for adding request to the Queue
    addToQueue(requestedFloor){  
        console.log(`       Add Floor: ${requestedFloor} to Queue [${this.queue.toString()}]`);                                                                     
        this.queue.push(requestedFloor)
        console.log(`       Floor: ${requestedFloor} Added to Queue [${this.queue.toString()}]`);                                                                 
		if (this.direction == `UP`) {
			this.queue.sort((a, b) => a - b);                           // Sort for going up Ascending
            console.log(`       Floor: ${requestedFloor} Sorted Ascending [${this.queue.toString()}]`);
        }
		if (this.direction == `DOWN`) {
			this.queue.sort((a, b) => b - a);                           // Sort for going down Descending
            console.log(`       Floor: ${requestedFloor} Sorted Descending [${this.queue.toString()}]`);
        }
    }

    // Method for moving the Elevator
    moveElevator(){                                 
		while (this.queue.length > 0){
			if (this.door == `OPENED`){
				this.closeDoor();
			}
			let firstReq = this.queue[0];
			if (firstReq == this.currentFloor){
				console.log(`       Arrive at Floor: ${firstReq}`);
                console.log(`       Remove request from Queue: [${this.queue.shift()}]`);
				this.queue.shift();                                     // Remove first service from the queue
                this.openDoor();
			}
			if (firstReq > this.currentFloor){
				this.status = `MOVING`;
				this.direction = `UP`;
				this.moveUp();
            }
            
			if (firstReq < this.currentFloor){
				this.status = `MOVING`;
				this.direction = `DOWN`;
				this.moveDown();
			}
		}
		if (this.queue.length > 0){
        } 
        else {
            this.status = `IDLE`;
            console.log(`       Elevator Status: ${this.status}`);
        }
    }

    // Method for moving Up the Elevator
    moveUp(){
        this.currentFloor = (this.currentFloor + 1);                    // Actual Floor + 1 (Going Up increasing)
        console.log(`       Going UP - Floor: ${this.currentFloor}`);
    }

    // Method for moving Down the Elevator
    moveDown(){                                 
        this.currentFloor = (this.currentFloor - 1);                    // Actual Floor - 1 (Going down decreasing)
        console.log(`       Going DOWN - Floor: ${this.currentFloor}`);
    }

    // Method for status Door Opened
    openDoor(){                                         
        this.door = `Door Opened`;
        console.log(`       Open Door`);
        if (this.queue.length > 0){
            this.closeDoor();
        }         
    }

    // Method for status Door Closed
    closeDoor(){                                        
        this.door = `Door Closed`;
        console.log(`       Close Door`);

    }
}

// Create a Class for Internal Button with Requested Floor
class InternalButton{                                   
    constructor(floor) {
		this.floor = floor;                                             // Destination floor
	}
}

function clearConsole(){
    console.clear();
}

/***********************************     TESTING     *************************************************/

var numberoOfFloorForTest = 10;
var numberOfElevatorForTest = 2;
let myColumn = new Column(numberoOfFloorForTest, numberOfElevatorForTest);


// ****** SCENARIO 1 ******
function scenario1Method1RequestElevator(){ 
    clearConsole();
    console.log(`****** SCENARIO 1`);
    console.log(`****** Elevator 1: Idle @ Floor 2 - Elevator 2: Idle @ Floor 6  -  User on Floor: 3 and Going UP to Floor: 7`);console.log(``);
    console.log(`****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION`); console.log(``);
    for (let i = 0; i < myColumn.numberOfElevatorInBuilding; i++){
        if (i === 0){
            myColumn.elevatorArray[i].currentFloor = 2;
            myColumn.elevatorArray[i].direction = ``;
            myColumn.elevatorArray[i].status = `IDLE`;
        } else {
            myColumn.elevatorArray[i].currentFloor = 6;
            myColumn.elevatorArray[i].direction = ``;
            myColumn.elevatorArray[i].status = `IDLE`;
        }
    }
    var userRequestedFloor = 3;
    var userRequestedDirection = `UP`;
    myColumn.requestElevator(userRequestedFloor, userRequestedDirection);
}

function scenario1Method2RequestFloor(){
    console.log(``);
    console.log(`****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR`);
    console.log(`****** User requested Floor: 7`);console.log(``);
    var elevatorSelected = 0;
    elevator = myColumn.elevatorArray[elevatorSelected];
    var userRequesFloor = 7;
    myColumn.requestFloor(elevator, userRequesFloor);
}

function buttonScenario1(){
    scenario1Method1RequestElevator();
    scenario1Method2RequestFloor();
}

// ****** SCENARIO 2 ******
function scenario2Method1RequestElevator() { 
    clearConsole();
    console.log(`****** SCENARIO 2`);
    console.log(`****** Elevator 1: Idle @ Floor 10  -  Elevator 2: Idle @ Floor 3  -  User on Floor: 1 and Going UP to Floor: 6`); console.log(``);
    console.log(`****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION`);console.log(``);
    for (let i = 0; i < myColumn.numberOfElevatorInBuilding; i++){
        if (i === 0){
            myColumn.elevatorArray[i].currentFloor = 10;
            myColumn.elevatorArray[i].direction = ``;
            myColumn.elevatorArray[i].status = `IDLE`;
        } else {
            myColumn.elevatorArray[i].currentFloor = 3;
            myColumn.elevatorArray[i].direction = ``;
            myColumn.elevatorArray[i].status = `IDLE`;
        }
    }
    var userRequestedFloor = 1;
    var userRequestedDirection = `UP`;
    myColumn.requestElevator(userRequestedFloor, userRequestedDirection);
}

function scenario2Method2RequestFloor(){
    console.log(``);
    console.log(`****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR`);
    console.log(`****** User requested Floor: 6`);console.log(``);
    var elevatorSelected = 1;
    elevator = myColumn.elevatorArray[elevatorSelected];
    var userRequesFloor = 6;
    myColumn.requestFloor(elevator, userRequesFloor);
}

function scenario2aMethod1RequestElevator() { 
    console.log(``);console.log(`****** 2 MINUTES LATER ******`);
    console.log(`****** Elevator 1: Idle @ Floor 10  -  Elevator 2: Idle @ Floor 6  -  User on Floor: 3 and Going UP to Floor: 5`);console.log(``);
    console.log(`****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION`);console.log(``);
    for (let i = 0; i < myColumn.numberOfElevatorInBuilding; i++){
        if (i === 0){
            myColumn.elevatorArray[i].currentFloor = 10;
            myColumn.elevatorArray[i].direction = ``;
            myColumn.elevatorArray[i].status = `IDLE`;
        } else {
            myColumn.elevatorArray[i].currentFloor = 6;
            myColumn.elevatorArray[i].direction = ``;
            myColumn.elevatorArray[i].status = `IDLE`;
        }
    }
    var userRequestedFloor = 3;
    var userRequestedDirection = `UP`;
    myColumn.requestElevator(userRequestedFloor, userRequestedDirection);  
}

function scenario2aMethod2RequestFloor(){
    console.log(``);
    console.log(`****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR`);
    console.log(`****** User requested Floor: 5`);console.log(``);
    var elevatorSelected = 1;
    elevator = myColumn.elevatorArray[elevatorSelected];
    var userRequesFloor = 5;
    myColumn.requestFloor(elevator, userRequesFloor);
}

function scenario2bMethod1RequestElevator() { 
    console.log(``);console.log(`****** FINALLY ******`);
    console.log(`****** Elevator 1: Idle @ Floor 10  -  Elevator 2: Idle @ Floor 5  -  User on Floor: 9 and Going DOWN to Floor: 2`);console.log(``);
    console.log(`****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION`);console.log(``);
    for (let i = 0; i < myColumn.numberOfElevatorInBuilding; i++){
        if (i === 0){
            myColumn.elevatorArray[i].currentFloor = 10;
            myColumn.elevatorArray[i].direction = ``;
            myColumn.elevatorArray[i].status = `IDLE`;
        } else {
            myColumn.elevatorArray[i].currentFloor = 5;
            myColumn.elevatorArray[i].direction = ``;
            myColumn.elevatorArray[i].status = `IDLE`;
        }
    }
    var userRequestedFloor = 9;
    var userRequestedDirection = `DOWN`;
    myColumn.requestElevator(userRequestedFloor, userRequestedDirection);   
}

function scenario2bMethod2RequestFloor(){
    console.log(``);
    console.log(`****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR`);
    console.log(`****** User requested Floor: 2`);console.log(``);
    var elevatorSelected = 0;
    elevator = myColumn.elevatorArray[elevatorSelected];
    var userRequesFloor = 2;
    myColumn.requestFloor(elevator, userRequesFloor);
}

function buttonScenario2(){
    scenario2Method1RequestElevator();
    scenario2Method2RequestFloor();
    scenario2aMethod1RequestElevator();
    scenario2aMethod2RequestFloor();
    scenario2bMethod1RequestElevator();
    scenario2bMethod2RequestFloor();
}


// ****** SCENARIO 3 ******
function scenario3Method1RequestElevator() {
    clearConsole(); 
    console.log(`****** SCENARIO 3`);
    console.log(`****** Elevator 1: Idle @ Floor 10 - Elevator 2: Moving from Floor 3 @ Floor 6  -  User on Floor: 3 and Going DOWN to Floor: 2`);console.log(``);
    console.log(`****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION`);console.log(``);
    for (let i = 0; i < myColumn.numberOfElevatorInBuilding; i++){
        if (i === 0){
            myColumn.elevatorArray[i].currentFloor = 10;
            myColumn.elevatorArray[i].direction = ``;
            myColumn.elevatorArray[i].status = `IDLE`;
        } else {
            myColumn.elevatorArray[i].currentFloor = 3;
            myColumn.elevatorArray[i].direction = `UP`;
            myColumn.elevatorArray[i].status = `MOVING`;
        }
    }
    var userRequestedFloor = 3;
    var userRequestedDirection = `DOWN`;
    myColumn.requestElevator(userRequestedFloor, userRequestedDirection);  
}

function scenario3Method2RequestFloor(){
    console.log(``);
    console.log(`****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR`);
    console.log(`****** User requested Floor: 2`);console.log(``);
    var elevatorSelected = 0;
    elevator = myColumn.elevatorArray[elevatorSelected];
    var userRequesFloor = 2;
    myColumn.requestFloor(elevator, userRequesFloor);
}

function scenario3aMethod1RequestElevator() { 
    console.log(``);console.log(`****** 5 MINUTES LATER ******`);
    console.log(`****** Elevator 1: Idle @ Floor 2  -  Elevator 2: Idle @ Floor 6  -  User on Floor: 10 and Going DOWN to Floor: 3`);console.log(``);
    console.log(`****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION`);console.log(``);
    for (let i = 0; i < myColumn.numberOfElevatorInBuilding; i++){
        if (i === 0){
            myColumn.elevatorArray[i].currentFloor = 2;
            myColumn.elevatorArray[i].direction = ``;
            myColumn.elevatorArray[i].status = `IDLE`;
        } else {
            myColumn.elevatorArray[i].currentFloor = 3;
            myColumn.elevatorArray[i].direction = ``;
            myColumn.elevatorArray[i].status = `IDLE`;
        }
    }
    var userRequestedFloor = 10;
    var userRequestedDirection = `DOWN`;
    myColumn.requestElevator(userRequestedFloor, userRequestedDirection);   
}

function scenario3aMethod2RequestFloor(){
    console.log(``);
    console.log(`****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR`);
    console.log(`****** User requested Floor: 3`);console.log(``);
    var elevatorSelected = 1;
    elevator = myColumn.elevatorArray[elevatorSelected];
    var userRequesFloor = 3;
    myColumn.requestFloor(elevator, userRequesFloor);
}

function buttonScenario3(){
    scenario3Method1RequestElevator();
    scenario3Method2RequestFloor();
    scenario3aMethod1RequestElevator();
    scenario3aMethod2RequestFloor();
}


// ****** SCENARIO 4 MAKE YOUR OWN SCENARIO ******
function scenario4Method1RequestElevator(){ 
    clearConsole();
    let userName = prompt(`Welcome to Rocket Elevators Residential Controller Simulator\nEnter your name: `);
    if (userName === ""){
        alert("Invalid Name");
        scenario4Method1RequestElevator();
    }
    let message = alert(`Hi ${userName.toUpperCase()}, let's setup the system`);
    let myCurrentFloor1 = prompt(`Elevator #1 Current Floor: \n(1 - ${numberoOfFloorForTest})`);
    if (myCurrentFloor1 < 1 || myCurrentFloor1 > 10){
        alert("Invalid Floor");
        scenario4Method1RequestElevator();
    }
    let myCurrentDirection1 = prompt(`Elevator #1 Current Direction: \n(UP / DOWN / IDLE)`);
    if (myCurrentDirection1 === "IDLE" || myCurrentDirection1 === "UP" || myCurrentDirection1 === "DOWN"){
    } else {
        alert("Invalid Directon");
        scenario4Method1RequestElevator();
    }
    let myCurrentStatus1 = prompt(`Elevator #1 Current Status: \n(IDLE - MOVING)`);
    if (myCurrentStatus1 === "IDLE" || myCurrentStatus1 === "MOVING"){
    } else {
        alert("Invalid Status");
        scenario4Method1RequestElevator();
    }   
    let myCurrentFloor2= prompt(`Elevator #2 Current Floor: \n(1 - ${numberoOfFloorForTest})`);
    if (myCurrentFloor2 < 1 || myCurrentFloor2 > 10){
        alert("Invalid Floor");
        scenario4Method1RequestElevator();
    }
    let myCurrentDirection2 = prompt(`Elevator #2 Current Direction: \n(UP / DOWN / IDLE)`);
    if (myCurrentDirection2 === "IDLE" || myCurrentDirection2 === "UP" || myCurrentDirection2 === "DOWN"){
    } else {
        alert("Invalid Directon");
        scenario4Method1RequestElevator();
    }
    let myCurrentStatus2 = prompt(`Elevator #2 Current Status: \n(IDLE - MOVING)`);
    if (myCurrentStatus2 === "IDLE" || myCurrentStatus2 === "MOVING"){
    } else {
        alert("Invalid Status");
        scenario4Method1RequestElevator();
    }
    let myRequestedFloor = prompt(`${userName.toUpperCase()} is on Floor: \n(1 - ${numberoOfFloorForTest})`);
    if (myRequestedFloor < 1 || myRequestedFloor > 10){
        alert("Invalid Floor");
        scenario4Method1RequestElevator();
    }
    let myRequestedDirection = prompt(`Direction Requested: \n(UP / DOWN / IDLE)`);
    if (myRequestedDirection === "IDLE" || myRequestedDirection === "UP" || myRequestedDirection === "DOWN"){
    } else {
        alert("Invalid Directon");
        scenario4Method1RequestElevator();
    }
    console.log(`****** YOUR OWN SCENARIO`);
    console.log(`****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION`); console.log(``);  
    for (let i = 0; i < myColumn.numberOfElevatorInBuilding; i++){
        if (i === 0){
            myColumn.elevatorArray[i].currentFloor = parseInt(myCurrentFloor1);
            myColumn.elevatorArray[i].direction = myCurrentDirection1;
            myColumn.elevatorArray[i].status = myCurrentStatus1;
        } else {
            myColumn.elevatorArray[i].currentFloor = parseInt(myCurrentFloor2);
            myColumn.elevatorArray[i].direction = myCurrentDirection2;
            myColumn.elevatorArray[i].status = myCurrentStatus2;
        }
    }
    myColumn.requestElevator(parseInt(myRequestedFloor), myRequestedDirection);
}

function scenario4Method2RequestFloor(){
    console.log(``);
    console.log(`****** METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR`);
    var elevatorSelected = (prompt(`Elevator Selected Method 1: \n(1 - ${numberOfElevatorForTest})`) - 1);
    if (elevatorSelected < 0 || elevatorSelected > 1){
        alert("Invalid Elevator");
        scenario4Method2RequestFloor()
    }
    elevator = myColumn.elevatorArray[elevatorSelected];
    var userRequesFloor = prompt(`User going to Floor: \n(1 - ${numberoOfFloorForTest})`);
    if (userRequesFloor < 1 || userRequesFloor > 10){
        alert("Invalid Floor");
        scenario4Method2RequestFloor()
    }
    myColumn.requestFloor(elevator, userRequesFloor);
}

function buttonScenario4(){
    scenario4Method1RequestElevator();
    scenario4Method2RequestFloor();
}

