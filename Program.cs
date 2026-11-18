/*
	NOTE FROM JACOB SEADORF -- PLEASE READ!
	I figure that a small little role playing game would be doable. Nothing like a roguelike, but a simple text adventure.
	(This is something that Jacob Powell and I had discussed over a zoom call)
	
	Maybe the player could still encounter monsters and enemies? 
	If it's not needed, health could be removed, but I added it just in case we end up using it.
 */

using System;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using System.Xml.Linq;
using static System.Console;




class Room {
	public Room(int roomNumber, int description) {
		RoomNumber = roomNumber;
		DescriptionIndex = description;
		Visited = false;
		Locked = false;
	}

	public int RoomNumber { get; set; }
	public int DescriptionIndex { get; set; }
	public Room North { get; set; }
	public Room East { get; set; }
	public Room South { get; set; }
	public Room West { get; set; }
	public bool Locked { get; set; }
	public bool Visited { get; set; }
}
class Player {
	public Player(string name, Room startingRoom) {
		Name = name;
		CurrentRoom = startingRoom;
		PreviousRoom = null;
		PlayerHasKey = false;
		IsPlayerDead = false;
		GameOver = false;
	}

	public Room CurrentRoom { get; set; }
	public Room PreviousRoom { get; set; }
	public bool PlayerHasKey { get; set; }
	public string Name { get; set; }
	public bool IsPlayerDead { get; set; }
	public bool GameOver { get; set; }
}

class Enemy {
	public void EnemyTaunt() {
		WriteLine("GIMME ALL YOUR MONEY!!!");
	}

	public void Confrontation() {
		EnemyTaunt();
		WriteLine("What is your action? R for run, A for attack, B for bribe >>");
		string Response = ReadLine();

		// Placeholder response actions... 

		if(Response == "A" || Response == "a") {

			//maybe roll a random number for damage, and subtract from the player's health?

			WriteLine("You died... He had a bazooka");


		} else if(Response == "B" || Response == "b") {
			WriteLine("He took literally everything you had... but you lived");


		} else if(Response == "R" || Response == "r") {
			WriteLine("Somehow you escaped...");


		} else {
			WriteLine("You froze! The perpetrator kidnapped you!");
		}
	}


	public int health { get; set; }

	public int attackStrength { get; set; }

	private int _health;
	private int _attackStrength;
}
// End of my tiny addition JP 



class NameException : Exception {
	public NameException(string name) : base("Name cannot be empty.") { }
}

class Program {

    static void movePlayer(Player player, string direction, string[] descriptions) {
        Room nextRoom = null;
        switch(direction) {
            case "north":
                nextRoom = player.CurrentRoom.North;
                break;
            case "east":
                nextRoom = player.CurrentRoom.East;
                break;
            case "south":
                nextRoom = player.CurrentRoom.South;
                break;
            case "west":
                nextRoom = player.CurrentRoom.West;
                break;
            default:
                //player cannot go that way
                WriteLine(descriptions[90]);
                return;
        }

		if(player.CurrentRoom.RoomNumber == 0 && direction.ToLower() == "south") {
			//player enters south in the first room, exit the dungeon
			WriteLine(descriptions[99]);
			player.GameOver = true;
			return;
		}

		if(nextRoom == null) {
			WriteLine(descriptions[90]);
			return;
		}



		if(nextRoom.Locked) {
			WriteLine(descriptions[91]);
			if(!player.PlayerHasKey) {
				WriteLine(descriptions[92]);
				return;
			}
			//player has the key, unlock the door
			nextRoom.Locked = false;
		}

		//remember previous room
		player.PreviousRoom = player.CurrentRoom;
		//actually move the player
		player.CurrentRoom = nextRoom;
		//mark the new room as visited
		player.CurrentRoom.Visited = true;
		
		//pick up the key in room 3 (west of the starting room)
		if(player.CurrentRoom.RoomNumber == 3 && !player.PlayerHasKey) {
			player.PlayerHasKey = true;
		}

		//display new room description
		Console.Clear();
		//WriteLine(descriptions[player.CurrentRoom.DescriptionIndex]);
    }


    static void mainMenu() {
		WriteLine("\tMain Menu");
		WriteLine("\t============");
		WriteLine("\t1) Play");
		WriteLine("\t2) Credits");
		WriteLine("\t3) Instructions");
		WriteLine("\t99) Quit");
		int menuChoice = Convert.ToInt32(ReadLine());
		switch(menuChoice) {
			case 1:
				Console.Clear(); //clear the screen before the game starts. Makes it so that once the game starts, the menu doesn't show up.
				gameLoop();
				break;
			case 2:
				Console.Clear();
				WriteLine("\tThis game was created for a final project at Grand Rapids Community College.");
				WriteLine("\t==================================================\n");
				WriteLine("\tThe team members for this project were:");
				WriteLine("\t* Jacob Seadorf");
				WriteLine("\t* Jacob Powell");
				WriteLine("\t* Brandon Choang");
				mainMenu();
				break;
			case 3:
				Console.Clear();
				WriteLine("\t\tGAME INSTRUCTIONS");
				WriteLine("\t======================================");
				WriteLine("The game will display a room description and a set of valid directions.");
				WriteLine("The valid directions you can enter are the first letter of the direction (n, e, s, w), \nor the full word (north, east, south, west).\nThe player input is not case-sensative.");
				mainMenu();
				break;
			case 99:
                Environment.Exit(0); //zero is used to mean "nothing went wrong"; exiting with 1 or another number usually means an error. 
                break;
			default:
				WriteLine("Error: Invalid Menu Option");
				mainMenu();
				break;
		}
	}


		//Actual game logic happens here, called from the while loop in mainMenu
	static void gameLoop() {
	   string[] descriptions = new string[100];
		//descriptions for special circumstances are best, in my opinion, left at the end of the array.
		descriptions[90] = "You cannot go that way."; //obvious what this does	
		descriptions[91] = "The door to this room is locked.";
		descriptions[97] = "You flee from the monster!"; //maybe a random chance to not flee?
		descriptions[99] = "You have left the dungeon of your own free will.\n\nGAME OVER"; //player enters "south" in the first room
		descriptions[98] = "You died!\n\nGAME OVER"; //obvious what this does 
		descriptions[92] = "You do not have the key. You cannot go this way until you find it."; //player does not have the key for a door
		
		//regular rooms are from 0-89, plenty of different options we could do.
		descriptions[0] = "ROOM 1\nTo the north you see a locked door. To the west, you see a room with a fountain. To the south, you see the exit."; //first room, player starts here
		descriptions[1] = "ROOM 2\nTo the south, you see the beginning of the dungeon. To the west, you see a set of stairs.";
		descriptions[2] = "ROOM 3\nTo the east you see the room with the locked door." +
			"\nIn this room, you see a fountain. As you walk over to the fountain, you see a key shimmering in the water.\nYou pick up the key and add it to your inventory.";
		descriptions[3] = "ROOM 4\nOpening the door, you see a set of stairs. Going down the stairs you see a dusty old library.";
		descriptions[4] = "ROOM 5\n";

			//create the rooms
		Room startingRoom = new Room(0, 0);
		Room lockedDoor = new Room(1, 1);
		Room fountainRoom = new Room(2, 2);
		Room afterLockedDoor = new Room(3, 3);

		//define connections to each room 
		
		//room 0 connects to room 2 via the west    
		startingRoom.West = fountainRoom;
		fountainRoom.East = startingRoom;
		//room 0 connects to room 1 via the north; this is the locked door that requires a key.
		startingRoom.North = lockedDoor;
		lockedDoor.South = startingRoom;


		//lock room 1
		lockedDoor.Locked = true;

		Write("What is your name? ");
		string playerName = ReadLine();
			
		//check if player name is empty
		if(String.IsNullOrWhiteSpace(playerName)) { throw new NameException(playerName); } else { WriteLine("Go forth, " + playerName + "!"); }


		//actually create the player and start the gmae
		Player player = new Player(playerName, startingRoom);
		player.CurrentRoom.Visited = true;

		
		while(player.IsPlayerDead == false && player.GameOver == false) {
			Console.Clear();
            WriteLine(descriptions[player.CurrentRoom.DescriptionIndex]);
            Write("Where do you want to go? >> ");
			string input = ReadLine();
			string direction = processInput(input);
            movePlayer(player, direction, descriptions);
        }
	}


		//processInput just recognizes the player's input; actually checking to see if the player can move to a room is done by movePlayer
		static string processInput(string input) {
			switch(input) {
				case "N":
				case "n":
				case "north":
				case "North":
				case "NORTH":
						return "north";
				case "E":
				case "e":
				case "east":
				case "East":
				case "EAST":
					return "east";
				case "S":
				case "s":
				case "south":
				case "South":
				case "SOUTH":
						return "south";
				case "w":
				case "W":
				case "west":
				case "West":
				case "WEST":
						return "west";
					default:
						return "invalid";
			}
		}

		static void Main() {
			mainMenu();
			}
		}
