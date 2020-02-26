/**********************************************************************************************************
*** PROGRAM:      CODEBOXX ODSSEY                                                                       ***               
*** WEEK 3:       THE MECHANICS OF COMPILDE LANGUAGES                                                   ***
*** DELIVERABLE:  RESIDENTIAL CONTROLLER - C#                                                           ***
*** BY:           JORGE CHAVARRIAGA                                                                     ***
*** DATE:         28-FEB-2020                                                                           ***
***********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Threading;

public class Column
{
    public int numberOfFloorToServe;
    public int numberOfElevatorInBuilding;
    public List<Elevator> elevatorList;
    public List<ExternalButton> externalButtonList;
    public Column(int numberOfFloorToServe, int numberOfElevatorInBuilding)
    {
        this.numberOfFloorToServe = numberOfFloorToServe;
        this.numberOfElevatorInBuilding = numberOfElevatorInBuilding;
        this.elevatorList = new List<Elevator>();
        this.externalButtonList = new List<ExternalButton>();
        for (int i = 0; i < this.numberOfElevatorInBuilding; i++) {
            Elevator elevator = new Elevator(i + 1, 1, numberOfElevatorInBuilding);
            this.elevatorList.Add(elevator);
        }
        this.SetUpExternalButtonList();
    }
    public void SetUpExternalButtonList()
    {
        for (int i = 1; i < this.numberOfFloorToServe; i++){
            this.externalButtonList.Add(new ExternalButton(i, "UP"));
        }
        for (int i = 2; i < this.numberOfFloorToServe + 1; i++){
            this.externalButtonList.Add(new ExternalButton(i, "DOWN"));
        }
    }
    public Elevator FindElevatorId(int id)
    {
        for (int i = 0; i < this.elevatorList.Count; i++)
        {
            if (this.elevatorList[i].id == id)
            {
                Console.WriteLine("Elevator Selected: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Elevator # {this.elevatorList[i].id} at Floor: {this.elevatorList[i].currentFloor}");
                Console.ForegroundColor = ConsoleColor.White;
                return this.elevatorList[i];
            }
        }
        return null;
    }
    public Elevator FindBestElevatorOption(int requestedFloor, string direction)
    {
        bool idleStatus = false;
        int bestGap = this.numberOfFloorToServe;
        int elevatorBestGap = 0;
        for (int i = 0; i < this.elevatorList.Count; i++)
        {
            if (this.elevatorList[i].status == "IDLE"){
            idleStatus = true;
            } else if ((this.elevatorList[i].direction == "UP") && (direction == "UP") && (requestedFloor > this.elevatorList[i].currentFloor)) {
                this.elevatorList[i].sameDirecton = true;
            } else if ((this.elevatorList[i].direction == "DOWN") && (direction == "DOWN") && (requestedFloor < this.elevatorList[i].currentFloor)) {
                this.elevatorList[i].sameDirecton = true;
            } else{
                this.elevatorList[i].sameDirecton = false;
            }
        }
        if (idleStatus == true) {
            for (int i = 0; i< this.elevatorList.Count; i++) {
                if (this.elevatorList[i].status == "IDLE") {
                    int gap = Math.Abs(this.elevatorList[i].currentFloor - requestedFloor);
                    if (gap < bestGap){
                        elevatorBestGap = this.elevatorList[i].id;
                        bestGap = gap;
                    }
                }
            }
            return this.FindElevatorId(elevatorBestGap);
        }
        if (idleStatus == false) {
            for (int i = 0; i< this.elevatorList.Count; i++) {
                if (this.elevatorList[i].sameDirecton == true) {
                    int gap = Math.Abs(this.elevatorList[i].currentFloor - requestedFloor);
                    if (gap < bestGap){
                        elevatorBestGap = this.elevatorList[i].id;
                        bestGap = gap;
                    }
                }
            }
            return this.FindElevatorId(elevatorBestGap);
        }
        return null;
    }
    public Elevator RequestElevator(int requestedFloor, string direction)
    {
        Console.WriteLine($"User in Floor: {requestedFloor} and going {direction}");
        Elevator elevator = this.FindBestElevatorOption(requestedFloor, direction);
        elevator.AddToQueue(requestedFloor);
        elevator.MoveElevator();
        return elevator;
    }
    public void RequestFloor(Elevator elevator, int requestedFloor)
    {
        Console.WriteLine($"User request Floor: {requestedFloor} with Selected Elevator #: {elevator.id} at Floor: {elevator.currentFloor}");
        elevator.AddToQueue(requestedFloor);
        elevator.CloseDoor();
        elevator.MoveElevator();
    }
}

public class Elevator
{
    public int id;
    public int currentFloor;
    public int numberOfFloorToServe;
    public string door;
    public string status;
    public bool sameDirecton;
    public string direction;
    public List<InternalButton> internalButtonList;
    public List<int> queue;
    public Elevator(int id, int currentFloor, int numberOfFloorToServe)
    {
        this.id = id; 								                    // Elevator[0] Id1, Elevator[1] Id2
        this.currentFloor = currentFloor;                               // Elevator current Floor
        this.numberOfFloorToServe = numberOfFloorToServe;               // Number of Floors
        this.internalButtonList = new List<InternalButton>();           // Destination Floor:  [0] Floor1, [1] Floor2, ... [9] Floor10 
        this.queue = new List<int>();                        // Elevator Queue
        this.door = "CLOSED";                                           // Door status: CLOSED/OPENED
        this.status = "IDLE"; 						                    // Elevator Status: IDLE/MOVING
        this.sameDirecton = false;                                      // Elevator Same direction of service
        this.direction = ""; 						                    // Elevator Direction: UP/DOWN/null(not moving)
        
    }
    public void AddToQueue(int requestedFloor) 
    {
        Console.WriteLine($"Add Floor: {requestedFloor} to Queue [{(String.Join(",", this.queue))}]");
        this.queue.Add(requestedFloor);
        Console.WriteLine($"Floor: {requestedFloor} Added to Queue [{(String.Join(",", this.queue))}]");
        if (this.direction == "UP") {
            this.queue.Sort((a, b) => a - b);                           // Sort for going up Ascending
            Console.WriteLine($"Floor: {requestedFloor}  Sorted Ascending [{(String.Join(",", this.queue))}]");
        }
        if (this.direction == "DOWN") {
            this.queue.Sort((a, b) => b - a);                           // Sort for going down Descending
            Console.WriteLine($"Floor: {requestedFloor} Sorted Descending [{(String.Join(",", this.queue))}]"); 
        }

    }
    public void MoveElevator()
    {
        while (this.queue.Count > 0)
        {
            if (this.door == "OPENED"){
                this.CloseDoor();
            }
            int firstReq = this.queue[0];
            if (firstReq == this.currentFloor)
            {
                Console.Beep();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Arrive at Floor: {firstReq}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Remove request from Queue: [{firstReq}]");
                this.queue.RemoveAt(0);                                     // Remove first service from the queue
                this.OpenDoor();
                Thread.Sleep(3000);

            }
            if (firstReq > this.currentFloor)
            {
                this.status = "MOVING";
                this.direction = "UP";
                this.MoveUp();
            }

            if (firstReq < this.currentFloor)
            {
                this.status = "MOVING";
                this.direction = "DOWN";
                this.MoveDown();
            }
        }
        if (this.queue.Count > 0)
        {
        }
        else
        {
            this.status = "IDLE";
            Console.WriteLine($"Elevator Status: {this.status}"); 
        }
    }
    public void MoveUp() 
    {
        this.currentFloor = (this.currentFloor + 1);                    // Actual Floor + 1 (Going Up increasing)
        Console.WriteLine($"Going UP - Floor: {this.currentFloor}");
    }
    public void MoveDown() 
    {
        this.currentFloor = (this.currentFloor - 1);                    // Actual Floor - 1 (Going down decreasing)
        Console.WriteLine($"Going DOWN - Floor: {this.currentFloor}");
    }
    public void OpenDoor()
    {
        this.door = "Door Opened";
        Console.WriteLine("Open Door"); 
        if (this.queue.Count > 0)
        {
            this.CloseDoor();
        }
    }
    public void CloseDoor()
    {
        this.door = "CLOSED";
        Console.WriteLine("Close Door");
    }
}

public class InternalButton
{
    public int floor;
    public InternalButton(int floor) 
    {
        this.floor = floor;    
    }
}

public class ExternalButton
{
    public int requestFloor;
    public string direction;
    public ExternalButton(int requestFloor, string direction)
    {
        this.requestFloor = requestFloor;
        this.direction = direction;

    }
}

public class Test
{
    static void Main()
    {
        // Scenario 1
        int numberoOfFloorForTest = 66;
        int numberOfElevatorForTest = 5;
        Column myColumn = new Column(numberoOfFloorForTest, numberOfElevatorForTest);
        Console.WriteLine("****** SCENARIO 1");
        Console.WriteLine("****** Elevator 1: Idle @ Floor 2 - Elevator 2: Idle @ Floor 6  -  User on Floor: 3 and Going UP to Floor: 7");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("****** METHOD 1: REQUEST ELEVATOR WITH REQUESTED FLOOR AND DIRECTION");
        Console.ForegroundColor = ConsoleColor.White;
        myColumn.elevatorList[0].currentFloor = 15;
        myColumn.elevatorList[0].direction = "";
        myColumn.elevatorList[0].status = "IDLE";

        myColumn.elevatorList[1].currentFloor = 30;
        myColumn.elevatorList[1].direction = "";
        myColumn.elevatorList[1].status = "IDLE";

        myColumn.elevatorList[2].currentFloor = 45;
        myColumn.elevatorList[2].direction = "";
        myColumn.elevatorList[2].status = "IDLE";

        myColumn.elevatorList[3].currentFloor = 60;
        myColumn.elevatorList[3].direction = "";
        myColumn.elevatorList[3].status = "IDLE";
        
        myColumn.elevatorList[4].currentFloor = 66;
        myColumn.elevatorList[4].direction = "";
        myColumn.elevatorList[4].status = "IDLE";
        int userRequestedFloor = 20;
        string userRequestedDirection = "UP";
        myColumn.RequestElevator(userRequestedFloor, userRequestedDirection);
        
        Console.WriteLine(); Console.WriteLine();
        Thread.Sleep(3000);
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("******METHOD 2: REQUEST FLOOR WITH ELEVATOR AND REQUESTED FLOOR");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("******User requested Floor: 7");
        Console.WriteLine();
        Elevator elevator = myColumn.elevatorList[0];
        int userRequesFloor = 7;
        myColumn.RequestFloor(elevator, userRequesFloor);
        Thread.Sleep(3000);

    }
}