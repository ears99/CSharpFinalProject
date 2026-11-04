/*
	NOTE FROM JACOB SEADORF -- PLEASE READ!
	I figure that a small little role playing game would be doable. Nothing like a roguelike, but a simple text adventure.
	(This is something that Jacob Powell and I had discussed over a zoom call)
	
	Maybe the player could still encounter monsters and enemies? 
	If it's not needed, health could be removed, but I added it just in case we end up using it.
 */

using System;
using System.Security.Cryptography;
using System.Xml.Linq;
using static System.Console;

class Player {
  public Player() {
		this.health = health;
		this.mana = mana;
		this.name = name;
		//maybe roll a random number for player health and mana?
	}

	public int health { get; set; }
	public int mana { get; set; }
	public string name { get; set; }
	public int gold { get; set; }
	public bool isPlayerDead { get; set; }

	private int _health;
	private int _mana;
	private string _name;
	private int _gold;
	private bool _playerDead;

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

class HealthOrManaException : Exception {
	public HealthOrManaException(int num) : base("Health or Mana cannot be zero on initialization.") {}
}

class Program { 

	static void mainMenu() {
		WriteLine("\tMain Menu");
		WriteLine("\t============");
		WriteLine("\t1) Play");
		WriteLine("\t2) Credits");
		WriteLine("\t3) Quit");
	   int menuChoice = Convert.ToInt32(ReadLine());
		switch(menuChoice) {
			case 1:
					Console.Clear(); //clear the screen before the game starts. Makes it so that once the game starts, the menu doesn't show up.
					gameLoop(); //
				break;
			case 2:
					Console.Clear();
					WriteLine("\tThis game was created for a final project at GRCC.");
					WriteLine("\t==================================================\n");
					WriteLine("\tThe team members for this project were:");
					WriteLine("\t* Jacob Seadorf");
					WriteLine("\t* Jacob Powell");
					WriteLine("\t* Brandon Choang");
					mainMenu();	
				break;
			case 3:
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
		//maybe an array of strings containing room descriptions, like a choose your own adventure book?
		//maybe a random number generator, say 1 to 6, and if it rolls a certain number, the player encounters a monster?

		bool playerHasKey = false; //player does not have the key when they start the game.


        Random rng = new Random(); //random number generator
		int d6 = rng.Next(1, 7); //roll 1 to 6
		int d20 = rng.Next(1, 21);

        Player player = new Player();
        player.isPlayerDead = false;

		int playerHealthRoll = rng.Next(25, 51); //number between 25 and 50

		player.health = playerHealthRoll;
		if(playerHealthRoll <= 0) { throw new HealthOrManaException(player.health); } //check to make sure health actually got assigned properly

		//get the player character's name
		Write("What is your name, adventurer? ");
		player.name = ReadLine();

		//check if name is "" or " ", throw an exception if the name is an empty string or whitespace.
		if(string.IsNullOrWhiteSpace(player.name)) {
			throw new NameException(player.name);
		} else { WriteLine("Go forth, " + player.name + "!"); }


        //maybe a simple way of processing the direction is for the player to type N S E W (so that lowercase n can be reserved for "Y/n")

        string[] rooms = new string[100];
        string response; //string for processing Y/n questions.
        //rooms for special circumstances are best, in my opinion, left at the end of the array. We can jump back to the most recent room before the circumstance
        rooms[97] = "You flee from the monster!"; //maybe a random chance to not flee?
        rooms[99] = "You have left the dungeon of your own free will.\n\nGAME OVER"; //player enters S in the first room
        rooms[98] = "You died!\n\nGAME OVER"; //self explanatory 
		rooms[96] = "You do not have the key."; //player does not have the key for a door
		rooms[95] = "Alright then."; //player enters lowercase n for a Y/n option.

        rooms[0] = "ROOM 1\nTo the north you see a locked door. You do not have the key...\nOptions are: N E S"; //first room, no key.
        rooms[1] = "ROOM 1\nTo the north you see a locked door. Use the key? (Y/n)"; //first room, has the key.
        rooms[2] = "ROOM 2\nTo the west you see the room with the locked door." +
            "\nIn this room, you see a fountain. As you walk over to the fountain, you see a key shimmering in the water. Pick up the key? (Y/n)";
        rooms[3] = "ROOM 3\nOpening the door, you see a set of stairs. Going down the stairs leads further into the dungeon. Treasure and danger await...";
        rooms[4] = "ROOM 4\nGoing down the stairs, you see a monster. Attack the monster? (Y/n)";

        int currentRoom = 0; //display the text for the current room.
        WriteLine(rooms[currentRoom]); //write the corrosponding room text
		
		//TODO: add logic to change rooms
	}

    static void Main() {
		mainMenu();
	}
}
