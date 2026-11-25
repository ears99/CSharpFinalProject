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
using System.Collections.Generic;
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
		health = 20; //Magic The Gathering reference, haha
		PreviousRoom = null;
		PlayerHasKey = false;
		IsPlayerDead = false;
		score = 0; //gold and general progress; getting the Gem of Power rewards 500; finding a hidden room rewards 100.
		GameOver = false;
	}

	public Room CurrentRoom { get; set; }
	public Room PreviousRoom { get; set; }
	public bool PlayerHasKey { get; set; }
	public string Name { get; set; }
	public bool IsPlayerDead { get; set; }
	public bool GameOver { get; set; }
	public int health { get; set; }

	public int score { get; set; }

	public HashSet<string> inventory = new HashSet<string>(); //inventory for storing items?
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
			Console.Clear();
			WriteLine(descriptions[90]); //"You cannot go that way"
			return;
		}

		//remember previous room
		player.PreviousRoom = player.CurrentRoom;
		//actually move the player
		player.CurrentRoom = nextRoom;
		//mark the new room as visited
		player.CurrentRoom.Visited = true;

		if(player.CurrentRoom.RoomNumber == 3) {
			//player is in a library
			player.inventory.Add("Magical Book");
		}

		if(player.CurrentRoom.RoomNumber == 19) {
			//player has found the Gem of Power
			player.inventory.Add("Gem of Power");
			WriteLine(descriptions[96]);
			player.GameOver = true;
		}

		//display new room description
		Console.Clear();
		//WriteLine(descriptions[player.CurrentRoom.DescriptionIndex]);
	}


	static void mainMenu() {
		WriteLine("\t===========================");
		WriteLine("\tQUEST FOR THE GEM OF POWER");
		WriteLine("\t===========================");
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
				WriteLine("The valid directions you can enter are the first letter of the direction (n, e, s, w), \nor the full word (north, east, south, west).");
				WriteLine("The game also contains an inventory system. To access the inventory, type either i or inventory.\n\nThe player input is not case-sensative.");
				WriteLine();
				WriteLine("Your quest, of course, is to go through the dungeon. Find riches and glory, \nand maybe, if the legends are true, the Gem of Power...");
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
		descriptions[91] = "You have encountered a monster!"; //player has encountered a monster
		descriptions[97] = "You flee from the monster!"; //maybe a random chance to not flee?
		descriptions[99] = "You have left the dungeon of your own free will.\n\nGAME OVER"; //player enters "south" in the first room
		descriptions[98] = "You died!\n\nGAME OVER"; //obvious what this does 
		descriptions[96] = "You have found the Gem of Power!\n\nGAME OVER";
		
		//hidden rooms
		descriptions[80] = "HIDDEN ROOM 1\nCongratulations! You've found a hidden room! To the east is the hallway with the stone carvings.\nYou've gained 100 gold pieces.";

		//regular rooms are from 0-89, plenty of different options we could do.
		descriptions[0] = "ROOM 0\nTo the north you see an old wooden door. To the west, you see a library. \nTo the east, you see a room with a fountain. \nTo the south, you see the exit."; //first room, player starts here
		descriptions[1] = "ROOM 1\nTo the south, you see the first room of the dungeon. To the north, you see what appears to be an armory. \nThis room appears to just be a hallway. " +
			"\nOn either side of you, the cracking stone walls show intricate carvings of a mystical gem."; //first hallway, hidden room on the west side
		descriptions[2] = "ROOM 2\nTo the west you see the beginning of the dungeon." +
			"\nIn this room, you see a fountain. As you dip your hand into the water, you feel rejuvinated. \nYour health has been fully restored.";
		descriptions[3] = "ROOM 3\nTo the east, you see the beginning of the dungeon. \nThis room appears to be a library. One book in particular seems to call to you, " +
			"\nand as you pull it off the shelf, you turn to the first page: \n\"Libraries give hints to the player as they explore the dungeon.\". You're not sure who this player is, however.";
		descriptions[4] = "ROOM 4\nThis room appears to be an armory. Weapons and armor are all around you. \nTo the south is the hallway with the stone carvings. " +
			"\nTo the west is a room that appears to be long-abandoned guard quarters. To the east appears to be a makeshift kitchen."; //armory, after room/hallway 1
		descriptions[5] = "ROOM 5\nIn this room, you see rows of beds. \nThis appears to be long-abandoned guard barracks.\nTo the east, you see the armory, probably used by the guards.";
		descriptions[6] = "ROOM 6\nThis room is a makeshift kitchen. You see a long extinguished fireplace, meat hooks, and long-empty barrels, \npotentially holding dried provisions.\nTo the west, you see the armory. To the east, you see a dining room.";
		descriptions[7] = "ROOM 7\nThis room is a dining room. \nDining chairs sit around a table, although there are no plates or silverware on the table, as this room has clearly long been abandoned.";
		descriptions[8] = "ROOM 8\n";
		descriptions[9] = "ROOM 9\n";
		descriptions[10] = "ROOM 10\n";
		descriptions[11] = "ROOM 11\n";
		descriptions[12] = "ROOM 12\n";
		descriptions[13] = "ROOM 13\n";
		descriptions[14] = "ROOM 14\n";
		descriptions[15] = "ROOM 15\n";
		descriptions[16] = "ROOM 16\n";
		descriptions[17] = "ROOM 17\n";
		descriptions[18] = "ROOM 18\n";
		descriptions[19] = "ROOM 19\nIn the center of the room, you see a pillar. On top of the pillar sits the Gem of Power, the object of your quest.\n";

		//create the rooms
		Room startingRoom = new Room(0, 0);
		Room firstFountain = new Room(2, 2); //Fountains restore the player's health
		Room firstLibrary = new Room(3, 3); //libraries give hints or information to the player
		Room firstHallway = new Room(1, 1); //cracked/crumbling walls are a good hint to the player about hidden things
		Room firstHidden = new Room(80, 80); //first hidden room in the first hallway
		Room armory = new Room(4, 4);
		Room oldBarracks = new Room(5, 5);
		Room makeshiftKitchen = new Room(6, 6);
		Room diningRoom = new Room(7, 7);


		//connect rooms in both directions, so the player can move between them
		startingRoom.East = firstFountain;
		firstFountain.West = startingRoom;
		
		startingRoom.West = firstLibrary;
		firstLibrary.East = startingRoom;

		firstHallway.South = startingRoom;
		startingRoom.North = firstHallway;

		firstHallway.West = firstHidden;
		firstHidden.East = firstHallway;

		firstHallway.North = armory;
		armory.South = firstHallway;



		Write("What is your name? ");
		string playerName = ReadLine();
			
		//check if player name is empty
		if(String.IsNullOrWhiteSpace(playerName)) { throw new NameException(playerName); }
		WriteLine("Go forth, " + playerName + "!");

		//actually create the player and start the game
		Player player = new Player(playerName, startingRoom);
		player.CurrentRoom.Visited = true;

		Console.Clear();
		while(player.IsPlayerDead == false && player.GameOver == false) {
			WriteLine(descriptions[player.CurrentRoom.DescriptionIndex]);
			Write("Please enter a command >> ");
			string input = ReadLine();
			string output = processInput(input);
			
			//check if player wants to print inventory or move
			if(output == "inventory") {
				PrintInventory(player.inventory);
			} else {
				//string direction = processInput(input);
				movePlayer(player, output, descriptions);
			}

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
				case "I":
				case "i":	
				case "inventory":
				case "Inventory":
				case "INVENTORY":
					return "inventory";
				default:
					return "invalid";
			}
		}

	static void PrintInventory(HashSet <string> l) {
		if(l.Count == 0) { 
			WriteLine("Your inventory is empty. Explore the dungeon to find items."); 
		} else {
			Console.Clear();
			WriteLine("INVENTORY");
			WriteLine("=========");
			foreach(string item in l) {
				//print all the items in the array
				WriteLine("* " + item);
			}
		}
	}


		static void Main() {
			mainMenu();
			}
		}
