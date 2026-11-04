/*
	NOTE FROM JACOB SEADORF -- PLEASE READ!
	I figure that a small little role playing game would be doable. Nothing like a roguelike, but a simple text adventure.
	(This is something that Jacob Powell and I had discussed over a zoom call)
	
	Maybe the player could still encounter monsters and enemies? 
	If it's not needed, health could be removed, but I added it just in case we end up using it.
 */

using System;
using System.Xml.Linq;
using static System.Console;

class Player {
  public Player() {
		this.health = health;
		this.mana = mana;
		this.name = name;
	}

	public int health { get; set; }
	public int mana { get; set; }
	public string name { get; set; }

	public bool isPlayerDead { get; set; }

	private int _health;
	private int _mana;
	private string _name;

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
            WriteLine("You died... He had a bazooka");


        } else if(Response == "B" || Response == "b") {
            WriteLine("He took literally everything you had... but you lived");


        } else if(Response == "R" || Response == "r") {
            WriteLine("Somehow you escaped...");


        } else {
            WriteLine("You froze! The perpetrator kidnapped you!");
        }
    }

}
// End of my tiny addition JP 



class StringException : Exception { 
	public StringException(string name) : base("Name cannot be empty.") { }
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
				//Environment.Exit(0); //exits the program for simplicity so I don't have to use a GOTO.
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
		
		Player player = new Player();
        player.isPlayerDead = false;
		
		//get the player character's name
		Write("What is your name, adventurer? ");
		player.name = ReadLine();

		//check if name is "" or " ", throw an exception if the name is an empty string or whitespace.
		if(string.IsNullOrWhiteSpace(player.name)) {
			throw new StringException(player.name);
		} else { WriteLine("Go forth, " + player.name + "!"); }

		while(player.isPlayerDead == false) { //while player is still alive, continue the game
			if(player.isPlayerDead == true) {
				WriteLine("You have died!");
				Environment.Exit(0); //exit the game if the player is dead
			}	
		}
	}

    static void Main() {
		mainMenu();
	}
}
