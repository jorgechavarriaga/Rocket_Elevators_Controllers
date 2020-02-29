/**********************************************************************************************************
*** PROGRAM:      CODEBOXX ODSSEY                                                                       ***     
*** WEEK 3:       THE MECHANICS OF COMPILED LANGUAGES													***
*** DELIVERABLE:  COMMERCIAL CONTROLLER - C#															***
*** BY:           JORGE CHAVARRIAGA                                                                     ***
*** DATE:         28-FEB-2020                                                                           ***
***********************************************************************************************************/

using System;
using System.Collections.Generic;


// *********************************** 1. Column class definition Start ***********************************
public class Column  
{ 
    public int id;
    public int numberOfFloorToServe;
    public int numberOfElevator; 
    public List<ExternalButton> externalButtonList; 
    public List<Elevator> elevatorList;
    public int startingFloor;
    public int endingFloor;
        
    public Column(int id, int numberOfFloorToServe, int numberOfElevator, List<int> startingFloorList, List<int> endingFloorList)
    {
        this.id = id + 1;
        this.numberOfFloorToServe = numberOfFloorToServe;
        this.numberOfElevator = numberOfElevator;
        this.externalButtonList = new List<ExternalButton>();
        this.elevatorList = new List<Elevator>();
        this.startingFloor = startingFloorList[id];
        this.endingFloor = endingFloorList[id];
        for (int i = 0; i < this.numberOfElevator; i++) {
            Elevator elevator = new Elevator(i, 1, numberOfFloorToServe, startingFloor, endingFloor);
            this.elevatorList.Add(elevator);
        }
        this.InitExternalButtonList();
    }

    public void InitExternalButtonList()
    {   
        ExternalButton buttonFirstFloor = new ExternalButton(1, "UP", false);
        this.externalButtonList.Add(buttonFirstFloor);				
   		for (int i = startingFloor; i <= endingFloor; i++)
		{
            if (i != endingFloor)
            {
				ExternalButton buttonUp = new ExternalButton(i, "UP", false);
                this.externalButtonList.Add(buttonUp);
            }
            if (i != 1)
            {
                ExternalButton buttonDown = new ExternalButton(i, "DOWN", false);
                this.externalButtonList.Add(buttonDown);
            }			
        }     
    }

    public Elevator FindElevatorById(int id) 
    {                       
        for (int i = 0; i < this.elevatorList.Count; i++) {
            if (this.elevatorList[i].id == id) {
                Console.WriteLine($"Find Elevator By Id Return Column Id: {this.id}, ElevatorId {this.elevatorList[i].id}, Current floor {this.elevatorList[i].currentFloor}");
                return this.elevatorList[i];
            }
        }
        Console.WriteLine("Find Elevator By Id Return Null");
        return null;
    }             
    
    public Elevator FindBestElevator(int requestedFloor, string direction)
    {
        int bestGap = numberOfFloorToServe;
        int elevatorIdWithBestGap = 0;
		for (int i = 0; i < elevatorList.Count; i++) {
			if (((elevatorList[i].status == "IDLE") || (elevatorList[i].direction == "UP" && direction == "UP" && (requestedFloor >= elevatorList[i].currentFloor))) || (elevatorList[i].direction == "DOWN" && direction == "DOWN" && (requestedFloor <= elevatorList[i].currentFloor)))
			{
				
				elevatorList[i].myCase = 1;
				//Console.WriteLine($"Elevator {elevatorList[i].id},  myCase: {elevatorList[i].myCase}");
			}
			if (((elevatorList[i].direction == "UP" && direction == "UP" && (requestedFloor < elevatorList[i].currentFloor))) || ((elevatorList[i].direction == "DOWN" && direction == "DOWN" && (requestedFloor > elevatorList[i].currentFloor))) || ((elevatorList[i].direction == "DOWN" &&  direction == "UP" && (requestedFloor >= elevatorList[i].currentFloor))) || ((elevatorList[i].direction == "DOWN" && direction == "UP" && (requestedFloor >= elevatorList[i].currentFloor))) || (elevatorList[i].direction == "DOWN" && direction == "UP" && (requestedFloor < elevatorList[i].currentFloor)))
			{
				elevatorList[i].myCase = 2;
				//Console.WriteLine($"Elevator {elevatorList[i].id},  myCase: {elevatorList[i].myCase}");
			}
		}
		for (int i = 0; i < this.elevatorList.Count; i++){
			if (elevatorList[i].myCase == 1)
			{
				int gap = Math.Abs(this.elevatorList[i].currentFloor - requestedFloor);
				if (gap < bestGap)
				{
					elevatorIdWithBestGap = this.elevatorList[i].id;
					//Console.WriteLine($"Elevator id with best gap: {this.elevatorList[i].id}");
					bestGap = gap;
					
					Console.WriteLine($"Elevator id {elevatorList[i].id},  gap: {gap}, in case: {elevatorList[i].myCase}");

				}
				//Console.WriteLine($"Elevator id {this.elevatorList[i].id} con gap: {gap}");
			}
			else if (elevatorList[i].myCase == 2) 
			{
				elevatorIdWithBestGap = this.elevatorList[i].id;
				int gap = Math.Abs(this.elevatorList[i].currentFloor - elevatorList[i].queue[0]) + Math.Abs(elevatorList[i].queue[0] - requestedFloor) ;
					Console.WriteLine($"Elevator id {elevatorList[i].id} with a gap: {gap}, in case: {elevatorList[i].myCase}");
					bestGap = gap;
				//Console.WriteLine($"Elevator id {elevatorList[i].id} whit a gap: {gap}");

			}

			//Console.WriteLine($"The Elevator {elevatorList[i].id} with a gap: {bestGap}");
			//return this.FindElevatorById(elevatorIdWithBestGap);
		}
		//return null;
		return this.FindElevatorById(elevatorIdWithBestGap);

	}
	public Elevator RequestElevator(int requestedFloor, string direction)
    {
        Console.WriteLine($"Request Elevator: {requestedFloor} and direction: {direction}");            
        Elevator elevator = FindBestElevator(requestedFloor, direction);
        elevator.AddQueue(requestedFloor);
        elevator.MoveElevator();
        return elevator;
    }
    public void RequestFloor(Elevator elevator , int requestedFloor)
	{
    Console.WriteLine($"Request Floor: {requestedFloor}, with elevator: {elevator.id}, and current floor: {elevator.currentFloor}");
    elevator.AddQueue(requestedFloor);
    elevator.CloseDoor();
    elevator.MoveElevator();
	}
}


// *********************************** 2. Battery class definition Start **********************************
public class Battery 
{
	public int numberOfColumn;
    public int numberOfElevator;
    public int numberOfFloorToServe;
	public List<Column> columnList;
	public List<int> startingFloorList;
	public List<int> endingFloorList;
	public Battery(int numberOfColumn, int numberOfElevator, int numberOfFloorToServe,List<int> startingFloorList, List<int> endingFloorList)
	{
		this.numberOfColumn = numberOfColumn;
        this.numberOfElevator = numberOfElevator;
        this.numberOfFloorToServe = numberOfFloorToServe;
		this.columnList = new List<Column>();
		this.startingFloorList = startingFloorList;
		this.endingFloorList = endingFloorList;
		for (int i = 0; i < numberOfColumn; i++)
		{
			Column column = new Column(i, numberOfFloorToServe, numberOfElevator, startingFloorList, endingFloorList);
			this.columnList.Add(column);			
		}
	}
}


// *********************************** 3. Elevator class definition Start *********************************
public class Elevator
{
	public int id;
	public string direction;
	public int numberOfFloorToServe;
	public int currentFloor;
	public string status;
	public List<int> queue;
	public List<InternalButton> internalButtonList; 
	public string door = "CLOSED";
	public bool sameDirection = false;
	public int myCase;
	public int startingFloor;
    public int endingFloor;
	public Elevator(int id, int currentFloor, int numberOfFloorToServe, int startingFloor, int endingFloor)
	{
		this.id = id + 1;													// Elevator's Id
		this.direction = "";												// Direction UP/DOWN
		this.numberOfFloorToServe = numberOfFloorToServe; 
		this.currentFloor = currentFloor;									// Elevator's current floor (Position)
		this.status = "IDLE";												// Elevator's status IDLE/MOVING
		this.queue = new List<int>();
		this.internalButtonList = new List<InternalButton>();
		this.door = "CLOSED";												// Elevator's doors CLOSED/OPENED
		this.myCase = 0;
		this.sameDirection = false;											// Elevator's same direction true/false
		this.startingFloor = startingFloor;									// Elevator's starting service
        this.endingFloor = endingFloor;                                     // Elevator's ending service
		InternalButton buttonFirstFloor = new InternalButton(1, false);
        this.internalButtonList.Add(buttonFirstFloor);	
   		for (int i = startingFloor; i <= endingFloor; i++)
		{
				InternalButton button = new InternalButton(i, false);
                this.internalButtonList.Add(button);			
        }
	}
	public void AddQueue(int requestedFloor) 
	{
	//Console.WriteLine($"Adding to Queue: [{requestedFloor}] to [{(String.Join(", ", this.queue))}]");
	this.queue.Add(requestedFloor);
	//Console.WriteLine($"Update Queue: [{(String.Join(", ", this.queue))}]");
	if (this.direction == "UP") 
		{
			this.queue.Sort((a, b) => a - b);									// If going UP sort ascending
			//Console.WriteLine($"Sort ascending for Up direction: [{(String.Join(", ", this.queue))}]");
		}
	if (this.direction == "DOWN") 
		{
		this.queue.Sort((a, b) => b - a);                                   // If going UP sort descending
			//Console.WriteLine($"Sort decending for Down direction: [{(String.Join(", ", this.queue))}]");
		}
	}
	public void MoveElevator() 
	{
	Console.WriteLine("Start moving the Elevator");
		while (this.queue.Count > 0)
		{
			if (this.door == "OPENED"){this.CloseDoor();}
			int firstElement = this.queue[0];
			if (firstElement == this.currentFloor) 
			{
				this.queue.RemoveAt(0); 
				this.OpenDoor();
				Console.WriteLine($"Door for: {firstElement}");
			}
			if (firstElement > this.currentFloor) 
			{
				this.status = "MOVING";
				this.direction = "UP";
				this.MoveUp();
			}
			if (firstElement < this.currentFloor) 
			{
				this.status = "MOVING";
				this.direction = "DOWN";
				this.MoveDown();
			}
		}
		if (this.queue.Count > 0) {} else {
			Console.WriteLine("Elevator Status: Idle");
			this.status = "IDLE";
		}
	}
	public void MoveUp() 
	{
		this.currentFloor = (this.currentFloor + 1);						// Going up, current floor plus 1 floor
		Console.WriteLine($"Current floor: {this.currentFloor}");
		//Console.Beep();
	}

	public void MoveDown() 
	{
		this.currentFloor = (this.currentFloor - 1);                        // Going down, current floor lesss 1 floor
		Console.WriteLine($"Current floor: {this.currentFloor}");
		//Console.Beep();
	}

	public void OpenDoor() 
	{
		this.door = "OPENED";
		//Console.WriteLine("Open door");
	}

	public void CloseDoor() 
	{
		this.door="CLOSED";
		//Console.WriteLine("Close door");
	}

}
    

// *********************************** 4. ExternalButton class definition Start ***************************
public class ExternalButton
{
	public int floor;
    public string direction;
    public bool activated;

	public ExternalButton(int floor, string direction, bool activated)
	{
		this.floor = floor;
		this.direction = direction;												// Direction UP/DOWN
		this.activated = false; 
	}
}


// *********************************** 5. InternalButton class definition Start ***************************
public class InternalButton
{
	public int floor;
    public bool activated;

	public InternalButton(int floor, bool activated)
	{
		this.floor = floor;
		this.activated = false; 			
	}
}

	
// *********************************** Testing class definition Start **********************************
public class Testing  
{
	static void Main()
	{
		int numberOfColumn = 4;
		int numberOfElevator = 5;
		int numberOfFloorToServe = 66;
		int columnA = 0;
		int columnB = 1;
		int columnC = 2;
		int columnD = 3;

		int b1 = 0; // b1 elevator 1 column 1 
		int b2 = 1; // b1 elevator 2 column 1 
		int b3 = 2; // b1 elevator 3 column 1 
		int b4 = 3; // b1 elevator 4 column 1 
		int b5 = 4; // b1 elevator 5 column 1  
		
		List<int> startingFloorList = new List<int> { 1,  7, 27, 47 };
		List<int> endingFloorList   = new List<int> { 6, 26, 46, 66 };
		Battery scenario = new Battery(numberOfColumn, numberOfElevator, numberOfFloorToServe,startingFloorList, endingFloorList);

		// Scenario 1
		
		scenario.columnList[columnB].elevatorList[b1].currentFloor = 26; 
		scenario.columnList[columnB].elevatorList[b1].direction = "DOWN";
		scenario.columnList[columnB].elevatorList[b1].status = "MOVING";
		scenario.columnList[columnB].elevatorList[b1].queue.Add(11);
		
		scenario.columnList[columnB].elevatorList[b2].currentFloor = 9;
		scenario.columnList[columnB].elevatorList[b2].direction = "UP";
		scenario.columnList[columnB].elevatorList[b2].status = "MOVING";
		scenario.columnList[columnB].elevatorList[b2].queue.Add(21);
		
		scenario.columnList[columnB].elevatorList[b3].currentFloor = 19;
		scenario.columnList[columnB].elevatorList[b3].direction = "DOWN";
		scenario.columnList[columnB].elevatorList[b3].status = "MOVING";
		scenario.columnList[columnB].elevatorList[b3].queue.Add(7);
		
		scenario.columnList[columnB].elevatorList[b4].currentFloor = 21;
		scenario.columnList[columnB].elevatorList[b4].direction = "DOWN";
		scenario.columnList[columnB].elevatorList[b4].status = "MOVING";
		scenario.columnList[columnB].elevatorList[b4].queue.Add(8);
		
		scenario.columnList[columnB].elevatorList[b5].currentFloor = 12;
		scenario.columnList[columnB].elevatorList[b5].direction = "DOWN";
		scenario.columnList[columnB].elevatorList[b5].status = "MOVING";
		scenario.columnList[columnB].elevatorList[b5].queue.Add(7);
		
		Elevator elevator = scenario.columnList[columnB].RequestElevator(7, "UP");
		scenario.columnList[columnB].RequestFloor(elevator, 26);
		Console.WriteLine("");


		/*
		//Scenario 2
		scenario.columnList[columnC].elevatorList[b1].currentFloor = 7;
		scenario.columnList[columnC].elevatorList[b1].direction = "UP";
		scenario.columnList[columnC].elevatorList[b1].status = "MOVING";
		scenario.columnList[columnC].elevatorList[b1].queue.Add(27);

		scenario.columnList[columnC].elevatorList[b2].currentFloor = 29;
		scenario.columnList[columnC].elevatorList[b2].direction = "UP";
		scenario.columnList[columnC].elevatorList[b2].status = "MOVING";
		scenario.columnList[columnC].elevatorList[b2].queue.Add(34);

		scenario.columnList[columnC].elevatorList[b3].currentFloor = 39;
		scenario.columnList[columnC].elevatorList[b3].direction = "DOWN";
		scenario.columnList[columnC].elevatorList[b3].status = "MOVING";
		scenario.columnList[columnC].elevatorList[b3].queue.Add(7);

		scenario.columnList[columnC].elevatorList[b4].currentFloor = 46;
		scenario.columnList[columnC].elevatorList[b4].direction = "DOWN";
		scenario.columnList[columnC].elevatorList[b4].status = "MOVING";
		scenario.columnList[columnC].elevatorList[b4].queue.Add(30);

		scenario.columnList[columnC].elevatorList[b5].currentFloor = 45;
		scenario.columnList[columnC].elevatorList[b5].direction = "DOWN";
		scenario.columnList[columnC].elevatorList[b5].status = "MOVING";
		scenario.columnList[columnC].elevatorList[b5].queue.Add(7);

		Elevator elevator = scenario.columnList[columnC].RequestElevator(7, "UP");
		scenario.columnList[1].RequestFloor(elevator, 42);
		
		
		// Scenario 3
		scenario.columnList[columnD].elevatorList[b1].currentFloor = 64;
		scenario.columnList[columnD].elevatorList[b1].direction = "DOWN";
		scenario.columnList[columnD].elevatorList[b1].status = "MOVING";
		scenario.columnList[columnD].elevatorList[b1].queue.Add(7);

		scenario.columnList[columnD].elevatorList[b2].currentFloor = 56;
		scenario.columnList[columnD].elevatorList[b2].direction = "UP";
		scenario.columnList[columnD].elevatorList[b2].status = "MOVING";
		scenario.columnList[columnD].elevatorList[b2].queue.Add(66);

		scenario.columnList[columnD].elevatorList[b3].currentFloor = 52;
		scenario.columnList[columnD].elevatorList[b3].direction = "UP";
		scenario.columnList[columnD].elevatorList[b3].status = "MOVING";
		scenario.columnList[columnD].elevatorList[b3].queue.Add(64);

		scenario.columnList[columnD].elevatorList[b4].currentFloor = 7;
		scenario.columnList[columnD].elevatorList[b4].direction = "UP";
		scenario.columnList[columnD].elevatorList[b4].status = "MOVING";
		scenario.columnList[columnD].elevatorList[b4].queue.Add(60);

		scenario.columnList[columnD].elevatorList[b5].currentFloor = 66;
		scenario.columnList[columnD].elevatorList[b5].direction = "DOWN";
		scenario.columnList[columnD].elevatorList[b5].status = "MOVING";
		scenario.columnList[columnD].elevatorList[b5].queue.Add(7);

		Elevator elevator = scenario.columnList[columnD].RequestElevator(60, "DOWN");
		scenario.columnList[columnD].RequestFloor(elevator, 7);
		Console.WriteLine("");
		
		
		// Scenario 4
		scenario.columnList[columnA].elevatorList[b1].currentFloor = 4;
		scenario.columnList[columnA].elevatorList[b1].direction = "";
		scenario.columnList[columnA].elevatorList[b1].status = "IDLE";

		scenario.columnList[columnA].elevatorList[b2].currentFloor = 7;
		scenario.columnList[columnA].elevatorList[b2].direction = "";
		scenario.columnList[columnA].elevatorList[b2].status = "IDLE";

		scenario.columnList[columnA].elevatorList[b3].currentFloor = 3;
		scenario.columnList[columnA].elevatorList[b3].direction = "DOWN";
		scenario.columnList[columnA].elevatorList[b3].status = "MOVING";
		scenario.columnList[columnA].elevatorList[b3].queue.Add(5);

		scenario.columnList[columnA].elevatorList[b4].currentFloor = 6;
		scenario.columnList[columnA].elevatorList[b4].direction = "UP";
		scenario.columnList[columnA].elevatorList[b4].status = "MOVING";
		scenario.columnList[columnA].elevatorList[b4].queue.Add(7);

		scenario.columnList[columnA].elevatorList[b5].currentFloor = 1;
		scenario.columnList[columnA].elevatorList[b5].direction = "DOWN";
		scenario.columnList[columnA].elevatorList[b5].status = "MOVING";
		scenario.columnList[columnA].elevatorList[b5].queue.Add(6);

		Elevator elevator = scenario.columnList[columnA].RequestElevator(3, "UP");
		scenario.columnList[columnA].RequestFloor(elevator, 7);
		Console.WriteLine("");
		*/
	}
}
