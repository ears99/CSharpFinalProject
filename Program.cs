/*
	NOTE FROM JACOB SEADORF -- PLEASE READ!
	I figure that a small little role playing game would be doable. Nothing like a roguelike, but a simple text adventure.
	(This is something that Jacob Powell and I had discussed over a zoom call)
	
	Maybe the player could still encounter monsters and enemies? 
	If it's not needed, health could be removed, but I added it just in case we end up using it.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Numerics;
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

	public bool InFountainRoom { get; set; } //player is in a room with a fountain; have a 0% chance for combat so the player can heal
}
class Player {
	public Player(string name, Room startingRoom) {
		Name = name;
		CurrentRoom = startingRoom;
		health = 20; //Magic The Gathering reference, haha
		PreviousRoom = null;
		PlayerHasKey = false;
		IsPlayerDead = false;
		gold = 0;
		score = 0; //gold and general progress; getting the Gem of Power rewards 500; finding a hidden room rewards 100.
		GameOver = false;
	}

	public Room CurrentRoom { get; set; }
	public Room PreviousRoom { get; set; }
	public bool PlayerHasKey { get; set; }
	public string Name { get; set; }
	public bool IsPlayerDead { get; set; }
	public bool GameOver { get; set; }
	public bool playerWin { get; set; }
	public int health { get; set; }

	public int score { get; set; }

	public int gold { get; set; }

	public HashSet<string> inventory = new HashSet<string>(); //inventory for storing items?
}

class Enemy {
	public void EnemyTaunt() {
		WriteLine("GIMME ALL YOUR MONEY!!!");
	}

	public void Confrontation(Player player) {
		EnemyTaunt();
		WriteLine("What is your action? R for run, A for attack, B for bribe >>");
		string Response = ReadLine();

		// Placeholder response actions... 

		if(Response == "A" || Response == "a") {

			// Note: I am considering using inheritence for different types of enemies, 
			// but would have to refresh my memory on how to do it. -JP 11/16/26 

			if(Response == "A" || Response == "a") {

				//maybe roll a random number for damage, and subtract from the player's health?
				// I think this does the two things you mentioned. JP 11/16/26

				Random rng = new Random();
				// Random num for damage |
				//                       V
				int damage = rng.Next(5, 16);
				WriteLine("The enemy attacks you for " + damage + " damage!");
				// Damage is subtracted from player health.
				//             |
				//             V

				player.health -= damage;
				WriteLine("Your remaining health: " + player.health);


			} else if(Response == "B" || Response == "b") {
				player.score -= 50; //player loses score (gold) for bribing enemy
				WriteLine("He took literally everything you had... but you lived");


			} else if(Response == "R" || Response == "r") {
				WriteLine("Somehow you escaped...");


			} else {
				WriteLine("You froze! The perpetrator kidnapped you!");
			}
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
			Console.Clear();
			//player enters south in the first room, exit the dungeon
			WriteLine(descriptions[99]);
			player.GameOver = true;
			return;
		}

		//if player is in the riddle room and tries to go north
		if(player.CurrentRoom.RoomNumber == 18 && direction.ToLower() == "north") {
			Console.Clear();
			WriteLine("You must answer the riddle to go forward from here.");
			return;
		}

		if(nextRoom == null) {
			Console.Clear();
			WriteLine(descriptions[90]); //"I'm sorry, I don't understand."
			return;
		}

		//remember previous room
		player.PreviousRoom = player.CurrentRoom;
		//actually move the player
		player.CurrentRoom = nextRoom;

		if(!player.CurrentRoom.Visited) {
			player.score += 10; //only increase score for newly visited rooms
		}

		//mark the new room as visited
		player.CurrentRoom.Visited = true;


		//player is in a library
		if(player.CurrentRoom.RoomNumber == 3 || player.CurrentRoom.RoomNumber == 8) {
			player.inventory.Add("Magical Book");
		}

		//player has discovered first hidden room
		if(player.CurrentRoom.RoomNumber == 80 || player.CurrentRoom.RoomNumber == 81 || player.CurrentRoom.RoomNumber == 82) {
			player.score += 100;
		}
		
		//player has discovered second hidden room
		if(player.CurrentRoom.RoomNumber == 80 || player.CurrentRoom.RoomNumber == 81 || player.CurrentRoom.RoomNumber == 82) {
			player.score += 200;
		}

		//player has discovered third hidden room
		if(player.CurrentRoom.RoomNumber == 82) {
			player.score += 250;
		}

		//player is in a room with a fountain
		if(player.CurrentRoom.RoomNumber == 2 || player.CurrentRoom.RoomNumber == 9) {
			player.CurrentRoom.InFountainRoom = true;
			player.inventory.Add("Coin");
			//player health is less than 20 but greater than 0
			if(player.health > 0 && player.health < 20) {
				player.health = 20;
			}
		} else { player.CurrentRoom.InFountainRoom = false; }
		

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
				WriteLine("Your score is measured by the number of rooms you visit, how many enemies defeated in combat, and if you discover any secrets.");
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
		descriptions[90] = "I'm sorry, I don't understand."; //obvious what this does	
		descriptions[91] = "You have encountered a monster!"; //player has encountered a monster
		descriptions[97] = "You flee from the monster!"; //maybe a random chance to not flee?
		descriptions[99] = "You have left the dungeon without the Gem of Power, never knowing if it truly exists or not."; //player enters "south" in the first room
		descriptions[98] = "You died!"; //obvious what this does 

		//hidden rooms
		descriptions[80] = "HIDDEN ROOM 1\nCongratulations! You've found a hidden room! To the east is the hallway with the stone carvings.\nYou've gained 100 gold pieces.";
		descriptions[81] = "HIDDEN ROOM 2\nPushing on the stone carving, you have discovered a hidden room! To the west is the hallway.\nYou've gained 200 gold pieces";
		descriptions[82] = "HIDDEN ROON 3\nCongratulations! You've found a hidden room! To the south is the hallway.\nYou've gained 250 gold pieces.";
				
		//regular rooms, plenty of different options we could do.
		descriptions[0] = "ROOM 0\nTo the north you see an old wooden door. To the west, you see a library. \nTo the east, you see a room with a fountain. \nTo the south, you see the exit.\n\nNOTE: Every time you enter a room, you are always facing north."; //first room, player starts here
		descriptions[1] = "ROOM 1\nTo the south, you see the first room of the dungeon. To the north, you see what appears to be an armory. \nThis room appears to just be a hallway. " +
			"\nOn either side of you, the cracking stone walls show intricate carvings of a mystical gem."; //first hallway, hidden room on the west side
		descriptions[2] = "ROOM 2\nTo the west you see the beginning of the dungeon." +
			"\nIn this room, you see a fountain. As you dip your hand into the water, you feel rejuvinated. \nYour health has been fully restored.";
		descriptions[3] = "ROOM 3\nTo the east, you see the beginning of the dungeon. \nThis room appears to be a library. One book in particular seems to call to you, " +
			"\nand as you pull it off the shelf, you turn to the first page: \n\"Libraries give hints to the player as they explore the dungeon.\". You're not sure who this player is, however.";
		descriptions[4] = "ROOM 4\nThis room appears to be an armory. Weapons and armor are all around you. \nTo the south is the hallway with the stone carvings. " +
			"\nTo the west is a room that appears to be long-abandoned guard quarters. To the east appears to be a makeshift kitchen.";
		descriptions[5] = "ROOM 5\nIn this room, you see rows of beds. \nThis appears to be long-abandoned guard barracks.\nTo the east, you see the armory, probably used by the guards.";
		descriptions[6] = "ROOM 6\nThis room is a makeshift kitchen. You see a long extinguished fireplace, meat hooks, and long-empty barrels, \npotentially holding dried provisions.\nTo the west, you see the armory. To the east, you see a dining room.";
		descriptions[7] = "ROOM 7\nThis room is a dining room. \nDining chairs sit around a table, although there are no plates or silverware on the table, as this room has clearly long been abandoned.\nTo the north is another hallway. To the west is the kitchen.";
		descriptions[8] = "ROOM 8\nThis appears to be another hallway. To the south is the dining room, to the north is a room with a fountain.\nThe walls are covered in intricate stone carvings showing just how powerful the Gem is rumored to be.\nOne carving on the western wall shows a fearsome dragon attacking a village, \nand another on the eastern wall shows the dragon being driven away from the village by the Gem.\nThe Gem of Power, if it exists, is clearly very powerful.";
		descriptions[9] = "ROOM 9\nThis room has a fountain in the center. As you approach the fountain, you feel rejuvinated. \nLooking into the fountain, you see many gold coins, presumably thrown into the fountain by the guards. \nTo the north is a library, to the west and east there are other rooms. To the south is the hallway.";
		descriptions[10] = "ROOM 10\nThis room is another hallway. The walls are covered in glowing mushrooms, \nwhich provide some light as you walk down the passageway. \nOn the cracked stone walls you see stone carvings of the Gem granting wealth, power, and prestige to those who own it.";
		descriptions[11] = "ROOM 11\nThis room seems to be a storage room -- tools, dried food, and other miscellaneous items are scattered around the room. The only exit is to the west.";
		descriptions[12] = "ROOM 12\nThis room appears to be a library. Opening a book, the following text suddenly appears, as if by magic: \n\n\"There are hidden rooms in the dungeon. Exploration is rewarded.\"";
		descriptions[13] = "ROOM 13\nThis room is a winding hallway, with metal braziers hanging from chains. They seem to be lit as if by magic. \nTo the east is the hallway with glowing mushrooms, and to the north is a burial chamber.";
		descriptions[14] = "ROOM 14\nThis room appears to be a burial chamber. \nSarcophagi are arranged along the western and eastern walls, and carved into the walls are stone murals \ndepicting knights defending the Gem of Power in battle. \nTo the south is the winding hallway, and to the north is another hallway.";
		descriptions[15] = "ROOM 15\nThis room is a long hallway, with statues of knights along the western wall. \nThe walls and statues are covered in moss and vine, having long been abandoned. \nTo the east is a chapel, to the south is another hallway, to the north is a mysterious room...";
		descriptions[16] = "ROOM 16\nThis room appears to be a chapel of some kind. \nIn the middle of the room is an old altar, and to the north is an old wooden door. \nTo the west is the hallway with statues.";
		descriptions[17] = "ROOM 17\nThis room appears to be a crypt. Sarcophagi are placed in alcoves carved into the stone walls. \nThe sarcophagi clearly hold the remains of the guards who lived here.\nThere is a room to the west, and a room to the south.";
		descriptions[18] = "ROOM 18\nBefore you stands a stone wall inscribed with a riddle carved in runes: \n\n\"I have a golden head and golden tail, but no body. What am I?\"\nTo the south is the hallway with statues, and to the east is the chapel.";
		descriptions[19] = "ROOM 19\nYou are worthy.\nThe stone wall splits down the middle, as if by magic, allowing you to pass through.\n\nIn the center of this room, you see a stone pillar surrounded by jewels, gold, and other treasures.\nOn top of the pillar sits the Gem of Power, glowing with magical energy. \nThe room's walls are decorated with every stone carving you saw earlier in the dungeon.\n";

		//create the rooms
		Room startingRoom = new Room(0, 0);
		Room firstHallway = new Room(1, 1); //cracked/crumbling walls are a good hint to the player about hidden things
		Room firstFountain = new Room(2, 2); //Fountains restore the player's health
		Room firstLibrary = new Room(3, 3); //libraries give hints or information to the player
		Room armory = new Room(4, 4);
		Room oldBarracks = new Room(5, 5);
		Room makeshiftKitchen = new Room(6, 6);
		Room diningRoom = new Room(7, 7);
		Room secondHallway = new Room(8, 8);
		Room secondFountain = new Room(9, 9); //player can go n, w, e, or s here
		Room thirdHallway = new Room(10, 10);
		Room storageRoom = new Room(11, 11);
		Room secondLibrary = new Room(12, 12);
		Room room13 = new Room(13, 13);
		Room room14 = new Room(14, 14);
		Room room15 = new Room(15, 15);
		Room room16 = new Room(16, 16);
		Room burialRoom = new Room(17, 17);
		Room riddleRoom = new Room(18, 18);
		Room gemRoom = new Room(19, 19);

		//hidden rooms
		Room firstHidden = new Room(80, 80); //first hidden room is west of room 1 (first hallway)
		Room secondHidden = new Room(81, 81); //second hidden room is north of room 10 (third hallway)
		Room thirdHidden = new Room(82, 82); //third hidden room is east of room 8 (second hallway)
		

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

		armory.West = oldBarracks;
		oldBarracks.East = armory;

		armory.East = makeshiftKitchen;
		makeshiftKitchen.West = armory;

		makeshiftKitchen.East = diningRoom;
		diningRoom.West = makeshiftKitchen;

		diningRoom.North = secondHallway;
		secondHallway.South = diningRoom;

		secondHallway.North = secondFountain;
		secondFountain.South = secondHallway;

		secondHallway.East = secondHidden;
		secondHidden.West = secondHallway;

		secondFountain.North = secondLibrary;
		secondLibrary.South = secondFountain;

		secondFountain.West = thirdHallway;
		thirdHallway.East = secondFountain;

		thirdHallway.North = thirdHidden;
		thirdHidden.South = thirdHallway;

		thirdHallway.West = room13;
		room13.East = thirdHallway;

		room13.North = room14;
		room14.South = room13;

		room14.North = room15;
		room15.South = room14;

		room15.North = riddleRoom;

		room15.East = room16;
		room16.West = room15;

		room16.North = burialRoom;
		burialRoom.South = room16;

		burialRoom.West = riddleRoom;
		riddleRoom.East = burialRoom;

		riddleRoom.South = room15;
		riddleRoom.East = burialRoom;

		riddleRoom.North = gemRoom;
		gemRoom.South = riddleRoom;


		secondFountain.East = storageRoom;
		storageRoom.West = secondFountain;


		Write("What is your name? ");
		string playerName = ReadLine();

		//check if player name is empty
		if(String.IsNullOrWhiteSpace(playerName)) { throw new NameException(playerName); }


		//actually create the player and start the game
		Player player = new Player(playerName, startingRoom);
		player.CurrentRoom.Visited = true;

		Console.Clear();
		WriteLine("Go forth, " + playerName + "!");
		WriteLine();

		//game loop
		while(player.IsPlayerDead == false || player.GameOver == false || player.playerWin == false) {
			WriteLine(descriptions[player.CurrentRoom.DescriptionIndex]);
			Write("Please enter a command >> ");
			string input = ReadLine();
			string output = processInput(input);

			//process input from the player
			if(output == "inventory") {
				PrintInventory(player.inventory);
			} else if(output == "quit") {
				//player quits the game prematurely.
				WriteLine("Your score is: " + player.score + " points.");
				WriteLine("GAME OVER");
				Environment.Exit(0);
			} else if(output == "coin" && player.CurrentRoom.RoomNumber == 18) {
				//player has entered the right answer to the riddle AND is in the correct room
				Console.Clear();
				WriteLine(descriptions[19]);
				player.inventory.Add("Gem of Power");
				player.score += 1000;
				player.playerWin = true;
			} else if(output == "die") {
				//TODO: remove this if-statement when done with testing
				player.health = -1;
			} else { 
				//player has entered a direction (north, east, south, west)
				string direction = processInput(input);
				movePlayer(player, output, descriptions);
			}

			//player has died
			if(player.health <= 0) {
				WriteLine(descriptions[98]); //"You have died, GAME OVER
				player.GameOver = true;
			}

			if(player.GameOver == true) {
				//Display score
				WriteLine("Your score is: " + player.score + " points.");
				WriteLine("GAME OVER");
				Environment.Exit(0);
			}

			if(player.playerWin == true) {
				Console.Clear();
				WriteLine(descriptions[19]); //write the description for the gem room, then end the game
				WriteLine("Congratulations, " + player.Name + "!");
				WriteLine("You have found the Gem of Power!");
				WriteLine("Your score is: " + player.score + " points.");
				WriteLine("GAME OVER");
				Environment.Exit(0);
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

			//answer to the riddle in room 18
			case "coin":
			case "a gold coin":
			case "a coin":
			case "COIN":
			case "A GOLD COIN":
			case "A COIN":
			case "gold coin":
			case "Gold Coin":
			case "A gold coin":
			case "A Gold Coin":
				return "coin";
			case "quit":
			case "QUIT":
				return "quit";
			//TODO: remove "die" command; only used for testing game over screen
			case "die":
				return "die";
			default:
				return "invalid";
		}
	}

	//uses hashsets to prevent multiple copies of the same item; every item is unique
	static void PrintInventory(HashSet<string> l) {
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
